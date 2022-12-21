using Racer.SaveSystem;
using Racer.SoundManager;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Wraps up all <see cref="SquareElement"/> logic.
/// </summary>
[DefaultExecutionOrder(-13)]
internal class Puzzle : MonoBehaviour
{
    private int _yCount;
    private int _xCount;
    private int _perAxisCount;
    private int _shuffleAmount;
    private bool _isPlaying;

    private Vector2 _lastMove;
    private Sprite _emptySprite;

    private SquareElement[,] _squareElements;
    private List<Sprite> _sprites;

    public PuzzleItem PuzzleItem { get; private set; }

    [Header("MISC"), Space(5)]
    [SerializeField] private SquareElement sqrElementPrefab;
    [SerializeField] private ParticleSystem winFx;
    [SerializeField] private AudioClip moveSfx;


    private void Awake()
    {
        // Retrieves a puzzle item from scriptable object, based on previous used item.
        PuzzleItem = ItemManager.Instance.GetPuzzle(SaveSystem.GetData<int>(Metrics.CurrentItemInUse));

        var difficultyIndex = SaveSystem.GetData<int>(Metrics.Difficulty);

        // Retrieves the sliced squares and empty-sprite based on the difficulty level selected.
        _sprites = PuzzleItem.GetSlicedImages(difficultyIndex);
        _emptySprite = PuzzleItem.GetGetEmptySprite(difficultyIndex);

        // Shuffles the puzzle between these values.
        _shuffleAmount = Random.Range(Metrics.MaxSeed / 2, Metrics.MaxSeed);

        _xCount = _yCount = _perAxisCount = PuzzleItem.Dimension.EvenDimension.Item1;
    }

    private void Start()
    {
        GameManager.OnCurrentState += GameManager_OnCurrentState;

        // Array initialized to a custom dimension.
        _squareElements = new SquareElement[_xCount, _yCount];

        Init();
    }

    private void GameManager_OnCurrentState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                _isPlaying = true;
                ShufflePuzzle();
                break;
            case GameState.GameOver:
                _isPlaying = false;
                winFx.Play();
                break;
        }
    }

    /// <summary>
    /// Initializes each puzzle element(square) property
    /// </summary>
    public void Init()
    {
        var index = 0;

        var end = _yCount - 1;

        for (int y = end; y >= 0; y--)
        {
            for (int x = 0; x < _xCount; x++)
            {
                var sqrElementClone = Instantiate(sqrElementPrefab, transform.GetChild(0));

                sqrElementClone.Init(x, y, index + 1, _sprites[index], ClickToSwap, CheckDelayCompleted, _emptySprite);

                _squareElements[x, y] = sqrElementClone;

                index++;
            }
        }
    }

    private void ShufflePuzzle()
    {
        for (int i = 0; i < _shuffleAmount; i++)
            Shuffle();
    }

    /// <summary>
    /// Swaps between two positions.
    /// </summary>
    /// <param name="x">initial x-position</param>
    /// <param name="y">initial y-position</param>
    /// <param name="dX">final x-position</param>
    /// <param name="dY">final y-position</param>
    /// <param name="hasInitialized">Should apply an instant or smooth update after initialization?</param>
    private void Swap(int x, int y, int dX, int dY, bool hasInitialized)
    {
        // current position to exchange
        var from = _squareElements[x, y];

        // new position to exchange
        var target = _squareElements[x + dX, y + dY];

        if (from == target)
            return;

        // Swap these two squares
        _squareElements[x, y] = target;
        _squareElements[x + dX, y + dY] = from;

        // Smoothly Update their positions if initialization is completed,
        // Otherwise instantly update them.
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
    /// Interchanges two square's position.
    /// </summary>
    /// <param name="x">new x-position</param>
    /// <param name="y">new y-position</param>
    private void ClickToSwap(int x, int y)
    {
        if (!_isPlaying)
            return;

        var dX = GetX(x, y);
        var dY = GetY(x, y);

        Swap(x, y, dX, dY, true);
    }

    private int GetX(int x, int y)
    {
        var start = _xCount - 1;

        // Move to Right
        if (x < start && _squareElements[x + 1, y].IsEmpty)
            return 1;

        // Move to Left
        if (x > 0 && _squareElements[x - 1, y].IsEmpty)
            return -1;

        // Otherwise Don't move
        return 0;
    }

    private int GetY(int x, int y)
    {
        var start = _yCount - 1;

        // Move up
        if (y < start && _squareElements[x, y + 1].IsEmpty)
            return 1;

        // Move down
        if (y > 0 && _squareElements[x, y - 1].IsEmpty)
            return -1;

        // Otherwise Don't move
        return 0;
    }

    /// <summary>
    /// Shuffles the squares randomly.
    /// </summary>
    private void Shuffle()
    {
        for (int x = 0; x < _xCount; x++)
        {
            for (int y = 0; y < _yCount; y++)
            {
                if (!_squareElements[x, y].IsEmpty) continue;

                var pos = GetValidMovePos(x, y);

                Swap(x, y, (int)pos.x, (int)pos.y, false);
            }
        }
    }

    /// <summary>
    /// Moves a square to available valid position.
    /// Could be up, down, left, right (per-call).
    /// </summary>
    private Vector2 GetValidMovePos(int x, int y)
    {
        var dir = 4;
        Vector2 pos;

        do
        {
            var count = Random.Range(0, dir);

            switch (count)
            {
                case 0:
                    pos = Vector2.left;
                    break;
                case 1:
                    pos = Vector2.right;
                    break;
                case 2:
                    pos = Vector2.up;
                    break;
                default:
                    pos = Vector2.down;
                    break;
            }
        } while (!(IsValidPos(x + (int)pos.x) &&
                   IsValidPos(y + (int)pos.y)) ||
                    IsRepeatMove(pos));

        _lastMove = pos;

        return pos;
    }

    /// <summary>
    /// True if all the squares are matched correctly to their initial positions before shuffling.
    /// </summary>
    private bool CheckWin
    {
        get
        {
            var isCheck = false;

            foreach (var item in _squareElements)
            {
                if (item.IsOnCurrentPos())
                    isCheck = true;
                else
                {
                    isCheck = false;
                    break;
                }
            }

            return isCheck;
        }
    }

    private void CheckDelayCompleted()
    {
        if (CheckWin)
        {
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
    }

    private bool IsValidPos(int count) => count >= 0 && count < _perAxisCount;

    private bool IsRepeatMove(Vector2 pos) => pos * -1 == _lastMove;

    private void OnDestroy()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;
    }
}