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

            _wg.ShowProgressBar = ShowProgresBar;
            _wg.MakeWordSet();
            EditorUtility.ClearProgressBar();
            _wg.ShowProgressBar = null;
            _wg.EditorInstatiate = EditorInstantiate;
            _wg.SpawnWordSet();
        }

        if (GUILayout.Button("Spawn Next"))
        {
            _wg.EditorInstatiate = EditorInstantiate;
            _wg.NextResultIndex++;
            _wg.SpawnWordSet();
        }
        if (GUILayout.Button("Spawn Previous"))
        {
            _wg.EditorInstatiate = EditorInstantiate;
            _wg.NextResultIndex--;
            _wg.SpawnWordSet();
        }

        DrawDefaultInspector();
    }

    private static double GetTime()
    {
        return EditorApplication.timeSinceStartup;
    }

    public void ShowProgresBar(string info, float v)
    {
        if (EditorUtility.DisplayCancelableProgressBar("Generate", info, v))
            _wg.Cancel();


    }

    private Letter EditorInstantiate(Letter LetterPrefab)
    {
        return (Letter) PrefabUtility.InstantiatePrefab(LetterPrefab);
    }

    /*[CustomEditor(typeof(Recorder))]
    public class RecorderEditor : Editor
    {
        private Recorder _recorder;

        public override void OnInspectorGUI()
        {
            _recorder = target as Recorder;


            if (GUILayout.Button("Save"))
            {
                

                if (_recorder.Clue == "")
                {
                    EditorUtility.DisplayDialog("Error", "Specify clue!", "OK");
                    return;
                }
                WordSet wordSet = _recorder.Save();

                string fileName = EditorUtility.SaveFilePanelInProject("Save WordSet Asset", "wordSet", "asset", "Save Asset");

                AssetDatabase.CreateAsset(wordSet,fileName);
                AssetDatabase.SaveAssets();

                if (_recorder.Category == null)
                {
                    EditorUtility.DisplayDialog("Error", "Specify package!", "OK");
                }
                else
                {
                    if (_recorder.Category.GetComponentsInChildren<CategoryComponent>().Length>1)
                    {
                        EditorUtility.DisplayDialog("Error", "Category "+_recorder.Category.Name+" has subcategory!", "OK");
                        return;
                    }

                    if (!_recorder.Category.AddPuzzle())
                        EditorUtility.DisplayDialog("Error", "Puzzle can't save in database!", "OK");

                }

            }
            DrawDefaultInspector();
        }

    }*/
}
