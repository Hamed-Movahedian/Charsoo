using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEditor;
using UnityEngine;

public class WordSpawnerEditor : EditorWindow
{
    public WordSpawner Wordspawner;
    private WordSet _wordSet;
    bool _useJason = false;

    private Letter EditorInstantiate(Letter letterPrefab)
    {
        return (Letter)PrefabUtility.InstantiatePrefab(letterPrefab);
    }

    [MenuItem("Window/Word Spawner")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(WordSpawnerEditor));
    }

    void OnGUI()
    {
        GUILayout.Label("Word Spawner", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Word Set");
        _wordSet = (WordSet)EditorGUILayout.ObjectField(_wordSet, typeof(WordSet), true);
        EditorGUILayout.EndHorizontal();

        if (!Wordspawner)
            Wordspawner = FindObjectOfType<WordSpawner>();

        if (_wordSet)
        {
            EditorGUILayout.LabelField("Word Set Clue : ", ArabicFixer.Fix(_wordSet.Clue));
            EditorGUILayout.LabelField("Word Count : ", _wordSet.Words.Count.ToString());
            EditorGUILayout.LabelField("Nonunique Word Count : ", _wordSet.NonuniqWords.Count.ToString());

            EditorGUILayout.BeginHorizontal();
            _useJason = EditorGUILayout.Toggle("From Json ", _useJason);
            if (GUILayout.Button("Spawn"))
                {
                    Wordspawner.EditorInstatiate = EditorInstantiate;
                    Wordspawner.WordSet = _useJason? ToJason(_wordSet):_wordSet;
                    Wordspawner.SpawnWords();
                }
            EditorGUILayout.EndHorizontal();
        }
    }

    private WordSet ToJason(WordSet wordset)
    {
        string s = JsonUtility.ToJson(wordset);
        Debug.Log(s.Length);

        string compressString =MyJsonUtility.CompressString(s) ;
        Debug.Log(compressString.Length);

        string decompressString = MyJsonUtility.DeCompressString(compressString);
        Debug.Log(decompressString.Length);

        WordSet newSet = new WordSet();
        JsonUtility.FromJsonOverwrite(decompressString, newSet);
        return newSet;
    }
}
