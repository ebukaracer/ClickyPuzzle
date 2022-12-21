#if UNITY_EDITOR
using Racer.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

internal class Initializer
{
    private static int _gridCount;

    private static Sprite _previewImg;
    private static SlicedSprite _slicedSprite;

    public static void Init(PuzzleItem puzzleItem, string path, bool isThis)
    {
        try
        {
            _gridCount = puzzleItem.Dimension.EvenDimension.Item2;

            if (isThis)
                InitThis(puzzleItem, path);
            else
                InitThat(puzzleItem, path);

        }
        catch (System.Exception ex)
        {
            Logging.LogError($"Failed! {ex.Message}");
        }
    }

    public static void AssignPreviewImg(PuzzleItem puzzleItem, string path)
    {
        _previewImg = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        puzzleItem.PreviewImage = _previewImg;

        Logging.Log("Success!");
    }

    private static void InitThis(PuzzleItem puzzleItem, string path)
    {
        if (!FillSlicedSprite(path))
            return;

        _slicedSprite.emptySlot = _slicedSprite.sprites[0];
        _slicedSprite.sprites[0] = null;
        _slicedSprite.name = "Easy";

        puzzleItem.SlicedSprites[0] = _slicedSprite;

        Logging.Log("Success!");
        EditorUtility.SetDirty(puzzleItem);
    }

    private static void InitThat(PuzzleItem puzzleItem, string path)
    {
        if (!FillSlicedSprite(path))
            return;

        _slicedSprite.emptySlot = _slicedSprite.sprites[0];
        _slicedSprite.sprites[0] = null;
        _slicedSprite.name = "Hard";

        puzzleItem.SlicedSprites[1] = _slicedSprite;

        Logging.Log("Success!");
        EditorUtility.SetDirty(puzzleItem);
    }

    private static bool FillSlicedSprite(string path)
    {
        var textures = AssetDatabase.LoadAllAssetsAtPath(path).ToList();

        if (textures.Count != _gridCount + 1)
        {
            Logging.LogError($"Failed! <b>{textures[0].name}</b> was <i>{textures.Count}</i>, but the Dimension set is <i>{_gridCount}</i>");
            return false;
        }

        textures = textures.OrderBy(o => o.name.IndexOf('_') + 1).ToList();

        _slicedSprite = new SlicedSprite
        {
            sprites = new List<Sprite>(_gridCount)
        };

        for (int i = 1; i < _gridCount + 1; i++)
        {
            var sprite = textures[i] as Sprite;

            _slicedSprite.sprites.Add(sprite);
        }
        return true;
    }
}
#endif