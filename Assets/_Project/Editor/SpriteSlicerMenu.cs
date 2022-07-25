#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Racer.Utilities;

public class SpriteSlicerMenu : MonoBehaviour
{
    private const string UnSlicedPath = "Sprites/Previews(Unsliced)/";

    private const string SlicedPath = "Sprites/Grids(Sliced)/";

    private const string ImportPathEasy = "Sprites/_Imports/Easy/";

    private const string ImportPathHard = "Sprites/_Imports/Hard/";


    [MenuItem("Sprites/Initialize Sprites")]
    public static void Initialize()
    {
        LoadEasySprite();

        LoadHardSprite();
    }

    static void LoadEasySprite()
    {
        var easyObjs = Resources.LoadAll(ImportPathEasy, typeof(Texture2D));

        foreach (var easyObj in easyObjs)
        {
            var toSprite = easyObj as Texture2D;

            var name = toSprite.name;

            var myOriginalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Puzzle/Resources/{ImportPathEasy}{name}.png");

            if (myOriginalTexture == null)
            {
                Logging.LogWarning("File does not exist");

                return;
            }

            string path = AssetDatabase.GetAssetPath(myOriginalTexture);


            var hasCopyToSlice = AssetDatabase.CopyAsset(path, $"Assets/Puzzle/Resources/{SlicedPath}{myOriginalTexture.name}(E).png");

            if (!hasCopyToSlice)
            {
                Logging.LogWarning("Unsuccessful");

                return;
            }
        }

    }

    static void LoadHardSprite()
    {
        var easyObjs = Resources.LoadAll(ImportPathHard, typeof(Texture2D));

        foreach (var easyObj in easyObjs)
        {
            var toSprite = easyObj as Texture2D;

            var name = toSprite.name;

            var myOriginalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Puzzle/Resources/{ImportPathHard}{name}.png");


            string path = AssetDatabase.GetAssetPath(myOriginalTexture);

            var hasCopyToUnSlice = AssetDatabase.CopyAsset(path, $"Assets/Puzzle/Resources/{UnSlicedPath}{myOriginalTexture.name}.png");

            var hasCopyToSlice = AssetDatabase.CopyAsset(path, $"Assets/Puzzle/Resources/{SlicedPath}{myOriginalTexture.name}(H).png");

            if (!hasCopyToSlice || !hasCopyToUnSlice)
            {
                Logging.LogWarning("Unsuccessful");

                return;
            }
        }
    }

    [MenuItem("Sprites/Slice Sprites")]
    static void SliceSprite()
    {
        var newObjs = Resources.LoadAll(SlicedPath, typeof(Sprite));

        foreach (var newObj in newObjs)
        {
            var toNewSprite = newObj as Sprite;

            var newSprite = toNewSprite.name;

            var myTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Puzzle/Resources/{SlicedPath}{newSprite}.png");

            // Logging.Log(myTexture.name);

            if (myTexture == null)
            {
                Logging.LogWarning($"[{myTexture.name}] File does not exist");

                return;
            }

            string pathNew = AssetDatabase.GetAssetPath(myTexture);

            SliceTexture(myTexture, pathNew);
        }
    }



    [MenuItem("Sprites/Delete UnSliced Sprites")]
    static void DeleteUnSlicedSprites()
    {
        var objs = Resources.LoadAll(UnSlicedPath, typeof(Sprite));


        foreach (var obj in objs)
        {
            var toSprite = obj as Sprite;

            var name = toSprite.name;

            var myTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Puzzle/Resources/{UnSlicedPath}{name}.png");

            if (myTexture == null)
            {
                Logging.LogWarning($"[{myTexture.name}] File does not exist");

                return;
            }

            string path = AssetDatabase.GetAssetPath(myTexture);

            bool hasDeleted = AssetDatabase.DeleteAsset(path);

            if (!hasDeleted)
            {
                Logging.LogWarning("Unsuccessful");

                return;
            }
        }
    }

    [MenuItem("Sprites/Delete Sliced Sprites")]
    static void DeleteSlicedSprites()
    {
        ResetSpritesToDefault();

        var objs = Resources.LoadAll(SlicedPath, typeof(Sprite));


        foreach (var obj in objs)
        {
            var toSprite = obj as Sprite;

            var name = toSprite.name;

            var myTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Puzzle/Resources/{SlicedPath}{name}.png");

            if (myTexture == null)
            {
                Logging.LogWarning($"[{myTexture.name}] File does not exist");

                return;
            }

            string path = AssetDatabase.GetAssetPath(myTexture);

            bool hasDeleted = AssetDatabase.DeleteAsset(path);

            if (!hasDeleted)
            {
                Logging.LogWarning("Unsuccessful");

                return;
            }
        }
    }

    [MenuItem("Sprites/Reset Sliced Sprites To Default Settings")]
    static void ResetSpritesToDefault()
    {
        var objs = Resources.LoadAll(SlicedPath, typeof(Texture2D));


        foreach (var obj in objs)
        {
            var toSprite = obj as Texture2D;

            var name = toSprite.name;

            var myTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"Assets/Puzzle/Resources/{SlicedPath}{name}.png");

            if (myTexture == null)
            {
                Logging.LogWarning($"[{myTexture.name}] File does not exist");

                return;
            }

            string path = AssetDatabase.GetAssetPath(myTexture);


            GetTextureDefaultProperties(path);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);


        }
    }

    private static void SliceTexture(Texture2D myTexture, string path)
    {
        int SliceWidth = 200;

        int SliceHeight = 200;

        int index = 0;

        TextureImporter ti = GetTextureProperties(path);

        // Loads sliced sprites to this list
        List<SpriteMetaData> newData = new List<SpriteMetaData>();

        // Slice Sprite
        try
        {

            for (int y = myTexture.height; y > 0; y -= SliceHeight)
            {
                for (int x = 0; x < myTexture.width; x += SliceWidth)
                {

                    SpriteMetaData smd = new SpriteMetaData
                    {

                        pivot = new Vector2(0.5f, 0.5f),

                        alignment = 9,

                        //name = myTexture.name + (myTexture.height - j) / SliceHeight + ", " + i / SliceWidth,

                        name = myTexture.name + "_" + index,

                        //rect = new Rect(x, y - SliceHeight, SliceWidth, SliceHeight)

                        rect = new Rect(x, y - SliceHeight, SliceWidth, SliceHeight)
                    };

                    index++;

                    newData.Add(smd);
                }
            }

        }
        catch (System.Exception ex)
        {
            Logging.LogWarning(ex.Message);

            return;
        }


        ti.spritesheet = newData.ToArray();

        //Refresh
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    private static TextureImporter GetTextureProperties(string path)
    {
        var ti = AssetImporter.GetAtPath(path) as TextureImporter;

        ti.isReadable = true;

        ti.maxTextureSize = 1024;

        ti.spriteImportMode = SpriteImportMode.Multiple;

        ti.spritePixelsPerUnit = 200;

        return ti;
    }

    private static TextureImporter GetTextureDefaultProperties(string path)
    {
        var ti = AssetImporter.GetAtPath(path) as TextureImporter;

        ti.isReadable = true;

        ti.maxTextureSize = 2048;

        ti.spriteImportMode = SpriteImportMode.Single;

        ti.spritePixelsPerUnit = 100;

        return ti;
    }
}

#endif