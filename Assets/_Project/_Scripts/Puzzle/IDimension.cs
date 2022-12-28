using UnityEngine;

internal interface IDimension
{
    // (2, 4, 2X2) -> (-1.9, .) and (3.8, .)
    // (3, 9, 3X3) -> (-2.5, .) and (2.5, .)
    // (4, 16, 4X4) -> (-2.85, .) and (1.9, .)

    public int CashAmount { get; }

    public (int, int, string) EvenDimension { get; }

    // Position and Scale(Hardcoded).
    public (Vector2, Vector2) BoardPlacementVector { get; }
}

internal class ThreeByThree : IDimension
{
    // Core:
    public (int, int, string) EvenDimension => (3, 9, "3X3");

    // Others:
    public int CashAmount => Random.Range(2, 5);

    public (Vector2, Vector2) BoardPlacementVector =>
        (new Vector2(-2.5f, -2.5f), new Vector2(2.5f, 2.5f));
}