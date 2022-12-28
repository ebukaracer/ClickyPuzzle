using UnityEngine;

[DefaultExecutionOrder(-11)]
internal class Positioner : MonoBehaviour
{
    private Puzzle _puzzle;
    private Transform _myTransform;


    private void Awake()
    {
        _myTransform = transform;

        _puzzle = GetComponentInParent<Puzzle>();

        SetPivotPositionAndScale();
    }

    /// <summary>
    /// Repositions and scales the puzzle elements transform,
    /// based on the current dimension.
    /// </summary>
    private void SetPivotPositionAndScale()
    {
        _myTransform.localPosition = _puzzle.PuzzleItem.Dimension.BoardPlacementVector.Item1;
        _myTransform.localScale = _puzzle.PuzzleItem.Dimension.BoardPlacementVector.Item2;
    }
}

