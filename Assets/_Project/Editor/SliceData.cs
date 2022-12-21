#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// Stores custom data used for slicing sprite.
/// </summary>
/// <remarks>
/// There should be only one instance of this object.
/// This object must be created within the path specified in: <seealso cref="Path"/>,
/// and their filename must match too.
/// </remarks>
[CreateAssetMenu(fileName = "SliceData", menuName = "Slice_Data")]
internal class SliceData : ScriptableObject
{
    [field: Header("CORE")]
    [field: Tooltip("Individual grids/squares will be based on this width.")]
    [field: SerializeField] public int SliceWidth { get; private set; } = 200;

    [field: Tooltip("Individual grids/squares will be based on this height.")]
    [field: SerializeField] public int SliceHeight { get; private set; } = 200;

    [field: Header("OTHERS"), Space(5)]
    [field: Tooltip("Default value serves best result.")]
    [field: SerializeField] public float PixelPerUnit { get; private set; } = 200;

    [field: Tooltip("Default value serves best result.")]
    [field: SerializeField] public float Pivot { get; private set; } = .5f;

    [field: Tooltip("Default value serves best result.")]
    [field: SerializeField] public int Alignment { get; private set; } = 9;
}
#endif

