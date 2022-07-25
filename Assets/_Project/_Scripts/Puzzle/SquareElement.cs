using System;
using System.Collections;
using UnityEngine;

public class SquareElement : MonoBehaviour
{
    float elapsedTime;

    int index;

    int x;
    int y;

    Vector2 initialPos;
    Vector2 currentPos;


    SpriteRenderer spriteRenderer;

    Sprite emptySprite;


    Action<int, int> OnClicked = delegate { };

    Action DelayCallback = delegate { };


    [SerializeField]
    float smoothMoveDuration = .1f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameManager.OnCurrentState += GameManager_OnCurrentState;
    }

    private void GameManager_OnCurrentState(GameStates currentState)
    {
        if (currentState == GameStates.GameOver)
        {
            SetSpriteOnGameover();
        }
    }

    /// <summary>
    /// Initializes this gameobject's properties when the game starts
    /// </summary>
    /// <param name="x">this x position</param>
    /// <param name="y">this y position</param>
    /// <param name="index">this current index</param>
    /// <param name="sprite">this current sprite</param>
    /// <param name="ClickToSwap">Callback when mouse is clicked</param>
    /// <param name="CheckDelayCallback">Callback when smooth-move is completed</param>
    /// <param name="emptySprite">this empty sprite</param>
    public void Init(int x, int y, int index, Sprite sprite, Action<int, int> ClickToSwap, Action CheckDelayCallback, Sprite emptySprite)
    {
        this.index = index;

        this.emptySprite = emptySprite;

        spriteRenderer.sprite = sprite;

        InstantUpdatePos(x, y);

        initialPos = new Vector2(x, y);

        currentPos = initialPos;

        OnClicked = ClickToSwap;

        DelayCallback = CheckDelayCallback;
    }

    /// <summary>
    /// Moves to a new empty position instantly
    /// </summary>
    /// <param name="x">new x-position</param>
    /// <param name="y">new y-position</param>

    public void InstantUpdatePos(int x, int y)
    {
        this.x = x;
        this.y = y;

        Vector2 newPos = new Vector2(x, y);

        transform.position = newPos;

        currentPos = newPos;
    }

    /// <summary>
    /// Move to a new empty position in a smooth manner
    /// </summary>
    public void SmoothUpdatePos(int x, int y)
    {
        this.x = x;
        this.y = y;


        StartCoroutine(SmoothMove());
    }

    IEnumerator SmoothMove()
    {
        elapsedTime = 0;

        Vector2 oldPos = transform.position;

        Vector2 newPos = new Vector2(x, y);

        while (elapsedTime < smoothMoveDuration)
        {
            Vector2 pos = Vector2.Lerp(oldPos, newPos, elapsedTime / smoothMoveDuration);

            transform.position = pos;

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = newPos;

        currentPos = newPos;

        DelayCallback?.Invoke();
    }

    // Empty if no sprite is assigned at the specified index
    public bool IsEmpty => index == 1;

    // Fills the empty-slot
    public void SetSpriteOnGameover()
    {
        if (IsEmpty)
            spriteRenderer.sprite = emptySprite;
    }

    public bool IsOnCurrentPos() => currentPos == initialPos;

    // Moves to a new empty position on mouse click
    private void OnMouseDown()
    {
        OnClicked?.Invoke(x, y);
    }

    // Stops listening to event
    private void OnDestroy()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;
    }
}
