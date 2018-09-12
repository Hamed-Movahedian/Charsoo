using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalPuzzlesSelectionWindow : UIMenuItemList
{
    public LocalCategorySelectionWindow CategoryWindow;

    public override void Refresh()
    {
        var categories = LocalDBController.Table<Puzzle>().SqlWhere(p=>p.CategoryID== CategoryWindow.SelectedCategory.ID).ToList();
        categories.Sort((p1, p2) => p1.Row.CompareTo(p2.Row));
        UpdateItems(categories.Cast<object>());
    }
    public Puzzle SelectedPuzzle => (Puzzle)GetSelectedItem();

}
