using Racer.LoadManager;
using Racer.SaveManager;
using Racer.SaveSystem;
using Racer.SoundManager;
using Racer.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


enum GameDifficulty
{
    Easy, pro
}


enum GameMusic
{
    Play, Stop
}

/// <summary>
/// Initializes and saves various ui-elements(main) properties.
/// </summary>
class UIControllerMain : SingletonPattern.Singleton<UIControllerMain>
{
    InputValidator inputValidator;

    Animator anim;

    readonly string defaultHighscore = "00:00";


    [SerializeField]
    GameDifficulty gameDifficulty;

    [SerializeField]
    GameMusic gameMusic;

    [Space(15)]

    //Music
    [SerializeField]
    Image musicOnIcon;

    [SerializeField]
    Image musicOffIcon;

    int soundIndex = 1;


    //User Panel
    bool isUserPanelActive;


    //Difficulty button
    readonly string[] difficulties = new string[] { "EASY", "PRO" };

    int difficultyIndex;

    [SerializeField, Space(15)]
    TextMeshProUGUI difficultyT;


    //Highscore
    string highscore;

    [SerializeField, Space(15)]
    TextMeshProUGUI highscoreT;


    //Cash
    int cashAmount;

    [SerializeField, Space(15)]
    TextMeshProUGUI[] cashT;


    /// <summary>
    /// Gets/updates player's wallet amount in case player purchases an item.
    /// </summary>
    public int TotalCash
    {
        get => cashAmount;

        set
        {
            cashAmount = value;

            //SaveManager.Save("Cash", cashAmount);
            SaveSystem.SaveData("Cash", cashAmount);
        }
    }


    private void Start()
    {
        inputValidator = GetComponent<InputValidator>();

        anim = GetComponentInParent<Animator>();

        //Retrievals
        InitDifficulty();
        InitMusicState();
        Highscore();
        InitCash();
        inputValidator.InitInputText();

    }

    /// <summary>
    /// Enables/disables music state; on/off,
    /// Saves the current state.
    /// This is assigned to the music button in the main-menu.
    /// </summary>
    public void MusicState()
    {
        soundIndex = soundIndex == 1 ? 0 : soundIndex = 1;

        gameMusic = soundIndex == 1 ? GameMusic.Play : GameMusic.Stop;

        if (soundIndex == 1)
        {
            SaveManager.SaveInt("SoundState", 0);

            musicOnIcon.enabled = true;

            musicOffIcon.enabled = false;
        }
        if (soundIndex == 0)
        {
            SaveManager.SaveInt("SoundState", 1);

            musicOffIcon.enabled = true;

            musicOnIcon.enabled = false;
        }

        AudioMixerState();
    }

    /// <summary>
    /// Initializes current music state when game starts,
    /// Retrieves the saved value.
    /// </summary>
    void InitMusicState()
    {
        soundIndex = SaveManager.GetInt("SoundState");

        gameMusic = soundIndex == 1 ? GameMusic.Play : GameMusic.Stop;

        MusicState();
    }

    void AudioMixerState()
    {
        switch (gameMusic)
        {
            case GameMusic.Play:
                SoundManager.Instance.GetSnapShot(0).TransitionTo(0);
                break;
            case GameMusic.Stop:
                SoundManager.Instance.GetSnapShot(1).TransitionTo(0f);
                break;
        }
    }
    /// <summary>
    /// Pops-out shopping cart panel when player clicks on the shop-icon.
    /// This is assigned to shop-button and close-shop-button on the main-menu.
    /// </summary>
    public void DisplayShoppingCart(bool state)
    {
        anim.SetBool(Animator.StringToHash("SlideInShop"), state);
    }

    /// <summary>
    /// Pops-out/off user panel when player clicks on the user-icon.
    /// This is assigned to user button on the main-menu.
    /// </summary>
    public void DisplayUserPanel()
    {
        isUserPanelActive = !isUserPanelActive;

        anim.SetBool(Animator.StringToHash("UserPanel"), isUserPanelActive);
    }

    /// <summary>
    /// Displays setting panel when player clicks on the setting-icon.
    /// This is assigned to setting-button and close-button on the main menu.
    /// </summary>
    /// <param name="state"></param>
    public void DisplaySettingsPanel(bool state)
    {
        anim.SetBool(Animator.StringToHash("SettingsPanel"), state);
    }


    /// <summary>
    /// Iterates and assigns a difficulty over a list of difficulties,
    /// Saves the current difficulty selected.
    /// The current difficulty has it's own highscore.
    /// This is assigned to the difficulty button on the main-menu.
    /// </summary>
    public void SwitchDifficulty()
    {
        difficultyIndex++;

        if (difficultyIndex >= difficulties.Length)
            difficultyIndex = 0;

        difficultyT.text = difficulties[difficultyIndex];

        SaveSystem.SaveData("Difficulty", difficultyIndex);

        gameDifficulty = difficultyIndex == 0 ? GameDifficulty.Easy : GameDifficulty.pro;

        Highscore();
    }


    /// <summary>
    /// Initializes saved difficulty when game starts
    /// </summary>
    void InitDifficulty()
    {
        difficultyIndex = SaveSystem.GetData("Difficulty", 0);

        gameDifficulty = difficultyIndex == 0 ? GameDifficulty.Easy : GameDifficulty.pro;

        difficultyT.text = difficulties[difficultyIndex];
    }


    /// <summary>
    /// Assigns player's saved highscore to highscore text.
    /// Assigns a default highscore if none.
    /// </summary>
    void Highscore()
    {
        //highscore = SaveManager.LoadString($"BestTime_{(int)gameDifficulty}");
        highscore = SaveSystem.GetData<string>($"BestTime_{(int)gameDifficulty}");


        if (string.IsNullOrEmpty(highscore))
        {
            highscoreT.text = defaultHighscore;

            return;
        }

        highscoreT.text = highscore;
    }

    /// <summary>
    /// Retrieves player's saved wallet amount.
    /// </summary>
    void InitCash()
    {
        //cashAmount = SaveManager.LoadInt("Cash");
        cashAmount = SaveSystem.GetData<int>("Cash");

        SyncCash();
    }

    /// <summary>
    /// Updates the cash amount text on the main/shop panel
    /// </summary>
    public void SyncCash()
    {
        foreach (var t in cashT)
        {
            t.SetText("${0}", cashAmount);
        }
    }

    /// <summary>
    /// Loads the next scene asynchronously
    /// </summary>
    public void LoadScene()
    {
        LoadManager.Instance.LoadSceneAsync(1);
    }

    /// <summary>
    /// Deletes all player's progress,
    /// Restarts game to indicate changes.
    /// </summary>
    public void ResetPlayerProgress()
    {
        SaveManager.ClearAllPrefs();

        SaveSystem.DeleteSaveFile();
    }

    /// <summary>
    /// Quits game.
    /// </summary>
    public void ExitGame()
    {
        Logging.Log("Bye!");

        Application.Quit();
    }
}