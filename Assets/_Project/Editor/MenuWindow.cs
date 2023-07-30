#if UNITY_EDITOR
using Racer.Utilities;
using UnityEditor;
using UnityEngine;

internal class MenuWindow : EditorWindow
{
    private const int SizeX = 500;
    private const int SizeY = 600;

    private static string _texturePath;
    private static string _previewPath;

    private const string Nil = "nil";

    private static MenuData _menuData;
    private SerializedProperty _texture2D;
    private SerializedProperty _puzzleItem;
    private SerializedProperty _sliceData;

    // This method will be called on load or recompile.
    [InitializeOnLoadMethod]
    private static void OnLoad()
    {
        // if no data exists yet, create and reference a new instance.
        if (_menuData) return;

        _menuData = AssetDatabase.LoadAssetAtPath<MenuData>(Path.MenuDataPath);

        // if a previous data exists and is loaded successfully, we use it.
        if (_menuData) return;

        // otherwise create and reference a new instance.
        _menuData = CreateInstance<MenuData>();

        AssetDatabase.CreateAsset(_menuData, Path.MenuDataPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("Puzzle/Menu Window")]
    private static void Init()
    {
        // Get existing open window or if none, make a new one:
        var window = GetWindow(typeof(MenuWindow)) as MenuWindow;

        if (window == null) return;

        window.titleContent.text = "Puzzle Menu";

        window.maxSize = new Vector2(SizeX, SizeY);
        window.minSize = new Vector2(SizeX, SizeY);

        window.Show();
    }

    private void OnGUI()
    {
        // Not only will Unity automatically handle the set dirty and saving,
        // but will also add Undo/Redo functionality.
        var serializedObject = new SerializedObject(_menuData);

        // Syncs the values of the real instance into the serialized one.
        serializedObject.Update();

        _texture2D = serializedObject.FindProperty("texture2D");
        _puzzleItem = serializedObject.FindProperty("puzzleItem");
        _sliceData = serializedObject.FindProperty("sliceData");

        #region BASE SETTING
        EditorGUILayout.Space(5);
        GUILayout.Label("BASE SETTING", EditorStyles.largeLabel);

        if (IsValidTexture())
            DrawHelpBox("Properties:",
                Texture2dObject.name,
                AssetDatabase.GetAssetPath(Texture2dObject),
                Path.RootPath + Path.SlicedPath);
        else
            DrawHelpBox(optionalText: "Assign to [Source Texture] to continue operation.", messageType: MessageType.Warning);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Source Texture", EditorStyles.boldLabel, GUILayout.Width(150));
        Texture2dObject = EditorGUILayout.ObjectField(Texture2dObject, typeof(Texture2D), false) as Texture2D;

        GUILayout.Box("Preview", GUILayout.MaxWidth(80), GUILayout.MaxHeight(80));
        var rect = GUILayoutUtility.GetLastRect();

        if (IsValidTexture())
            EditorGUI.DrawPreviewTexture(rect, Texture2dObject);
        EditorGUILayout.EndHorizontal();
       
       
        if (IsValidTexture())
            RenderPreviewLabel();
        /*else
        {
            RenderPreviewLabel();
        }*/

        EditorGUILayout.Space(10);
        #endregion

        #region ACTION
        GUILayout.Label("ACTION", EditorStyles.largeLabel);
        EditorGUILayout.HelpBox("Drag in the original/imported sprite and hit [Setup] to the [Sliced Path] directory ready to be sliced.\n" +
                                "Drag in a [SliceData] scriptable object which contains properties that would be assigned to a sliced-sprite, before slicing. \n" +
                                "Drag in the setup-ed sprite and hit [Slice] to automatically slice it into grids ready to be initialized to the scriptable object.\n" +
                                "Drag in the sliced-sprite and hit [Reset] to revert it back to it's default settings.\n" +
                                "Drag in any sprite and hit [Delete] to delete it from project.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Slice Data", EditorStyles.boldLabel, GUILayout.Width(150));
        SliceDataObject = EditorGUILayout.ObjectField(SliceDataObject, typeof(SliceData), false) as SliceData;

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Setup"))
        {
            if (IsValidTexture() && _previewPath != Nil)
                MenuAction.CopyToPath(Texture2dObject, _previewPath);
            else
                Logging.LogError($"Failed! <b>{nameof(_texture2D)}</b> must not be empty.");
        }

        if (GUILayout.Button("Slice"))
        {
            if (IsValidTexture())
                MenuAction.SliceTexture(Texture2dObject, SliceDataObject);
            else
                Log(isTextureEmpty: true);
        }

        if (GUILayout.Button("Reset"))
        {
            if (IsValidTexture())
                MenuAction.ResetTexture(Texture2dObject);
            else
                Log(isTextureEmpty: true);
        }

        if (GUILayout.Button("Delete"))
        {
            if (IsValidTexture())
                MenuAction.DeleteTexture(Texture2dObject);
            else
                Log(isTextureEmpty: true);
        }
        EditorGUILayout.EndHorizontal();
        #endregion

        #region INIT
        EditorGUILayout.Space(10);

        GUILayout.Label("INIT", EditorStyles.largeLabel);
        EditorGUILayout.HelpBox("Assign to [Source Texture] preferably a less detailed unsliced-sprite and hit [Assign Preview Image].\n" +
                                "You must assign to [Scriptable Object] field before proceeding with the above operations.", MessageType.Info);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Scriptable Object", EditorStyles.boldLabel, GUILayout.Width(150));
        PuzzleItemObject = EditorGUILayout.ObjectField(PuzzleItemObject, typeof(PuzzleItem), false) as PuzzleItem;
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Assign Preview Image"))
        {
            if (PuzzleItemObject && _texturePath != Nil)
                Initializer.AssignPreviewImg(PuzzleItemObject, _texturePath);
            else
                Log(isTextureOrPuzzleEmpty: true);
        }

        if (GUILayout.Button("Initialize"))
        {
            if (PuzzleItemObject && _texturePath != Nil)
                Initializer.InitToScriptableObj(PuzzleItemObject, _texturePath);
            else
                Log(isTextureOrPuzzleEmpty: true);
        }

        EditorGUILayout.EndHorizontal();
        #endregion

        EditorGUILayout.Space(30);

        if (GUILayout.Button(new GUIContent("RESET MENU", "Clears all user settings, leaving all fields blank!")))
        {
            if (AssetDatabase.DeleteAsset(Path.MenuDataPath))
            {
                Logging.Log("Success!");
                serializedObject = null;
                OnLoad();
            }
            else
                Logging.LogWarning("Failed!");
        }

        // Writes back all modified values into the real instance.
        serializedObject?.ApplyModifiedProperties();
    }

    private PuzzleItem PuzzleItemObject
    {
        get => (PuzzleItem)_puzzleItem.objectReferenceValue;
        set => _puzzleItem.objectReferenceValue = value;
    }

    private SliceData SliceDataObject
    {
        get => (SliceData)_sliceData.objectReferenceValue;
        set => _sliceData.objectReferenceValue = value;
    }

    private Texture2D Texture2dObject
    {
        get => (Texture2D)_texture2D.objectReferenceValue;
        set => _texture2D.objectReferenceValue = value;
    }

    #region Utility Methods
    private static void DrawHelpBox(string optionalText,
        string textureName = Nil,
        string path = Nil,
        string slicedPath = Nil,
        MessageType messageType = MessageType.Info)
    {
        EditorGUILayout.HelpBox(
            optionalText +
            $"\nName: {textureName}\nPath: {_texturePath = path}\nSliced Path: {slicedPath}",
            messageType);
    }

    private void RenderPreviewLabel(string suffix = null)
    {
        _previewPath = !string.IsNullOrEmpty(suffix) ? $"{Texture2dObject.name}({suffix})" : Nil;

        GUILayout.Label(
            $"Preview: {_previewPath}",
            EditorStyles.miniBoldLabel);
    }

    private bool IsValidTexture()
    {
        return _texture2D.objectReferenceValue != null ? _texture2D.objectReferenceValue : null;
    }

    private static void Log(bool isTextureEmpty = false, bool isTextureOrPuzzleEmpty = false)
    {
        if (isTextureEmpty)
            Logging.LogError($"Failed! <b>{nameof(_texture2D)}</b> must not be empty");

        if (isTextureOrPuzzleEmpty)
            Logging.LogError($"Failed! <b>{nameof(_texturePath)} or {nameof(_puzzleItem)}</b> must not be empty.");
    }
    #endregion
}
#endif