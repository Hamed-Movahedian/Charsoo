using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEditor;
using UnityEngine;
using MgsCommonLib.Utilities;

[CustomEditor(typeof(WordSetGenerator))]
public class WordGeneratorEditor : Editor
{
    private WordSetGenerator _wg;

    public override void OnInspectorGUI()
    {
        _wg = target as WordSetGenerator;


        if (GUILayout.Button("Generate"))
        {
            MgsCoroutine.GetTime = GetTime;
            MgsCoroutine.Start(
                _wg.MakeWordSet(), 
                () => EditorUtility.DisplayCancelableProgressBar(MgsCoroutine.Title, MgsCoroutine.Info,MgsCoroutine.Percentage),
                0.1);

            EditorUtility.ClearProgressBar();

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
    
    private Letter EditorInstantiate(Letter LetterPrefab)
    {
        return (Letter) PrefabUtility.InstantiatePrefab(LetterPrefab);
    }

}
