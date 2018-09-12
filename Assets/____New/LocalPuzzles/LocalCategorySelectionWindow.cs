using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;

public class LocalCategorySelectionWindow : UIMenuItemList
{
    public override void Refresh()
    {
        int? parentID = SelectedCategory?.ID;
        var categories = LocalDBController.Table<Category>().SqlWhere(c => c.ParentID == parentID).ToList();
        categories.Sort((p1, p2) => p1.Row.CompareTo(p2.ID));
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

        if (SelectedCategory == null)
            FollowMachine.SetOutput("ExitLocalPuzzles");
        else
        {
            Category pc = LocalDBController.Table<Category>().SqlWhere(c => c.ID == SelectedCategory.ParentID).ToList()[0];
            Select(pc);
            FollowMachine.SetOutput("CategoryParent");

        }
    }

    public Category SelectedCategory => (Category)GetSelectedItem();

}
