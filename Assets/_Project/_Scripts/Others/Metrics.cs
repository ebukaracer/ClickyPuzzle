using Racer.Utilities;
using UnityEngine;

/// <summary>
/// Caches constant/static value references across various scripts.
/// </summary>
internal class Metrics : MonoBehaviour
{
    // Save Strings:
    public static string BooksReadCount = nameof(BooksReadCount);
    public static string Username = nameof(Username);
    public static string CurrentItemInUse = nameof(CurrentItemInUse);
    public static string Mode = nameof(Mode);// either, 2X2, 3X3, 4X4...
    public static string Item = "Item_";

    public static string IsMatched(int id) => $"IsMatched.{id}";
    public static string BestTimeStr(string mode) => $"BestTime.{mode}.Str";
    public static string BestTimeNum(string mode) => $"BestTime.{mode}.Num";

    // Animator IDs:
    public static int LibraryIn = Utility.GetAnimId(nameof(LibraryIn));
    public static int InfoIn = Utility.GetAnimId(nameof(InfoIn));
    public static int UserPanel = Utility.GetAnimId("UserPanel");
    public static int SettingsPane = Utility.GetAnimId("SettingsPanel");
    public static int InputFieldIn = Utility.GetAnimId("SlideInInputField");
    public static int Highscore = Utility.GetAnimId("Highscore");
    public static int UiStart = Utility.GetAnimId("UIStart");
    public static int UiStop = Utility.GetAnimId("UIStop");
    public static int RewardIn = Utility.GetAnimId(nameof(RewardIn));
    public static int PrevImageZoom = Utility.GetAnimId("PrevImage");

    // Other Random Fields:
    public const string DefaultText = "USER_001"; // Default text if no input(username) was entered.
    public const string DefaultHighscoreFormat = "00:00"; // Format to display user's highscore.
    public const int MaxSeed = 100; // Rate to shuffle the grid/square elements, keep this number moderate for optimized speed.
}

