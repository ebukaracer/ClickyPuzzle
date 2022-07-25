#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Setup))]
public class PuzzleEditor : Editor
{
    //This will delegate control the class provided

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var setup = target as Setup;

        GUILayout.Space(15f);

        GUILayout.BeginHorizontal();

        bool initButton = GUILayout.Button("Initialize");

        GUILayout.EndHorizontal();

        GUILayout.Space(15f);

        if (initButton)
        {
            setup.Init();
        }
    }

}

#endif

