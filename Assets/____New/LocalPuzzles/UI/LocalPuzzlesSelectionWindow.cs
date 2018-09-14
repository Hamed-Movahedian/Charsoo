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
    public Category PlayingCategory;

    private Puzzle _playingPuzzle;

    public override void Refresh()
    {
        PlayingCategory = CategoryWindow.SelectedCategory;
        CategoryName.text = PlayingCategory != null ? ArabicFixer.Fix(PlayingCategory.Name) : ArabicFixer.Fix("جدول های اصلی");
        var puzzles = LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == PlayingCategory.ID).ToList();
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

    public void Back()
    {
        if (PlayingCategory.ParentID != null)
            CategoryWindow.Select(
                LocalDBController.Table<Category>().SqlWhere(c => c.ID == PlayingCategory.ParentID).ToList()[0]//.ElementAt(_playingCategory.ParentID.Value)
                );
    }

    public void LockSelect()
    {
        Close("SelectedLockItem");
    }

    public void UnlockCategoryPuzzles()
    {

        foreach (Puzzle puzzle in LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == PlayingCategory.ID))
        {
            puzzle.Paid = true;
            LocalDBController.InsertOrReplace(puzzle);
            Purchases purchase = new Purchases
            {
                LastUpdate = DateTime.Now,
                PlayerID = LocalDBController.Table<PlayerInfo>().FirstOrDefault().PlayerID,
                PurchaseID = "C-P-" + PlayingCategory.ID
            };
            LocalDBController.InsertOrReplace(purchase);
        }
    }



}
