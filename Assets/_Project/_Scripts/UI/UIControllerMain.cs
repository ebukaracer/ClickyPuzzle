using System.Collections;
using Racer.LoadManager;
using Racer.SaveSystem;
using Racer.Utilities;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DefaultExecutionOrder(-12)]
internal partial class UIControllerMain : SingletonPattern.Singleton<UIControllerMain>
{
    private Animator _anim;

    private int _readCount;
    private bool _hasReadGuide;
    private string _highscore;
    private bool _isUserPanelActive;
    private int _booksReadCount;

    [Header("TEXTS")]
    [SerializeField] private TextMeshProUGUI highscoreT;
    [SerializeField] private TextMeshProUGUI modeT;
    [SerializeField] private TextMeshProUGUI booksReadT;


    public int TotalBooksRead
    {
        get => _booksReadCount;
        set
        {
            _booksReadCount = value;
            booksReadT.text = $"{_booksReadCount}/{ItemManager.Instance.ItemCount}";
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _anim = GetComponent<Animator>();

        _hasReadGuide = SaveSystem.GetData<int>(Metrics.ReadCount) >= 1;

        InitMode();
        UpdateHighscore();
    }

    /// <summary>
    /// Sets the mode-text on the UI to the current mode we're in.
    /// </summary>
    private void InitMode()
    {
        modeT.text = ItemManager.Instance.GetPuzzle
            (SaveSystem.GetData<int>
                (Metrics.CurrentItemInUse)).Dimension.EvenDimension.Item3;
    }


    public void DisplayLibrary(bool state)
    {
        CollapseUserPanel();

        DisplayGuide(false);

        ModifyReadCount();

        _anim.SetBool(Metrics.LibraryIn, state);
    }

    public void DisplayInfo(bool state)
    {
        CollapseUserPanel();

        ModifyReadCount();

        _anim.SetBool(Metrics.InfoIn, state);
    }

    private void ModifyReadCount()
    {
        if (_hasReadGuide) return;

        if (_readCount < 1)
        {
            _readCount++;

            if (_readCount == 1)
                SaveSystem.SaveData(Metrics.ReadCount, _readCount);
        }
        else _hasReadGuide = true;
    }

    /// <summary>
    /// Pops-out/off user panel when player clicks on the user-icon.
    /// This is assigned to user button on the main-menu.
    /// </summary>
    public void DisplayUserPanel()
    {
        _isUserPanelActive = !_isUserPanelActive;

        _anim.SetBool(Metrics.UserPaneIn, _isUserPanelActive);
    }

    private void CollapseUserPanel()
    {
        if (_isUserPanelActive)
            _anim.SetBool(Metrics.UserPaneIn, _isUserPanelActive = false);
    }

    /// <summary>
    /// Displays setting panel when player clicks on the setting-icon.
    /// This is assigned to setting-button and close-button on the main menu.
    /// </summary>
    /// <param name="state"></param>
    public void DisplaySettingsPanel(bool state)
    {
        _anim.SetBool(Metrics.SettingsPaneIn, state);

        CollapseUserPanel();
    }

    /// <summary>
    /// Assigns player's saved highscore to highscore text.
    /// Assigns a default highscore if none.
    /// </summary>
    private void UpdateHighscore()
    {
        _highscore = SaveSystem.GetData<string>(Metrics.BestTimeStr(modeT.text));

        highscoreT.text = string.IsNullOrEmpty(_highscore)
            ? Metrics.DefaultHighscoreFormat
            : _highscore;
    }

    /// <summary>
    /// Loads to the next scene asynchronously.
    /// </summary>
    public void LoadScene()
    {
        if (_hasReadGuide)
            LoadManager.Instance.LoadSceneAsync(1);
        else
            StartCoroutine(GuidTextRoutine());

        CollapseUserPanel();
    }

    private IEnumerator GuidTextRoutine()
    {
        DisplayGuide(true);
        yield return Utility.GetWaitForSeconds(2f);
        DisplayGuide(false);
    }

    private void DisplayGuide(bool value)
    {
        _anim.SetBool(Metrics.GuideIn, value);
    }

    /// <summary>
    /// Deletes all player's progress.
    /// Reloads scene to apply changes.
    /// </summary>
    public void ResetPlayerProgress()
    {
        SaveSystem.DeleteSaveFile();
        LoadManager.Instance.LoadSceneAsync(0);
    }

    public void ExitGame()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Application.Quit();
#else
        Logging.Log("Bye!");
#endif
    }
}
/// <summary>
/// This is a leftover class of <see cref="UIControllerMain"/>.
/// </summary>
/// <remarks>
/// This class basically provides editor methods that helps
/// to display hidden UI elements (while not on playmode)
/// that are controlled by animation while on playmode.
/// It does not contain any important logic and therefore can be removed.
/// </remarks>
internal partial class UIControllerMain
{
    #region Hidden UI Elements
    private int _index;

    private bool _isLibraryOpen;
    private bool _isSettingOpen;
    private bool _isUserOpen;
    private bool _isInputFieldOpen;

    [Header("HIDDEN UIs REFs"), Space(10)]
    [SerializeField] private CanvasGroup settingPanel;
    [SerializeField] private CanvasGroup libraryPanel;
    [SerializeField] private Canvas userPanel;

    [Space(5)]

    [SerializeField] private GameObject playB;
    [SerializeField] private GameObject startupInputField;
    [SerializeField] private GameObject mainPanel;
    #endregion

    [ContextMenu(nameof(ShowShopMenu))]
    private void ShowShopMenu()
    {
        if (_isSettingOpen || _isUserOpen || _isInputFieldOpen)
            return;

        _isLibraryOpen = !_isLibraryOpen;

        mainPanel.SetActive(!_isLibraryOpen);
        playB.SetActive(!_isLibraryOpen);
        libraryPanel.enabled = _isLibraryOpen;
    }

    [ContextMenu(nameof(ShowSettingMenu))]
    private void ShowSettingMenu()
    {
        if (_isLibraryOpen || _isUserOpen || _isInputFieldOpen)
            return;

        _index++;
        _index %= 2;

        _isSettingOpen = !_isSettingOpen;

        settingPanel.alpha = _index;
        mainPanel.SetActive(!_isSettingOpen);
        playB.SetActive(!_isSettingOpen);
    }

    [ContextMenu(nameof(ShowUserPane))]
    private void ShowUserPane()
    {
        if (_isSettingOpen || _isLibraryOpen || _isInputFieldOpen)
            return;

        _isUserOpen = !_isUserOpen;
        userPanel.enabled = _isUserOpen;
    }

    [ContextMenu(nameof(ShowInputFieldStartup))]
    private void ShowInputFieldStartup()
    {
        if (_isSettingOpen || _isUserOpen || _isLibraryOpen)
            return;

        _isInputFieldOpen = !_isInputFieldOpen;
        startupInputField.SetActive(_isInputFieldOpen);
    }

    [ContextMenu(nameof(ResetPanelsToDefault))]
    private void ResetPanelsToDefault()
    {
        // User defined-settings in the scene.
        settingPanel.alpha = _index = 0;
        _isSettingOpen = false;
        libraryPanel.enabled = _isLibraryOpen = false;
        userPanel.enabled = _isUserOpen = false;
        startupInputField.SetActive(_isInputFieldOpen = false);

        mainPanel.SetActive(true);
        playB.SetActive(true);

#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }
}