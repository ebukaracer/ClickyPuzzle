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

internal class FourByFour : IDimension
{
    // Core:
    public (int, int, string) EvenDimension => (4, 16, "4X4");

    // Others:
    public int CashAmount => Random.Range(2, 5);

    public (Vector2, Vector2) BoardPlacementVector =>
        (new Vector2(-2.85f, -2.85f), new Vector2(1.9f, 1.9f));
}