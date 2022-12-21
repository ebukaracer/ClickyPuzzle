using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scriptable Object/Container that stores the properties of items available.
/// </summary>
[CreateAssetMenu(fileName = "Puzzle_Name", menuName = "Puzzle_Item")]
internal class PuzzleItem : ScriptableObject
{
    private const int Size = 2;

    public IDimension Dimension { get; } = new FourByFour();

    [field: Tooltip("If this is your preferred first item, set this field to 0 and increment(by 1) for other items.")]
    [field: SerializeField] public int ItemID { get; private set; }

    [field: Tooltip("If this is your preferred first item, set this field to 0 and increment(by any amount) for other items.")]
    [field: SerializeField] public int ItemPrice { get; private set; }

    [field: SerializeField] public string ImageName { get; private set; }

    [Space(5), SerializeField] private SlicedSprite[] slicedSprites;
    [Space(5), SerializeField] private Sprite previewImage;


    private void OnValidate()
    {
        // Always resize and update the array to the constant size defined.
        if (slicedSprites.Length != Size)
        {
            Array.Resize(ref slicedSprites, Size);
        }
    }

    // 0->E, 1->H
    public List<Sprite> GetSlicedImages(int difficultyIndex)
    {
        return slicedSprites[difficultyIndex].sprites;
    }

    public SlicedSprite[] SlicedSprites
    {
        get => slicedSprites;
        set => slicedSprites = value;
    }

    public Sprite GetGetEmptySprite(int difficultyIndex)
    {
        return slicedSprites[difficultyIndex].emptySlot;
    }

    public Sprite PreviewImage
    {
        get => previewImage;
        set => previewImage = value;
    }
}