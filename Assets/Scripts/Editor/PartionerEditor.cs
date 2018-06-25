using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.Utilities;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Partioner))]
public class PartionerEditor : Editor
{
    private Partioner _partioner;
    private double _startTime;

    public override void OnInspectorGUI()
    {
        _partioner = target as Partioner;

        if (GUILayout.Button("Partition"))
        {
            // Record start time
            _startTime = EditorApplication.timeSinceStartup;

            MgsCoroutine.Start(
                _partioner.PortionLetters(),
                () => EditorUtility.DisplayCancelableProgressBar(MgsCoroutine.Title, MgsCoroutine.Info, MgsCoroutine.Percentage),
                0.1);

            EditorUtility.ClearProgressBar();

            if(_partioner.PartitionSuccessfully)
                Debug.Log(
                    string.Format("Connect in {0} try. ({1} invalid results)" + " in {2} sec.", 
                    _partioner.TryCount, 
                    _partioner.InvalidResults, 
                    EditorApplication.timeSinceStartup - _startTime));
            else
                Debug.LogError(string.Format("Portion failed with {0} Invalid results !!!", _partioner.InvalidResults));

        }

        if (GUILayout.Button("Shuffle"))
        {
            _partioner.Undo= Undo.RecordObject;
            _partioner.Shuffle();
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
}
