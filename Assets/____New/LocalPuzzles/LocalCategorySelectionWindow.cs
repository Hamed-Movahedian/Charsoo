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
        int? parentID = SelectedCategory?.ID;
        var categories = LocalDBController.Table<Category>().SqlWhere(c => c.ParentID == parentID).ToList();
        if (categories.Count == 0)
            FollowMachine.SetOutput("Puzzle List");
        else
            FollowMachine.SetOutput("Child Category List");

    }


    public Category SelectedCategory => (Category)GetSelectedItem();

}
