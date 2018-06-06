using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WordSetGenerator))]
public class WordGeneratorEditor : Editor
{
    private WordSetGenerator _wg;

    public override void OnInspectorGUI()
    {
        _wg = target as WordSetGenerator;


        if (GUILayout.Button("Generate"))
        {
            _wg.GetTime = GetTime;

            _wg.ShowProgressBar = ShowProgressBar;
            _wg.MakeWordSet();
            EditorUtility.ClearProgressBar();
            _wg.ShowProgressBar = null;
            _wg.EditorInstantiate = EditorInstantiate;
            _wg.SpawnWordSet();
        }

        if (GUILayout.Button("Spawn Next"))
        {
            _wg.EditorInstantiate = EditorInstantiate;
            _wg.NextResultIndex++;
            _wg.SpawnWordSet();
        }
        if (GUILayout.Button("Spawn Previous"))
        {
            _wg.EditorInstantiate = EditorInstantiate;
            _wg.NextResultIndex--;
            _wg.SpawnWordSet();
        }

        DrawDefaultInspector();
    }

    private static double GetTime()
    {
        return EditorApplication.timeSinceStartup;
    }

    public void ShowProgressBar(string info, float v)
    {
        if (EditorUtility.DisplayCancelableProgressBar("Generate", info, v))
            _wg.Cancel();


    }

    private Letter EditorInstantiate(Letter LetterPrefab)
    {
        return (Letter) PrefabUtility.InstantiatePrefab(LetterPrefab);
    }

}
