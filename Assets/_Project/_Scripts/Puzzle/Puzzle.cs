using Racer.SaveManager;
using Racer.SaveSystem;
using Racer.SoundManager;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Initializes a list of grids(square elements) and update their various positions.
/// </summary>
public class Puzzle : MonoBehaviour
{
    int y_count;

    int perAxisCount;

    int x_Count;

    int shuffleAmount;

    // x==y
    const int dimension = 4;

    bool isPlaying;

    Vector2 lastMove;

    //2D array
    SquareElement[,] squareElements;

    List<Sprite> sprites;

    Sprite emptySprite;

    [SerializeField]
    SquareElement sqrElementPrefab;

    [Space(5)]

    [SerializeField]
    ParticleSystem winFx;

    [Space(5), SerializeField]
    AudioClip moveSfx;


    private void Awake()
    {
        // Randomly shuffles the puzzle between these values
        shuffleAmount = Random.Range(100, 500);

        // Retrieving an item from a scriptable object
        //PuzzlePopulator pp = ItemManager.Instance.GetPuzzle(saveManager.LoadInt("CurrentItemUsed"));
        PuzzlePopulator pp = ItemManager.Instance.GetPuzzle(SaveSystem.GetData<int>("CurrentItemUsed"));

        // Sprites set to the item's property
        sprites = pp.GetSplittedImage(SaveManager.GetInt("Difficulty"));

        emptySprite = pp.GetGetEmptySprite(SaveManager.GetInt("Difficulty"));
    }


    private void Start()
    {
        GameManager.OnCurrentState += GameManager_OnCurrentState;

        // 4=4=4=4
        x_Count = y_count = perAxisCount = dimension;

        // Array initialized to dimension of 4X4(4-rows/columns)
        squareElements = new SquareElement[x_Count, y_count];


        Init();
    }


    void ShufflePuzzle()
    {
        for (int i = 0; i < shuffleAmount; i++)
            Shuffle();
    }


    private void GameManager_OnCurrentState(GameStates state)
    {
        switch (state)
        {
            case GameStates.Playing:
                isPlaying = true;
                ShufflePuzzle();
                break;
            case GameStates.GameOver:
                winFx.Play();
                break;
        }
    }

    /// <summary>
    /// Initializes each puzzle element(square) property
    /// </summary>
    public void Init()
    {
        int index = 0;

        int end = y_count - 1;

        for (int y = end; y >= 0; y--)
        {
            for (int x = 0; x < x_Count; x++)
            {
                var sqrElementClone = Instantiate(sqrElementPrefab, transform);

                sqrElementClone.Init(x, y, index + 1, sprites[index], ClickToSwap, CheckDelayCompleted, emptySprite);

                squareElements[x, y] = sqrElementClone;

                index++;

            }
        }
    }

    /// <summary>
    /// Swaps two positions
    /// </summary>
    /// <param name="x">initial x-position</param>
    /// <param name="y">initial y-position</param>
    /// <param name="dX">final x-position</param>
    /// <param name="dY">final y-position</param>
    /// <param name="hasInitialized">instant or smooth update</param>
    void Swap(int x, int y, int dX, int dY, bool hasInitialized)
    {
        // current position to exchange
        var from = squareElements[x, y];
        // new position to exchange
        var target = squareElements[x + dX, y + dY];


        if (from == target)
            return;

        // Swap these two squares
        squareElements[x, y] = target;
        squareElements[x + dX, y + dY] = from;

        // Smoothly Update their positions if initialization is completed
        // Otherwise instantly update them
        if (hasInitialized)
        {
            from.SmoothUpdatePos(x + dX, y + dY);
            target.SmoothUpdatePos(x, y);

            SoundManager.Instance.PlaySfx(moveSfx);
        }

        else
        {
            from.InstantUpdatePos(x + dX, y + dY);
            target.InstantUpdatePos(x, y);
        }

    }

    /// <summary>
    /// Sets two squares to a new position as long as game is not over
    /// </summary>
    /// <param name="x">new x-position</param>
    /// <param name="y">new y-position</param>
    void ClickToSwap(int x, int y)
    {
        if (!isPlaying)
            return;

        int dX = GetX(x, y);
        int dY = GetY(x, y);

        Swap(x, y, dX, dY, true);

    }




    int GetX(int x, int y)
    {
        int start = x_Count - 1;

        //Move to Right
        if (x < start && squareElements[x + 1, y].IsEmpty)
            return 1;
        //Move to Left
        if (x > 0 && squareElements[x - 1, y].IsEmpty)
            return -1;

        //Don't move
        return 0;
    }

    int GetY(int x, int y)
    {
        int start = y_count - 1;

        //Move up
        if (y < start && squareElements[x, y + 1].IsEmpty)
            return 1;
        //Move down
        if (y > 0 && squareElements[x, y - 1].IsEmpty)
            return -1;

        //Don't move
        return 0;
    }

    /// <summary>
    /// Shuffles the squares randomly
    /// </summary>
    void Shuffle()
    {
        for (int x = 0; x < x_Count; x++)
        {
            for (int y = 0; y < y_count; y++)
            {
                if (squareElements[x, y].IsEmpty)
                {
                    Vector2 pos = GetValidMovePos(x, y);

                    Swap(x, y, (int)pos.x, (int)pos.y, false);
                }

            }
        }
    }

    /// <summary>
    /// Moves a square to available valid position
    /// Could be up, down, left, right (per-call)
    /// </summary>
    Vector2 GetValidMovePos(int x, int y)
    {
        int dir = 4;

        Vector2 pos;

        do
        {
            int count = Random.Range(0, dir);

            if (count == 0)
                pos = Vector2.left;
            else if (count == 1)
                pos = Vector2.right;
            else if (count == 2)
                pos = Vector2.up;
            else
                pos = Vector2.down;

        } while (!(IsValidPos(x + (int)pos.x) && IsValidPos(y + (int)pos.y)) || IsRepeatMove(pos));

        lastMove = pos;

        return pos;
    }

    bool IsValidPos(int count) => count >= 0 && count < perAxisCount;

    bool IsRepeatMove(Vector2 pos) => pos * -1 == lastMove;


    /// <summary>
    /// True if all the squares are matched correctly to their initial positions before shuffling
    /// </summary>
    bool GetCheckWin
    {
        get
        {
            bool check = false;

            foreach (var item in squareElements)
            {
                if (item.IsOnCurrentPos())
                    check = true;
                else
                {
                    check = false;

                    break;
                }
            }

            return check;
        }
    }

    /// <summary>
    /// If all squares are matched completely, change the current game-state
    /// </summary>
    void CheckDelayCompleted()
    {
        if (GetCheckWin)
        {
            GameManager.Instance.SetGameState(GameStates.GameOver);
        }
    }



    // Stops listening to event
    private void OnDestroy()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;
    }
}