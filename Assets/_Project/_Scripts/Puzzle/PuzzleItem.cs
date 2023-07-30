using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object/Container that stores the properties of items available.
/// </summary>
[CreateAssetMenu(fileName = "Puzzle_Name", menuName = "Puzzle_Item")]
internal class PuzzleItem : ScriptableObject
{
    public IDimension Dimension { get; } = new ThreeByThree();

    [field: Tooltip("If this is your preferred first item, set this field to 0 and increment(by 1) for other items.")]
    [field: SerializeField] public int ItemID { get; private set; }

    [field: SerializeField] public string ImageName { get; private set; }
    [field: SerializeField, TextArea] public string Description { get; private set; }

    [Space(10), SerializeField] private SlicedSprite slicedSprites;
    [Space(5), SerializeField] private Sprite previewImage;


    // 0->E, 1->H

    public List<Sprite> SlicedImages => slicedSprites.sprites;

    public SlicedSprite SlicedSprites
    {
        get => slicedSprites;
        set => slicedSprites = value;
    }

    public Sprite GetEmptySprite => slicedSprites.emptySlot;

    public Sprite PreviewImage
    {
        get => previewImage;
        set => previewImage = value;
    }
}