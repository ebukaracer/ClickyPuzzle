using Racer.Utilities;
using UnityEngine;

/// <summary>
/// Caches constant/static value references across various scripts.
/// </summary>
internal class Metrics : MonoBehaviour
{
    // Save Strings:
    public static string Cash = "Cash";
    public static string Username = "Username";
    public static string CurrentItemInUse = "CurrentItemInUsed";
    public static string Item = "Item_";
    public static string Difficulty = "Difficulty";
    public static string Mode = "Mode";// either, 2X2, 3X3, 4X4...
    public static string BestTimeStr(int difficulty, string mode) => $"BestTime.{mode}.Str_{difficulty}";
    public static string BestTimeNum(int difficulty, string mode) => $"BestTime.{mode}.Num_{difficulty}";

    // Animator IDs:
    public static int SlideInShop = Utility.GetAnimId("SlideInShop");
    public static int UserPanel = Utility.GetAnimId("UserPanel");
    public static int SettingsPane = Utility.GetAnimId("SettingsPanel");
    public static int InputFieldIn = Utility.GetAnimId("SlideInInputField");
    public static int Highscore = Utility.GetAnimId("Highscore");
    public static int UiStart = Utility.GetAnimId("UIStart");
    public static int UiStop = Utility.GetAnimId("UIStop");
    public static int PrevImageZoom = Utility.GetAnimId("PrevImage");

    // Other Random Fields:
    public const string DefaultText = "USER_001"; // Default text if no input(username) was entered.
    public const string DefaultHighscoreFormat = "00:00"; // Format to display user's highscore.
    public const int MaxSeed = 100; // Rate to shuffle the grid/square elements, keep this number moderate for optimized speed.
    public const string PurchasedText = "PURCHASED";
}

