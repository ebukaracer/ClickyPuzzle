#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using Racer.Utilities;
using UnityEditor;
using UnityEngine;

internal class MenuAction
{
    private const string Extension = ".png";

    private static string _textureName;
    private static string _path;

    private static TextureImporter _textureImporter;
    private static SliceData _sliceData;


    #region Texture Actions
    public static void CopyToPath(Texture2D texture, string path)
    {
        GetProperties(out _path, out _textureName, texture);

        if (string.IsNullOrEmpty(_path))
            return;

        var newPath = Path.RootPath + Path.SlicedPath + path + Extension;

        if (File.Exists(newPath))
        {
            Logging.LogError($"Failed! <b>{System.IO.Path.GetFileName(path)}</b> already exists at specified path.");
            return;
        }

        if (!AssetDatabase.CopyAsset(_path, newPath))
        {
            Logging.LogError($"Failed! Couldn't copy <b>{System.IO.Path.GetFileName(path)}</b> to specified path: <i>{newPath}</i>");
            return;
        }

        Logging.Log("Success!");
    }

    public static void SliceTexture(Texture2D texture, SliceData sliceData)
    {
        GetProperties(out _path, out _textureName, texture);

        if (string.IsNullOrEmpty(_path))
            return;

        _sliceData = sliceData;
        Slice(texture, _path);
    }

    public static void ResetTexture(Texture2D texture)
    {
        GetProperties(out _path, out _textureName, texture);

        if (string.IsNullOrEmpty(_path))
            return;

        AssignSettings(_path);

        Logging.Log("Success!");

        // Re-import to apply new changes.
        AssetDatabase.ImportAsset(_path, ImportAssetOptions.ForceUpdate);
    }

    public static void DeleteTexture(Texture2D texture)
    {
        GetProperties(out _path, out _textureName, texture);

        if (string.IsNullOrEmpty(_path))
            return;

        if (AssetDatabase.DeleteAsset(_path))
            Logging.Log("Success!");
        else
            Logging.LogError($"Failed! Couldn't delete <b>{_textureName}");
    }
    #endregion

    #region Utility Methods
    private static void Slice(Texture myTexture, string path)
    {
        if (!GetSliceDataRef())
            return;

        var index = 0;
        var sliceWidth = _sliceData.SliceWidth;
        var sliceHeight = _sliceData.SliceHeight;
        var pivot = _sliceData.Pivot;
        var alignment = _sliceData.Alignment;

        var textureImporter = AssignSettings(path, pixelPerUnit: _sliceData.PixelPerUnit, isReadable: true, importMode: SpriteImportMode.Multiple);

        // list to store sliced-sprites.
        var metaData = new List<SpriteMetaData>();

        try
        {
            for (int y = myTexture.height; y > 0; y -= sliceHeight)
            {
                for (int x = 0; x < myTexture.width; x += sliceWidth)
                {
                    var tempMetaData = new SpriteMetaData
                    {
                        pivot = new Vector2(pivot, pivot),
                        alignment = alignment,
                        name = myTexture.name + "_" + index,
                        rect = new Rect(x, y - sliceHeight, sliceWidth, sliceHeight)
                    };

                    index++;
                    metaData.Add(tempMetaData);
                }
            }
        }
        catch (System.Exception ex)
        {
            Logging.LogError(ex.Message);

            return;
        }

        textureImporter.spritesheet = metaData.ToArray();

        Logging.Log("Success!");

        // Re-import to apply new changes.
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    private static void GetProperties(out string path, out string nameWithExt, Texture2D texture = null)
    {
        path = AssetDatabase.GetAssetPath(texture);

        if (texture)
            nameWithExt = texture.name + Extension;
        else
            nameWithExt = null;

        if (!string.IsNullOrEmpty(path)) return;

        path = null;
        Logging.LogError($"Failed! Invalid path: <i>{_path}</i>");
    }
    #endregion

    #region Texture Settings
    private static TextureImporter AssignSettings(string path,
        float pixelPerUnit = 100,
        int maxSize = 1024,
        bool isReadable = false,
        SpriteImportMode importMode = SpriteImportMode.Single)
    {
        _textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (_textureImporter == null) return null;

        _textureImporter.isReadable = isReadable;
        _textureImporter.spriteImportMode = importMode;
        _textureImporter.spritePixelsPerUnit = pixelPerUnit;
        _textureImporter.maxTextureSize = maxSize;

        return _textureImporter;
    }

    private static bool GetSliceDataRef()
    {
        // if no data exists yet, create and reference a new instance.
        if (_sliceData) return true;

        // if a previous data exists and is loaded successfully, we use it.
        if (_sliceData) return true;

        Logging.LogError(
            $"No <i>SliceData</i> Scriptable object assigned! Create/Assign a <b>{nameof(SliceData)}</b> then try again.");

        return false;
    }

    #endregion
}
#endif