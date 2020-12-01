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
        _playingPuzzle = PuzzleList.PlayingPuzzle;

        if (Singleton.Instance.WordSpawner.PuzzleReward)
        {
            ZPlayerPrefs.SetInt("LastPlayedPuzzle", _playingPuzzle.ID);
        }


        var puzzles = LocalDBController.Table<Puzzle>().
            SqlWhere(p => p.CategoryID == _playingPuzzle.CategoryID);

        Puzzle nextPuzzle = puzzles.FirstOrDefault(p => p.Row == _playingPuzzle.Row + 1);

        if (nextPuzzle == null || nextPuzzle.Solved)
        {
            FollowMachine.SetOutput("No Next Puzzle");
            return;
        }

        PuzzleList.SetForSpawn(nextPuzzle);
        FollowMachine.SetOutput("Play Next");
    }

    public void UnlockCategoryPuzzles()
    {
        int? playerID = null;
        playerID = Singleton.Instance.PlayerController.GetPlayerID;

        int? id = PuzzleList.PlayingCategory.ID;

        Purchases purchase = new Purchases
        {
            LastUpdate = DateTime.Now,
            PlayerID = playerID,
            PurchaseID = "C-P-" + id,
            Dirty = true

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

        Puzzle puzzle = LocalDBController.Table<Puzzle>().
            SqlWhere(p => p.ID == lastPuzzleID).
            FirstOrDefault();

        int? categoryID = puzzle?.CategoryID;
        if (categoryID != null)
        {
            int id = categoryID.Value;
            Category category = 
                LocalDBController.
                Table<Category>().
                SqlWhere(c => c.ID == id).
                FirstOrDefault();

            PuzzleList.CategoryWindow.Select(category);
            FollowMachine.SetOutput("Play");
            return;
        }
        FollowMachine.SetOutput("No Last Puzzle");

    }


    public IEnumerator ReportPuzzle()
    {
        string resualt = "";
        string puzzlePlayer = Singleton.Instance.WordSpawner.PuzzleID + "/" + Singleton.Instance.PlayerController.GetPlayerID;

        yield return ServerController.Post<string>(
            $@"Puzzles/Report?puzzlePlayer={puzzlePlayer}",
            null,
            // On Successfully connect to the account
            info => { resualt = info; },
            // On Error
            request =>
            {
                // Network Error !!!!!
                if (request.isNetworkError)
                    resualt="Network Error";

                // Account recovery Error !!!!
                else if (request.isHttpError)
                    resualt="Puzzle Error";
            });


    }
}
