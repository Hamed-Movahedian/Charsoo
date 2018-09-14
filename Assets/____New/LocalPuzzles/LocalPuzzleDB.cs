using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using SQLite4Unity3d;
using UnityEngine;

public class LocalPuzzleDB : MonoBehaviour
{
    private Puzzle _playingPuzzle;
    public LocalPuzzlesSelectionWindow PuzzleList;



    [FollowMachine("Prepare next puzzle for spawn", "Play Next,No Next Puzzle")]
    public void PuzzleSolved()
    {
        _playingPuzzle = PuzzleList.SelectedPuzzle;

        if (_playingPuzzle.Solved)
        {
            FollowMachine.SetOutput("No Next Puzzle");
            return;
        }

        _playingPuzzle.Solved = true;
        LocalDBController.InsertOrReplace(_playingPuzzle);

        ZPlayerPrefs.SetInt("LastPlayedPuzzle", _playingPuzzle.ID);

        TableQuery<Puzzle> puzzles = LocalDBController.Table<Puzzle>().
            SqlWhere(p => p.CategoryID == _playingPuzzle.CategoryID);

        Debug.Log(puzzles.Count());


        if (puzzles.Count() <= _playingPuzzle.Row + 1)
        {
            FollowMachine.SetOutput("No Next Puzzle");
            return;
        }


        Puzzle nextPuzzle = puzzles.First(p => p.Row == _playingPuzzle.Row + 1);
        nextPuzzle.Paid = true;
        LocalDBController.InsertOrReplace(nextPuzzle);

        PuzzleList.SetForSpawn(nextPuzzle);
        FollowMachine.SetOutput("Play Next");
    }
    
    public void UnlockCategoryPuzzles()
    {
        int? id = PuzzleList.PlayingCategory.ID;
        foreach (Puzzle puzzle in LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == id))
        {
            puzzle.Paid = true;
            LocalDBController.InsertOrReplace(puzzle);
        }
        Purchases purchase = new Purchases
        {
            LastUpdate = DateTime.Now,
            PlayerID = LocalDBController.Table<PlayerInfo>().FirstOrDefault().PlayerID,
            PurchaseID = "C-P-" + id
        };
        LocalDBController.InsertOrReplace(purchase);
    }


}
