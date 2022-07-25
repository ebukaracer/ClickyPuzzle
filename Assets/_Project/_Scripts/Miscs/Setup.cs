using Racer.Utilities;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


public class Setup : MonoBehaviour
{
    [SerializeField, Tooltip("This should match the original sprite's name.")]
    string spriteName;

    [Space(10)]

    [SerializeField, Header("Scriptable Object")]
    PuzzlePopulator puzzle;

    readonly string slicedSpritePath = "Sprites/Grids(Sliced)/";

    readonly string slicePreviewPath = "Sprites/Previews(Unsliced)/";


    public void Init()
    {
        try
        {
            InitPuzzle();
        }
        catch (System.Exception ex)
        {
            Logging.Log(ex.Message);
        }
    }

    private void InitPuzzle()
    {
        var spritePreview = Resources.Load($"{slicePreviewPath}{spriteName}", typeof(Sprite)) as Sprite;

        var objsE = Resources.LoadAll($"{slicedSpritePath}{spriteName}(E)", typeof(Sprite));

        var objsH = Resources.LoadAll($"{slicedSpritePath}{spriteName}(H)", typeof(Sprite));

        SlicedSprite ssE = new SlicedSprite
        {
            sprites = new List<Sprite>(16)
        };

        SlicedSprite ssH = new SlicedSprite()
        {
            sprites = new List<Sprite>(16)
        };

        for (int i = 0; i < objsE.Length; i++)
        {
            var toSprite = objsE[i] as Sprite;

            ssE.sprites.Add(toSprite);
        }

        for (int i = 0; i < objsH.Length; i++)
        {
            var toSprite = objsH[i] as Sprite;

            ssH.sprites.Add(toSprite);
        }

        ssE.emptySprite = ssE.sprites[0];

        ssH.emptySprite = ssH.sprites[0];

        ssE.sprites[0] = null;

        ssH.sprites[0] = null;

        //..
        ssE.Name = "Easy";

        ssH.Name = "Hard";
        //..

        puzzle.PreviewImage = spritePreview;

        puzzle.InitSliceSprites = new SlicedSprite[] { ssE, ssH };

#if UNITY_EDITOR
        EditorUtility.SetDirty(puzzle);
#endif
    }
}

