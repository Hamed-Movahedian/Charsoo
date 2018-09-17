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

        if (!_playingPuzzle.Solved)
        {
            _playingPuzzle.Solved = true;
            LocalDBController.InsertOrReplace(_playingPuzzle);
            ZPlayerPrefs.SetInt("LastPlayedPuzzle", _playingPuzzle.ID);
        }


        TableQuery<Puzzle> puzzles = LocalDBController.Table<Puzzle>().
            SqlWhere(p => p.CategoryID == _playingPuzzle.CategoryID);

        Puzzle nextPuzzle = puzzles.FirstOrDefault(p => p.Row == _playingPuzzle.Row + 1);

        if (nextPuzzle==null || nextPuzzle.Solved)
        {
            FollowMachine.SetOutput("No Next Puzzle");
            return;
        }


        nextPuzzle.Paid = true;
        LocalDBController.InsertOrReplace(nextPuzzle);

        PuzzleList.SetForSpawn(nextPuzzle,!nextPuzzle.Solved);
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

    [FollowMachine("Show Last Played Puzzle", "Play,No Last Puzzle")]
    public void ShowLastPlayedPuzzle()
    {
        int lastPuzzleID = ZPlayerPrefs.GetInt("LastPlayedPuzzle");
        if (lastPuzzleID == 0)
        {
            FollowMachine.SetOutput("No Last Puzzle");
            return;
        }

        int? categoryID = LocalDBController.Table<Puzzle>().FirstOrDefault(p=>p.ID==lastPuzzleID).CategoryID;
        if (categoryID != null)
        {
            int id = categoryID.Value;
            Category category = LocalDBController.Table<Category>().First(c=>c.ID==id);
            Debug.Log(category.ID);
            PuzzleList.CategoryWindow.Select(category);
            FollowMachine.SetOutput("Play");
            return;
        }
        FollowMachine.SetOutput("No Last Puzzle");

    }

}
