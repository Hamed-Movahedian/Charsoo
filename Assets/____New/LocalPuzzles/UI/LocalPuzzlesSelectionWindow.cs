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

    public Puzzle PlayingPuzzle;

    public override void Refresh()
    {
        PlayingCategory = CategoryWindow.SelectedCategory;
        CategoryName.text = PlayingCategory != null ? PersianFixer.Fix(PlayingCategory.Name) : PersianFixer.Fix("جدول های اصلی");
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
        PlayingPuzzle = selectedPuzzle;
        //Puzzle selectedPuzzle
        WordSet wordSet = selectedPuzzle.GetWordSet();
        Singleton.Instance.WordSpawner.ClearTable();
        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.Clue = selectedPuzzle.Clue;
        Singleton.Instance.WordSpawner.PuzzleRow = (selectedPuzzle.Row+1).ToString();
        Singleton.Instance.WordSpawner.PuzzleReward = !selectedPuzzle.Solved;
        Singleton.Instance.WordSpawner.PuzzleID = selectedPuzzle.ID;

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
        Debug.Log("SelectedLockItem");
        Close("SelectedLockItem");
    }

    public void UnlockCategoryPuzzles()
    {
        Purchases purchase = new Purchases
        {
            LastUpdate = DateTime.Now,
            PlayerID = LocalDBController.Table<PlayerInfo>().FirstOrDefault().PlayerID,
            PurchaseID = "C-P-" + PlayingCategory.ID
        };

        LocalDBController.InsertOrReplace(purchase);
    }
}
