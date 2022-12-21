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

    private string _highscore;
    private bool _isUserPanelActive;
    private int _cashAmount;

    [Header("TEXTS")]
    [SerializeField] private TextMeshProUGUI highscoreT;
    [SerializeField] private TextMeshProUGUI modeT;
    [SerializeField] private TextMeshProUGUI[] cashT;


    /// <summary>
    /// Gets/updates player's wallet amount in case player purchases an item.
    /// </summary>
    public int TotalCash
    {
        get => _cashAmount;
        set
        {
            _cashAmount = value;
            SaveSystem.SaveData(Metrics.Cash, _cashAmount);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        _anim = GetComponent<Animator>();

        InitCash();
        InitMode();
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

    /// <summary>
    /// Retrieves and refreshes player's cash from wallet.
    /// </summary>
    private void InitCash()
    {
        _cashAmount = SaveSystem.GetData<int>(Metrics.Cash);
        SyncCash();
    }

    /// <summary>
    /// Pops-out shopping cart panel when player clicks on the shop-icon.
    /// This is assigned to shop-button and close-shop-button on the main-menu.
    /// </summary>
    public void DisplayShoppingCart(bool state)
    {
        _anim.SetBool(Metrics.SlideInShop, state);
    }

    /// <summary>
    /// Pops-out/off user panel when player clicks on the user-icon.
    /// This is assigned to user button on the main-menu.
    /// </summary>
    public void DisplayUserPanel()
    {
        _isUserPanelActive = !_isUserPanelActive;

        _anim.SetBool(Metrics.UserPanel, _isUserPanelActive);
    }

    /// <summary>
    /// Displays setting panel when player clicks on the setting-icon.
    /// This is assigned to setting-button and close-button on the main menu.
    /// </summary>
    /// <param name="state"></param>
    public void DisplaySettingsPanel(bool state)
    {
        _anim.SetBool(Metrics.SettingsPane, state);
    }

    /// <summary>
    /// Assigns player's saved highscore to highscore text.
    /// Assigns a default highscore if none.
    /// </summary>
    public void SyncHighscore(int difficulty = 0)
    {
        _highscore = SaveSystem.GetData<string>(Metrics.BestTimeStr(difficulty, modeT.text));

        if (string.IsNullOrEmpty(_highscore))
        {
            highscoreT.text = Metrics.DefaultHighscoreFormat;

            return;
        }

        highscoreT.text = _highscore;
    }

    /// <summary>
    /// Updates the cash amount text on the main/shop panel.
    /// </summary>
    public void SyncCash()
    {
        foreach (var t in cashT)
        {
            t.SetText("${0}", _cashAmount);
        }
    }

    /// <summary>
    /// Loads to the next scene asynchronously.
    /// </summary>
    public void LoadScene()
    {
        LoadManager.Instance.LoadSceneAsync(1);
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

    private bool _isShopOpen;
    private bool _isSettingOpen;
    private bool _isUserOpen;
    private bool _isInputFieldOpen;

    [Header("HIDDEN UIs REFs"), Space(10)]
    [SerializeField] private CanvasGroup settingPanel;
    [SerializeField] private Canvas shopPanel;
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

        _isShopOpen = !_isShopOpen;

        mainPanel.SetActive(!_isShopOpen);
        playB.SetActive(!_isShopOpen);
        shopPanel.enabled = _isShopOpen;
    }

    [ContextMenu(nameof(ShowSettingMenu))]
    private void ShowSettingMenu()
    {
        if (_isShopOpen || _isUserOpen || _isInputFieldOpen)
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
        if (_isSettingOpen || _isShopOpen || _isInputFieldOpen)
            return;

        _isUserOpen = !_isUserOpen;
        userPanel.enabled = _isUserOpen;
    }

    [ContextMenu(nameof(ShowInputFieldStartup))]
    private void ShowInputFieldStartup()
    {
        if (_isSettingOpen || _isUserOpen || _isShopOpen)
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
        shopPanel.enabled = _isShopOpen = false;
        userPanel.enabled = _isUserOpen = false;
        startupInputField.SetActive(_isInputFieldOpen = false);

        mainPanel.SetActive(true);
        playB.SetActive(true);

#if UNITY_EDITOR
        EditorUtility.SetDirty(gameObject);
#endif
    }
}