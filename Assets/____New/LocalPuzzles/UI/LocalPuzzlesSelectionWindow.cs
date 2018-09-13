using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using FMachine;
using FollowMachineDll.Attributes;
using SQLite4Unity3d;
using UnityEngine;
using UnityEngine.UI;

public class LocalPuzzlesSelectionWindow : UIMenuItemList
{
    public LocalCategorySelectionWindow CategoryWindow;
    public Text CategoryName;

    private Puzzle _playingPuzzle;
    private Category _playingCategory;

    public override void Refresh()
    {
        _playingCategory = CategoryWindow.SelectedCategory;
        CategoryName.text = _playingCategory != null ? ArabicFixer.Fix(_playingCategory.Name) : ArabicFixer.Fix("جدول های اصلی");
        var puzzles = LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == _playingCategory.ID).ToList();
        puzzles.Sort((p1, p2) => p1.Row.CompareTo(p2.Row));
        UpdateItems(puzzles.Cast<object>());
    }
    public Puzzle SelectedPuzzle => (Puzzle)GetSelectedItem();

    public void SetForSpawn()
    {
        SetForSpawn(SelectedPuzzle);
    }

    public void SetForSpawn(Puzzle selectedPuzzle)
    {
        _playingPuzzle = selectedPuzzle;
        //Puzzle selectedPuzzle
        WordSet wordSet = selectedPuzzle.GetWordSet();

        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.Clue = selectedPuzzle.Clue;
        Singleton.Instance.WordSpawner.PuzzleRow = selectedPuzzle.Row.ToString();

        Singleton.Instance.WordSpawner.EditorInstatiate = null;
    }

    [FollowMachine("Prepare next puzzle for spawn", "Play Next,No Next Puzzle")]
    public void SetNextPuzzle()
    {
        _playingPuzzle.Solved = true;
        LocalDBController.InsertOrReplace(_playingPuzzle);
        TableQuery<Puzzle> puzzles = LocalDBController.Table<Puzzle>().
            SqlWhere(p => p.CategoryID == _playingPuzzle.CategoryID);

        Puzzle nextPuzzle = puzzles.First(p => p.Row == _playingPuzzle.Row + 1);
        if (nextPuzzle != null)
        {
            nextPuzzle.Paid = true;
            LocalDBController.InsertOrReplace(nextPuzzle);
            SetForSpawn(nextPuzzle);
            FollowMachine.SetOutput("Play Next");
        }
        else
            FollowMachine.SetOutput("No Next Puzzle");
    }

    public void Back()
    {
        if (_playingCategory.ParentID != null)
            CategoryWindow.Select(
                LocalDBController.Table<Category>().SqlWhere(c => c.ID == _playingCategory.ParentID).ToList()[0]//.ElementAt(_playingCategory.ParentID.Value)
                );
    }
    
    public void LockSelect()
    {
        Close("SelectedLockItem");
    }

    public void UnlockCategoryPuzzles()
    {
        foreach (Puzzle puzzle in LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == _playingCategory.ID))
        {
            puzzle.Paid = true;
            LocalDBController.InsertOrReplace(puzzle);
            Purchases purchase=new Purchases
            {
                LastUpdate = DateTime.Now,
                PlayerID = LocalDBController.Table<PlayerInfo>().FirstOrDefault().PlayerID,
                PurchaseID = "C-P-"+ _playingCategory.ID
            };
            LocalDBController.InsertOrReplace(purchase);
        }
    }

}
