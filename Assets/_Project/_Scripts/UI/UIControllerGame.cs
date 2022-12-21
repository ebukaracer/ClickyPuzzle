using System.Collections;
using System.Collections.Generic;
using Racer.LoadManager;
using Racer.SaveSystem;
using Racer.SoundManager;
using Racer.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

internal partial class UIControllerGame : MonoBehaviour
{
    private GameManager _gameManager;
    private Canvas _loadingBarCanvas;
    private Animator _animator;
    private PuzzleItem _puzzleItem;
    private CommentGenerator _commentGenerator;

    // This corresponds to the difficulties available in
    // the main menu. Their ordering and indexing matters.
    private readonly string[] _difficulties = { "EASY", "PRO" };

    private float _gameLoadDelay;
    private float _currentTime;
    private float _bestTime;

    private int _currentItem;
    private int _savedCash;
    private int _difficultyIndex;

    private bool _hasGameStarted;
    private bool _hasGameEnded;

    [Space(5), Header("IMAGES")]
    [SerializeField] private Image loadingBar;
    [SerializeField] private Image previewImage;
    [SerializeField] private Image previewImageZoomed;

    [Space(5), Header("TEXTS")]
    [SerializeField] private TextMeshProUGUI countdownT;
    [SerializeField] private TextMeshProUGUI highscoreT;
    [SerializeField] private TextMeshProUGUI winT;
    [SerializeField] private TextMeshProUGUI cashT;
    [SerializeField] private TextMeshProUGUI puzzleNameT;
    [SerializeField] private TextMeshProUGUI puzzleDifficultyT;

    [Space(5), Header("CLIPS")]
    [SerializeField] private AudioClip startSfx;
    [SerializeField] private AudioClip gameoverSfx;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _loadingBarCanvas = loadingBar.GetComponentInParent<Canvas>();

        _savedCash = SaveSystem.GetData<int>(Metrics.Cash);
        _currentItem = SaveSystem.GetData<int>(Metrics.CurrentItemInUse);
        _difficultyIndex = SaveSystem.GetData<int>(Metrics.Difficulty);
    }

    private void Start()
    {
        GameManager.OnCurrentState += GameManager_OnCurrentState;
        _gameManager = GameManager.Instance;
        _commentGenerator = CommentGenerator.Instance;

        loadingBar.fillAmount = 1;
        _gameLoadDelay = _gameManager.StartDelay;

        InitPuzzleProperties();

        if (_gameLoadDelay != 0)
            StartCoroutine(InitLoadingBar(_gameLoadDelay));
    }

    private void InitPuzzleProperties()
    {
        // Retrieves an item based on player's used previous item.
        _puzzleItem = ItemManager.Instance.GetPuzzle(_currentItem);

        // Retrieves time/highscore based on the current puzzle mode.
        _bestTime = SaveSystem.GetData<float>(Metrics.BestTimeNum(_difficultyIndex,
            _puzzleItem.Dimension.EvenDimension.Item3));

        previewImage.sprite = _puzzleItem.PreviewImage;
        previewImageZoomed.sprite = previewImage.sprite;
        puzzleNameT.text = $"{_puzzleItem.ImageName} {_puzzleItem.Dimension.EvenDimension.Item3}";

        puzzleDifficultyT.text = _difficulties[_difficultyIndex];
    }

    private void Update()
    {
        if (!_hasGameStarted || _hasGameEnded)
            return;

        _currentTime = Time.timeSinceLevelLoad;

        countdownT.text = Utility.TimeFormat(_currentTime);
    }

    /// <summary>
    /// Performs various actions depending on the game's current state.
    /// </summary>
    private void GameManager_OnCurrentState(GameState currentState)
    {
        switch (currentState)
        {
            case GameState.Playing:
                _hasGameStarted = true;
                _animator.SetTrigger(Metrics.UiStart);
                SoundManager.Instance.PlaySfx(startSfx);
                break;

            case GameState.GameOver:
                _hasGameEnded = true;
                StartCoroutine(GameoverDelay());
                CompareTime();
                DisplayGameoverTime();
                SoundManager.Instance.PlaySfx(gameoverSfx);
                break;

            case GameState.Exit:
                _hasGameEnded = true;
                break;
        }
    }

    /// <summary>
    /// A little delay(with visuals) before starting game.
    /// </summary>
    private IEnumerator InitLoadingBar(float time)
    {
        var end = Time.time + time;
        var changeRate = loadingBar.fillAmount / time;

        while (Time.time < end)
        {
            loadingBar.fillAmount -= changeRate * Time.deltaTime;

            yield return 0;
        }

        _loadingBarCanvas.enabled = false;
    }

    /// <summary>
    /// Sets a new highscore if their exists none or player attains one.
    /// Also, rewards player in return.
    /// </summary>
    /// <remarks>
    /// Highscore changes depending on player's difficulty mode, see also: <seealso cref="DifficultyToggle"/>
    /// </remarks>
    private void CompareTime()
    {
        if (_bestTime == 0 || _currentTime < _bestTime)
        {
            _animator.SetTrigger(Metrics.Highscore);

            // Saved as number.
            SaveSystem.SaveData(Metrics.BestTimeNum(_difficultyIndex,
                    _puzzleItem.Dimension.EvenDimension.Item3),
                _currentTime);

            // Saved as string converted time-format.
            SaveSystem.SaveData(Metrics.BestTimeStr(_difficultyIndex,
                    _puzzleItem.Dimension.EvenDimension.Item3),
                Utility.TimeFormat(_currentTime));

            RewardCash(_puzzleItem.Dimension.CashAmount);
            SetCommentText(_commentGenerator.GoodTexts);
        }
        else
        {
            // Probably earn a reward.
            RewardCash(Random.Range(0, 2));
            SetCommentText(_commentGenerator.FairTexts);
        }
    }

    /// <summary>
    /// Adds up and saves the rewarded cash on the player's wallet. 
    /// </summary>
    private void RewardCash(int amount = 1)
    {
        _savedCash += amount;

        SaveSystem.SaveData(Metrics.Cash, _savedCash);

        cashT.SetText("${0}", amount);
    }

    /// <summary>
    /// Displays the time player elapsed on the game-over UI.
    /// </summary>
    private void DisplayGameoverTime()
    {
        highscoreT.text = Utility.TimeFormat(_currentTime);
    }

    /// <summary>
    /// A little delay before displaying the game-over screen.
    /// </summary>
    private IEnumerator GameoverDelay()
    {
        yield return Utility.GetWaitForSeconds(_gameManager.EndDelay);

        _animator.SetTrigger(Metrics.UiStop);
    }

    /// <summary>
    /// Assigns a gameover win text, depending on highscore. 
    /// </summary>
    private void SetCommentText(IReadOnlyList<string> texts)
    {
        winT.text = texts[Random.Range(0, texts.Count)];
    }

    /// <summary>
    /// Zooms the preview Image when the player long-presses on it.
    /// </summary>
    public void ZoomPrevImage(bool state)
    {
        if (_hasGameStarted)
            _animator.SetBool(Metrics.PrevImageZoom, state);
    }

    /// <summary>
    /// See: <seealso cref="UIControllerMain"/>
    /// </summary>
    public void LoadMainMenu()
    {
        LoadManager.Instance.LoadSceneAsync(0);
    }

    // Stops listening to event when this object is destroyed.
    private void OnDestroy()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;
    }
}

/// <summary>
/// See: <see cref="UIControllerMain"/>
/// </summary>
internal partial class UIControllerGame
{
    #region Hidden UI Elements
    private bool _isGameoverOpen;
    private bool _isPrevZoomImgOpen;

    [Header("HIDDEN UIs REFs"), Space(10)]
    [SerializeField] private Canvas gameoverPanel;
    [SerializeField] private Canvas previewZoomImg;
    #endregion

    [ContextMenu(nameof(ShowPreviewZoomImage))]
    private void ShowPreviewZoomImage()
    {
        _isPrevZoomImgOpen = !_isPrevZoomImgOpen;
        previewZoomImg.enabled = _isPrevZoomImgOpen;
    }

    [ContextMenu(nameof(ShowGameoverMenu))]
    private void ShowGameoverMenu()
    {
        if (_isPrevZoomImgOpen)
            return;

        _isGameoverOpen = !_isGameoverOpen;
        gameoverPanel.enabled = _isGameoverOpen;
    }

    [ContextMenu(nameof(ResetPanelsToDefault))]
    private void ResetPanelsToDefault()
    {
        previewZoomImg.enabled = _isPrevZoomImgOpen = false;
        gameoverPanel.enabled = _isGameoverOpen = false;

#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }
}
