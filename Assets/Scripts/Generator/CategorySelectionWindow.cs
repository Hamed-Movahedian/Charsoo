using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CategorySelectionWindow : UIMenuItemList
{

    public void Refresh()
    {
        var categories = LocalDBController.Table<Category>().ToList();
        categories.Sort((p1, p2) => p1.Row.CompareTo(p2.Row));
        UpdateItems(categories.Cast<object>());
    }
}
