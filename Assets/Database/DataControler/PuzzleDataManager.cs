using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class PuzzleDataManager : BaseObject
{
    void Start()
    {
        CommandController.AddListenerForCommand("AddPuzzles", AddPuzzles);
    }

    private void AddPuzzles(JToken dataToken)
    {
        // Get new categories from json
        List<Puzzle> newPuzzles = dataToken.Select(ct => ct.ToObject<Puzzle>()).ToList();

        // Add or update local db
        foreach (Puzzle puzzle in newPuzzles)
        {
            //puzzle.Visit = false;

            LocalDatabase.InsertOrReplace(puzzle);
        }

        CommandController.LastCmdTime = newPuzzles.Max(c => c.LastUpdate);

    }
}
