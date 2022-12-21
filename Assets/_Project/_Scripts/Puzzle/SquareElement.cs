using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages individual square grids.
/// </summary>
public class SquareElement : MonoBehaviour
{
    private float _elapsedTime;
    private int _index;
    private int _x;
    private int _y;

    private Vector2 _initialPos;
    private Vector2 _currentPos;

    private SpriteRenderer _spriteRenderer;
    private Sprite _emptySprite;

    private Action<int, int> _onClicked;
    private Action _delayCallback;

    [SerializeField,
     Range(0, 1)]
    private float moveSpeed = .1f;


    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        GameManager.OnCurrentState += GameManager_OnCurrentState;
    }

    private void OnMouseDown()
    {
        _onClicked?.Invoke(_x, _y);
    }

    private void GameManager_OnCurrentState(GameState currentState)
    {
        if (currentState == GameState.GameOver)
            SetSpriteOnGameover();
    }

    /// <summary>
    /// Initializes this gameobject properties at startup.
    /// </summary>
    /// <param name="x">this x position</param>
    /// <param name="y">this y position</param>
    /// <param name="index">this current index</param>
    /// <param name="sprite">this current sprite</param>
    /// <param name="clickToSwap">Callback when mouse is clicked</param>
    /// <param name="checkDelayCallback">Callback when smooth-move is completed</param>
    /// <param name="emptySprite">this empty sprite</param>
    public void Init(int x,
        int y,
        int index,
        Sprite sprite,
        Action<int, int> clickToSwap,
        Action checkDelayCallback,
        Sprite emptySprite)
    {
        _index = index;
        _emptySprite = emptySprite;
        _spriteRenderer.sprite = sprite;

        InstantUpdatePos(x, y);

        _initialPos = new Vector2(x, y);
        _currentPos = _initialPos;

        _onClicked = clickToSwap;
        _delayCallback = checkDelayCallback;
    }

    /// <summary>
    /// Moves to a new empty position instantly.
    /// </summary>
    public void InstantUpdatePos(int x, int y)
    {
        _x = x;
        _y = y;

        var newPos = new Vector2(x, y);
        
        transform.localPosition = newPos;
        _currentPos = newPos;
    }

    /// <summary>
    /// Move to a new empty position in a smoothly.
    /// </summary>
    public void SmoothUpdatePos(int x, int y)
    {
        _x = x;
        _y = y;

        StartCoroutine(SmoothMove());
    }

    private IEnumerator SmoothMove()
    {
        _elapsedTime = 0;

        Vector2 oldPos = transform.localPosition;
        var newPos = new Vector2(_x, _y);

        while (_elapsedTime < moveSpeed)
        {
            var pos = Vector2.Lerp(oldPos, newPos, _elapsedTime / moveSpeed);

            transform.localPosition = pos;

            _elapsedTime += Time.deltaTime;

            yield return 0;
        }

        transform.localPosition = newPos;
        _currentPos = newPos;

        _delayCallback?.Invoke();
    }

    // True, if no sprite is assigned at the specified index.
    public bool IsEmpty => _index == 1;

    // Auto-fills up remaining empty-slot.
    public void SetSpriteOnGameover()
    {
        if (IsEmpty)
            _spriteRenderer.sprite = _emptySprite;
    }

    public bool IsOnCurrentPos() => _currentPos == _initialPos;

    private void OnDestroy()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;
    }
}
