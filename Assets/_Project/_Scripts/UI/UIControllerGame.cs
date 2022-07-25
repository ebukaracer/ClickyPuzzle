using Racer.LoadManager;
using Racer.SaveManager;
using Racer.SaveSystem;
using Racer.SoundManager;
using Racer.Utilities;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Initializes and saves various ui-elements(game) properties.
/// </summary>
class UIControllerGame : MonoBehaviour
{
    GameManager gameManager;

    float gameLoadDelay;

    float currentTime;

    float bestTime;

    int savedCash;

    int difficultyIndex;

    bool hasGameStarted;


    // This should correspond to the difficulties available in the game's main menu
    // Ordering and indexing matters
    readonly string[] difficulties = new string[] { "EASY", "PRO" };

    // Time format to display elapsed time
    TimeSpan timeSpan;


    [SerializeField]
    Animator uiAnimator;

    [Space(10), Header("Images")]

    [SerializeField]
    Image loadingBar;

    [SerializeField]
    Image previewImage;

    [SerializeField]
    Image previewImageZoomed;

    [Space(10)]

    // T => text
    [SerializeField, Header("TextMeshPro texts")]
    TextMeshProUGUI countdownT;

    [SerializeField]
    TextMeshProUGUI highscoreT;

    [SerializeField]
    TextMeshProUGUI cashT;

    [SerializeField]
    TextMeshProUGUI puzzle_NameT;

    [SerializeField]
    TextMeshProUGUI puzzle_DifficultyT;

    [Space(10)]

    [SerializeField]
    AudioClip startSfx, gameoverSfx;

    private void Start()
    {
        gameManager = GameManager.Instance;

        loadingBar.fillAmount = 1;


        // Delay before game starts
        // This is defined in the game-manager class
        gameLoadDelay = gameManager.GetStartDelay;

        // Listens for anytime the game's current state is changed
        GameManager.OnCurrentState += GameManager_OnCurrentState;


        // Retrieves an item from the scriptable object
        //var puzzlePopulator = ItemManager.Instance.GetPuzzle(saveManager.LoadInt("CurrentItemUsed"));
        var puzzlePopulator = ItemManager.Instance.GetPuzzle(SaveSystem.GetData<int>("CurrentItemUsed"));

        // Initializes some stuffs based the retrieved item
        // ...
        previewImage.sprite = puzzlePopulator.PreviewImage;

        previewImageZoomed.sprite = previewImage.sprite;

        puzzle_NameT.text = puzzlePopulator.GetImageName;
        // ...

        difficultyIndex = SaveSystem.GetData<int>("Difficulty");

        puzzle_DifficultyT.text = difficulties[difficultyIndex];

        //bestTime = saveManager.LoadFloat($"Time_{saveManager.LoadInt("Difficulty")}");
        bestTime = SaveSystem.GetData($"BestTime.Num_{difficultyIndex}", 0f);

        //savedCash = saveManager.LoadInt("Cash");
        savedCash = SaveSystem.GetData<int>("Cash");

        // Animates the loading bar via the game start delay
        if (gameLoadDelay != 0)
            StartCoroutine(InitLoadingBar(gameLoadDelay));
    }

    /// <summary>
    /// Performs various actions depending on the game's current state
    /// </summary>
    private void GameManager_OnCurrentState(GameStates currentState)
    {
        switch (currentState)
        {
            case GameStates.Playing:
                hasGameStarted = true;
                uiAnimator.SetTrigger(Animator.StringToHash("Start"));
                SoundManager.Instance.PlaySfx(startSfx);
                break;

            case GameStates.GameOver:
                StartCoroutine(GameoverDelay());
                CompareTime();
                DisplayGameoverTime();
                SoundManager.Instance.PlaySfx(gameoverSfx);
                break;
        }
    }


    /// <summary>
    /// Filled-Loading-bar that simulates the 'delay before game starts'
    /// </summary>
    /// <param name="time">time in (s)</param>
    IEnumerator InitLoadingBar(float time)
    {
        var end = Time.time + time;

        var changeRate = loadingBar.fillAmount / time;

        while (Time.time < end)
        {
            loadingBar.fillAmount -= changeRate * Time.deltaTime;

            yield return null;
        }

        loadingBar.GetComponentInParent<Canvas>().enabled = false;
    }

    /// <summary>
    /// Time player spends on a session
    /// </summary>
    void UpdateGamePlayTime()
    {
        timeSpan = TimeSpan.FromSeconds(currentTime);

        // timeSpan.ToString("m':'ss"));
        if (timeSpan.Hours < 60)
            countdownT.SetText("{0}m:{1}s", timeSpan.Minutes, timeSpan.Seconds);

        if (timeSpan.Hours >= 60)
            countdownT.SetText("{0}h:{1}m:{2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }

    /// <summary>
    /// Sets a new highscore if there exists none
    /// Overwrites already existing highscore if gotten a value less than it
    /// Highscore vary depending on player's difficulty mode
    /// </summary>
    void CompareTime()
    {
        //saveManager.Save($"Time_{difficultyIndex}", currentTime);

        if (bestTime == 0 || currentTime < bestTime)
        {
            uiAnimator.SetTrigger(Animator.StringToHash("Highscore"));

            // Saved as number
            SaveSystem.SaveData($"BestTime.Num_{difficultyIndex}", currentTime);

            // Saved as string
            SaveSystem.SaveData($"BestTime_{difficultyIndex}", timeSpan.ToString("m':'ss"));

            RewardCash(Random.Range(3, 5));
        }
        else
        {
            RewardCash();
        }
    }

    /// <summary>
    /// Rewards player anytime a new highscore is acquired
    /// Adds up and saves the rewarded cash on the player's wallet 
    /// </summary>
    void RewardCash(int amount = 1)
    {
        savedCash += amount;

        SaveSystem.SaveData("Cash", savedCash);

        cashT.SetText("${0}", amount);
    }

    /// <summary>
    /// Zooms the preview Image when the player long-presses on it
    /// </summary>
    /// <param name="state">true if player is long-pressing on the image otherwise false</param>
    public void ShowZoomedPrevImage(bool state)
    {
        if (hasGameStarted)
            uiAnimator.SetBool(Animator.StringToHash("PrevImage"), state);
    }

    private void Update()
    {
        if (!hasGameStarted)
            return;

        currentTime = Time.timeSinceLevelLoad;

        UpdateGamePlayTime();

        // For testing 
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
            gameManager.SetGameState(GameStates.GameOver);
#endif

    }

    /// <summary>
    /// Displays the time player elapsed on the game-over screen
    /// </summary>
    void DisplayGameoverTime()
    {
        highscoreT.SetText("{0}m:{1}s", timeSpan.Minutes, timeSpan.Seconds);
    }

    /// <summary>
    /// A little delay before displaying the game-over screen
    /// </summary>
    IEnumerator GameoverDelay()
    {
        yield return Utility.GetWaitForSeconds(2f);

        uiAnimator.SetTrigger(Animator.StringToHash("Stop"));
    }

    /// <summary>
    /// Loads the next scene
    /// </summary>
    public void LoadMainMenu()
    {
        LoadManager.Instance.LoadSceneAsync(0);
    }

    // Stops listening to event when this object is destroyed
    private void OnDestroy()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;
    }
}