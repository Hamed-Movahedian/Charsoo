using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PuzzleRegenerator : EditorWindow
{
    private string _clue = "";
    private PuzzleComponent _puzzle;
    private WordSet _wordSet;

    #region Window

    [MenuItem("Word Game/ReGenerator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(PuzzleRegenerator));
    }

    #endregion


    void OnGUI()
    {
        #region Get clue and category

        GUILayout.Label("Generator", EditorStyles.boldLabel);
        _clue = EditorGUILayout.TextField("Clue", _clue);



        _puzzle = (PuzzleComponent)EditorGUILayout.ObjectField(_puzzle, typeof(PuzzleComponent), true);

        #endregion

        #region Spawn wordSet

        if (GUILayout.Button("Spawn"))
        {
            var Wordspawner = FindObjectOfType<WordSpawner>();

            if (Wordspawner == null)
            {
                EditorUtility.DisplayDialog("Error", "Can't find word spawner", "Ok");
                return;
            }



            Wordspawner.EditorInstatiate = EditorInstantiate;

            WordSet wSet = new WordSet();
            JsonUtility.FromJsonOverwrite(StringCompressor.DecompressString(_puzzle.Content), wSet);
            Wordspawner.WordSet = wSet;
            _wordSet = wSet;
            Wordspawner.SpawnWords();

        }

        #endregion

        
        #region Spawn wordSet

        if (GUILayout.Button("Regenerate"))
        {
            var wordGenerator = FindObjectOfType<WordSetGenerator>();
            wordGenerator.AllWords = "";
            if (wordGenerator == null)
            {
                EditorUtility.DisplayDialog("Error", "Can't find word spawner", "Ok");
                return;
            }

            foreach (var word in _wordSet.Words)
            {
                wordGenerator.AllWords += word.Name + " ";
            }
            
        }

        #endregion

        
        if (GUILayout.Button("Save"))
        {
            
            #region Get word manager

            var wordManagers = FindObjectsOfType<WordManager>();

            if (wordManagers.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "Word manager not found!", "OK");
                return;
            }

            if (wordManagers.Length > 1)
            {
                EditorUtility.DisplayDialog("Error", "More than one Word manager!", "OK");
                return;
            }

            var wordManager = wordManagers[0];

            #endregion




            #region Create New wordSet

            WordSet wordSet = new WordSet();

            wordSet.Clue = _clue;
            wordSet.Words = new List<SWord>();

            foreach (var word in wordManager.GetComponentsInChildren<Word>())
            {
                wordSet.Words.Add(new SWord(word));
            }

            #endregion


            #region Save wordset to database
            // create category in database
            var puzzle = new Puzzle
            {
                ID = 1,
                CategoryID = _puzzle.PuzzleData.CategoryID,
                Clue = _puzzle.Clue,
                Row = _puzzle.PuzzleData.Row,
                Content = StringCompressor.CompressString(JsonUtility.ToJson(wordSet)),
                LastUpdate = DateTime.Now
            };

            Puzzle newPuzzle = ServerEditor.Post<Puzzle>(@"Puzzles/Create", puzzle, "Create puzzle", "Create");

            if (newPuzzle == null)
            {
                EditorUtility.DisplayDialog("Error", "Puzzle can't save in server!", "OK");
                return;
            }

            //_category.AddPuzzle(newPuzzle);

            #endregion
        }
    }
    private Letter EditorInstantiate(Letter letterPrefab)
    {
        return (Letter)PrefabUtility.InstantiatePrefab(letterPrefab);
    }

}
