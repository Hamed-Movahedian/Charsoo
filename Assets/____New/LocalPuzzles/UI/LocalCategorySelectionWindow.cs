using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class LocalCategorySelectionWindow : UIMenuItemList
{
    public Text CategoryName;
    private Category _clickedCategory;

    public override void Refresh()
    {
        int? parentID = 3;
        if (SelectedCategory != null)
            parentID = SelectedCategory?.ID;


        var categories = LocalDBController.Table<Category>().SqlWhere(c => c.ParentID == parentID).ToList();
        categories.Sort((p1, p2) => p1.Row.CompareTo(p2.Row));
        CategoryName.text = SelectedCategory != null ? PersianFixer.Fix(SelectedCategory.Name) : PersianFixer.Fix("جدول های اصلی");
        UpdateItems(categories.Cast<object>());
    }

    [FollowMachine("Refresh List", "Child Category List,Puzzle List")]
    public void ListToShow()
    {
        if (LocalDBController.Table<Category>().SqlWhere(c => c.ParentID == SelectedCategory.ID).ToList().Count == 0)
            FollowMachine.SetOutput("Puzzle List");
        else
            FollowMachine.SetOutput("Child Category List");
    }

    [FollowMachine("Back", "ExitLocalPuzzles,CategoryParent")]
    public void Back()
    {

        if (SelectedCategory == null || SelectedCategory.ParentID == 3)
            FollowMachine.SetOutput("ExitLocalPuzzles");
        else
        {
            if (SelectedCategory.ParentID == null )
            {
                //Select((Category)null);
                FollowMachine.SetOutput("ExitLocalPuzzles");
                return;
            }

            Category pc =
                LocalDBController.Table<Category>()
                .SqlWhere(c => c.ID == SelectedCategory.ParentID)
                .ToList()[0];

            Select(pc);
            FollowMachine.SetOutput("CategoryParent");

        }
    }

    public Category SelectedCategory => (Category)GetSelectedItem();

    public void UnlockCategory()
    {
        Purchases purchase = new Purchases
        {
            LastUpdate = DateTime.Now,
            PlayerID = LocalDBController.Table<PlayerInfo>().FirstOrDefault().PlayerID,
            PurchaseID = "C-" + _clickedCategory.ID
        };
        LocalDBController.InsertOrReplace(purchase);

    }

    public void LockSelect(Category data)
    {
        _clickedCategory = data;
        Close("SelectedLockItem");
    }

}
