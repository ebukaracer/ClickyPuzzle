#if UNITY_EDITOR
using UnityEngine;

internal class MenuData : ScriptableObject
{
    // Custom data that will be saved:
    public Texture2D texture2D;
    public PuzzleItem puzzleItem;
    public SliceData sliceData;
}
#endif