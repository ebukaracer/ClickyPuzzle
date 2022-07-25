using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class SlicedSprite
{
    public string Name;

    public List<Sprite> sprites;

    // The first index of the 'splittedImages' list.
    public Sprite emptySprite;
}

/// <summary>
/// Scriptable objects that hold the properties of elements to be used/purchased.
/// </summary>

[CreateAssetMenu(fileName = "Puzzle_Name", menuName = "Puzzle")]
class PuzzlePopulator : ScriptableObject
{
    // IndexID = 0 ; Default item which is purchased at default.

    // Setters
    [SerializeField]
    int itemID;

    [Space(5)]

    [SerializeField]
    int itemPrice;

    [Space(5)]

    [SerializeField]
    string imageName;

    [Space(5)]

    [SerializeField]
    SlicedSprite[] slicedSprites;

    [Space(5)]
    [SerializeField]
    Sprite previewImage;



    // Getters
    public List<Sprite> GetSplittedImage(int difficultyIndex)
    {
        return slicedSprites[difficultyIndex].sprites;
    }

    public SlicedSprite[] InitSliceSprites
    {
        get => slicedSprites;

        set => slicedSprites = value;
    }

    public Sprite GetGetEmptySprite(int difficultyIndex)
    {
        return slicedSprites[difficultyIndex].emptySprite;
    }


    public Sprite PreviewImage
    {
        get => previewImage;

        set => previewImage = value;
    }

    public string GetImageName => imageName;

    public int GetIndexID => itemID;

    public int GetItemPrice => itemPrice;
}
