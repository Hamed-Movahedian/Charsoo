using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FollowMachineEditor.Server;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[CustomEditor(typeof(DatabaseComponent))]
public class DatabaseEditor : Editor
{
    private DatabaseComponent _databaseComponent;

    public override void OnInspectorGUI()
    {
        _databaseComponent = target as DatabaseComponent;

        #region ReloadAll

        if (GUILayout.Button("ReloadAll"))
        {
            // read all categories
            var categories = ServerEditor.Get<List<Category>>(@"Categories", "Download categories", "Download");

            if (categories == null)
                return;

            _databaseComponent.SetCategories(categories);

            // read all puzzles
            List<Puzzle> puzzles = ServerEditor.Get<List<Puzzle>>(@"Puzzles", "Download puzzles", "Download");

            if (puzzles == null)
                return;

            _databaseComponent.SetPuzzles(puzzles);

            // create game objects
            _databaseComponent.ReloadAll();
        }

        #endregion

        #region UpdateData All

        if (GUILayout.Button("UpdateData All"))
        {
            // Update categories
            var categoryComponents = _databaseComponent.GetComponentsInChildren<CategoryComponent>();

            foreach (var categoryComponent in categoryComponents)
                if (categoryComponent.Dirty)
                    CategoryEditor.UpdateServer(categoryComponent);

            //Update puzzles
            var puzzleComponents = _databaseComponent.GetComponentsInChildren<PuzzleComponent>();

            foreach (var puzzleComponent in puzzleComponents)
                if (puzzleComponent.Dirty)
                    PuzzleEditor.UpdateServer(puzzleComponent);
        }

        #endregion

        #region Sync localDB

        if (GUILayout.Button("Sync All with localDB"))
        {
            // read all categories
            var categories = ServerEditor.Get<List<Category>>(@"Categories", "Download categories", "Download");

            if (categories == null)
                return;

            // read all puzzles
            List<Puzzle> puzzles = ServerEditor.Get<List<Puzzle>>(@"Puzzles", "Download puzzles", "Download");

            if (puzzles == null)
                return;

            LocalDBController.DeleteAll<Category>();
            LocalDBController.DeleteAll<Puzzle>();

            foreach (Category category in categories)
                LocalDBController.InsertOrReplace(category);

            foreach (Puzzle puzzle in puzzles)
                LocalDBController.InsertOrReplace(puzzle);
        }

        #endregion

        if (GUILayout.Button("Update Categories in Server From LOCAL"))
        {
            SendCategoriesToServer();
        }

        if (GUILayout.Button("Update Puzzles in Server From LOCAL"))
        {
            SendPuzzlesToServer();
        }
    }

    public void SendCategoriesToServer()
    {
        List<Category> cats = new List<Category>();
        cats.AddRange(LocalDBController.Table<Category>().ToList());

        while (cats.Any())
            foreach (Category c in cats)
            {
                if (c.PrerequisiteID == null || cats.FirstOrDefault(t => t.ID == c.PrerequisiteID) == null)
                    if (c.ParentID == null || cats.FirstOrDefault(t => t.ID == c.ParentID) == null)
                    {
                        ServerEditor.Post(@"Categories/Create", c, "Update category", "Update");
                        cats.Remove(c);
                        break;
                    }
            }
    }

    public void SendPuzzlesToServer()
    {
        List<Puzzle> puzzles = new List<Puzzle>();
        puzzles.AddRange(LocalDBController.Table<Puzzle>().ToList());
        Debug.Log(puzzles.Count);

        foreach (Puzzle p in puzzles)
        {
            ServerEditor.Post(@"Puzzles/Create", p, "Update Puzzle", "Update");
        }
    }
}