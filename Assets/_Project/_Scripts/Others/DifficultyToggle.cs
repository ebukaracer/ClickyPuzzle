using Racer.SaveSystem;
using Racer.Utilities;
using TMPro;
using UnityEngine;

/// <summary>
/// Toggles the game difficulty levels.
/// </summary>
/// <remarks>
/// This class only supports two levels: On/Off -> Easy/Pro.
/// See also: <seealso cref="SoundToggle"/>
/// </remarks>
internal class DifficultyToggle : ToggleProvider
{
    [Space(5), Header("TARGET GRAPHICS")]
    public TextMeshProUGUI parentText;
    public string[] difficultyTexts;


    private void Awake()
    {
        InitToggle();
    }

    protected override void InitToggle()
    {
        ToggleIndex = SaveSystem.GetData<int>(saveString);

        SyncToggle();
    }

    public override void Toggle()
    {
        base.Toggle();

        SaveSystem.SaveData(saveString, ToggleIndex);

        SyncToggle();
    }

    protected override void SyncToggle()
    {
        base.SyncToggle();

        // 0->E, 1 ->H
        UIControllerMain.Instance.SyncHighscore(ToggleIndex);

        parentText.text = $"{difficultyTexts[ToggleIndex]}";
    }
}
