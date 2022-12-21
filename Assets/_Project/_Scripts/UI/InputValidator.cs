using Racer.SaveSystem;
using TMPro;
using UnityEngine;

/// <summary>
/// This handles player's username text inputs.
/// </summary>
internal class InputValidator : MonoBehaviour
{
    private Animator _anim;

    private string _currentInput;
    private string _savedInput;

    [Header("INPUT FIELDS")]
    [SerializeField] private TMP_InputField inputFieldStartup;
    [SerializeField] private TMP_InputField inputFieldMain;

    [Space(5), Header("MISC")]
    [SerializeField] private TextMeshProUGUI usernameT;


    private void Awake()
    {
        _anim = GetComponentInParent<Animator>();

        _savedInput = SaveSystem.GetData<string>(Metrics.Username);
    }

    private void Start()
    {
        DisplayInputFieldStartup();

        inputFieldStartup.onEndEdit.AddListener(EndEditInputField);
        inputFieldMain.onEndEdit.AddListener(EndEditInputField);

        inputFieldStartup.ActivateInputField();
    }

    private void DisplayInputFieldStartup()
    {
        if (string.IsNullOrWhiteSpace(_savedInput))
            _anim.SetBool(Metrics.InputFieldIn, true);
        else
            AssignText(_savedInput);
    }

    private void EndEditInputField(string text)
    {
        _currentInput = string.IsNullOrWhiteSpace(text) ? Metrics.DefaultText : text;

        SaveSystem.SaveData(Metrics.Username, _currentInput);

        AssignText(_currentInput);
    }

    public void ExitInputFieldStartup()
    {
        _anim.SetBool(Metrics.InputFieldIn, false);
    }

    /// <summary>
    /// Loads and Initializes the value entered in both input-fields at startup.
    /// </summary>
    public void AssignText(string text)
    {
        usernameT.text = text;
        inputFieldMain.text = text;
    }
}