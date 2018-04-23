using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Partioner))]
public class PartionerEditor : Editor
{
    private Partioner _partioner;

    public override void OnInspectorGUI()
    {
        _partioner = target as Partioner;

        _partioner.GetTime = GetTime;
        

        _partioner.ShowProgressBar = ShowProgresBar;

        if (GUILayout.Button("Partition"))
        {
            _partioner.PartionLetters();
            EditorUtility.ClearProgressBar();
        }

        if (GUILayout.Button("Shuffle"))
        {
            _partioner.Undo= Undo.RecordObject;
            _partioner.Shafle();
        }

        if (GUILayout.Button("Compress"))
        {
            _partioner.Undo = Undo.RecordObject;
            _partioner.Compress();
        }

        if (GUILayout.Button("Rotate"))
        {
            _partioner.Undo = Undo.RecordObject;
            _partioner.Rotate();
        }
        DrawDefaultInspector();
    }

    private static double GetTime()
    {
        return EditorApplication.timeSinceStartup;
    }

    public bool ShowProgresBar(string info, float v)
    {
        if (EditorUtility.DisplayCancelableProgressBar("Partitioning..", info, v))
        {
            _partioner.Cancel();
            return false;
        }
        return true;

    }

}
