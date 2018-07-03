using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.Utilities;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Partitioner))]
public class PartionerEditor : Editor
{
    private Partitioner _partitioner;
    private double _startTime;

    public override void OnInspectorGUI()
    {
        _partitioner = target as Partitioner;

        if (GUILayout.Button("Partition"))
        {
            // Record start time
            _startTime = EditorApplication.timeSinceStartup;
            MgsCoroutine.GetTime = GetTime;
            MgsCoroutine.Start(
                _partitioner.PortionLetters(),
                () => EditorUtility.DisplayCancelableProgressBar(MgsCoroutine.Title, MgsCoroutine.Info, MgsCoroutine.Percentage),
                0.1);

            EditorUtility.ClearProgressBar();

            if(_partitioner.PartitionSuccessfully)
                Debug.Log(
                    string.Format("Connect in {0} try. ({1} invalid results)" + " in {2} sec.", 
                    _partitioner.TryCount, 
                    _partitioner.InvalidResults, 
                    EditorApplication.timeSinceStartup - _startTime));
            else
                Debug.LogError(string.Format("Portion failed with {0} Invalid results !!!", _partitioner.InvalidResults));

        }

        if (GUILayout.Button("Shuffle"))
        {
            _partitioner.Undo= Undo.RecordObject;
            _partitioner.Shuffle();
        }

        if (GUILayout.Button("Compress"))
        {
            _partitioner.Undo = Undo.RecordObject;
            _partitioner.Compress();
        }

        if (GUILayout.Button("Rotate"))
        {
            _partitioner.Undo = Undo.RecordObject;
            _partitioner.Rotate();
        }
        DrawDefaultInspector();
    }

    private static double GetTime()
    {
        return EditorApplication.timeSinceStartup;
    }
}
