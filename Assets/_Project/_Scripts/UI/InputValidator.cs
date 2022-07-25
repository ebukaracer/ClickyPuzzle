using Racer.SaveManager;
using Racer.SaveSystem;
using Racer.Utilities;
using TMPro;
using UnityEngine;

/// <summary>
/// This handles player's username text input.
/// </summary>
class InputValidator : MonoBehaviour
{
    Animator anim;

    // Default text if no input was entered.
    readonly string defaultText = "USER_001";

    string inputText;

    //Input fields
    [SerializeField, Space(10)]
    TMP_InputField inputFieldStartup;

    [SerializeField]
    TMP_InputField inputFieldMain;

    [Space(10)]

    //User panel 
    [SerializeField]
    TextMeshProUGUI usernameT;


    private void Start()
    {
        // Startup-Input-field active-state is handled by animation.
        anim = GetComponentInParent<Animator>();

        ShowInputFieldStartup();

        inputFieldStartup.onEndEdit.AddListener(InitInputFieldStartup);

        inputFieldMain.onEndEdit.AddListener(InitInputFieldMain);

        inputFieldStartup.ActivateInputField();
    }

    /// <summary>
    /// Stores and saves player's inputted username in a temporary variable.
    /// Initialized the very first time game is loaded.
    /// </summary>
    /// <param name="t">input text(username)</param>
    void InitInputFieldStartup(string t)
    {
        inputText = string.IsNullOrEmpty(t) ? defaultText : t;

        SaveManager.SaveString("Username", inputText);
    }

    /// <summary>
    /// Stores and saves player's inputted username in a temporary variable.
    /// Enable players to change their username on the main-panel.
    /// </summary>
    /// <param name="t">input text(username)</param>
    void InitInputFieldMain(string t)
    {
        inputText = string.IsNullOrEmpty(t) ? inputText : t;

        SaveManager.SaveString("Username", inputText);

        InitInputText();
    }


    /// <summary>
    /// Exits the input field via an exit button.
    /// This is assigned to a exit-button on the input-field panel
    /// </summary>
    public void ExitInputFieldStartup()
    {
        InitInputText();

        anim.SetBool(Animator.StringToHash("SlideInInputField"), false);
    }

    /// <summary>
    /// Enables the input-field(startup) the very first time game loads.
    /// </summary>
    void ShowInputFieldStartup()
    {
        if (string.IsNullOrEmpty(SaveManager.GetString("Username")))
            anim.SetBool(Animator.StringToHash("SlideInInputField"), true);
    }


    /// <summary>
    /// Loads and Initializes the value entered in the two input-fields when game starts
    /// </summary>
    public void InitInputText()
    {
        string name = SaveManager.GetString("Username");

        inputText = name;

        usernameT.text = name;

        inputFieldMain.text = name;
    }
}