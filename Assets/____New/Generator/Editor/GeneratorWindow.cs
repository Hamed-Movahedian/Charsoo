using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FollowMachineEditor.Server;
using UnityEditor;
using UnityEngine;

public class GeneratorWindow : EditorWindow
{
    private string _clue = "";
    private CategoryComponent _category;

    #region Window

    [MenuItem("Word Game/Generator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(GeneratorWindow));
    }


    #endregion

    void OnGUI()
    {
        #region Get clue and category

        GUILayout.Label("Generator", EditorStyles.boldLabel);
        _clue = EditorGUILayout.TextField("Clue", _clue);
        _category = (CategoryComponent)EditorGUILayout.ObjectField(_category, typeof(CategoryComponent), true);


        #endregion

        if (GUILayout.Button("Save"))
        {
            #region Initial Checks

            if (_clue == "")
            {
                EditorUtility.DisplayDialog("Error", "Specify clue!", "OK");
                return;
            }
            if (_category == null)
            {
                EditorUtility.DisplayDialog("Error", "Specify category!", "OK");
                return;
            }
            if (_category.GetComponentsInChildren<CategoryComponent>().Length > 1)
            {
                EditorUtility.DisplayDialog("Error", "Category " + _category.Name + " has subcategory!", "OK");
                return;
            }


            #endregion

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

            #region Create wordSet

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
                CategoryID = _category.ID,
                Clue = wordSet.Clue,
                Row = _category.transform.childCount,
                Content = StringCompressor.CompressString(JsonUtility.ToJson(wordSet)),
                LastUpdate = DateTime.Now
            };

            Puzzle newPuzzle = ServerEditor.Post<Puzzle>(@"Puzzles/Create", puzzle, "Create puzzle", "Create");

            if (newPuzzle==null)
            {
                EditorUtility.DisplayDialog("Error", "Puzzle can't save in server!", "OK");
                return;
            }

            _category.AddPuzzle(newPuzzle);

            #endregion
        }
    }

}


