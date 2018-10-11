using System;
using System.Linq;
using SQLite4Unity3d;
using UnityEngine;

public class Category : BaseTable
{

    public string Name { get; set; }
    public string Icon { get; set; }
    public int? ParentID { get; set; }
    public int? PrerequisiteID { get; set; }
    public int Price { get; set; }
    public bool Visit { get; set; }
    public int Row { get; set; }

    public bool IsUnlocked
    {
        get
        {
            var pId = "C-P-" + ID;
            return LocalDBController.Table<Purchases>().SqlWhere(p => p.PurchaseID == pId).Any();
        }
    }

    public bool IsPurchased
    {
        get
        {
            var pId = ("C-" + ID);

            return LocalDBController.Table<Purchases>().SqlWhere(p =>p.PurchaseID == pId).Any();
        }
    }

    public bool Completed
    {
        get
        {
            var puzzles = LocalDBController
                .Table<Puzzle>().SqlWhere(p => p.CategoryID == ID);

            foreach (Puzzle p in puzzles)
                if (!p.Solved) return false;
            return true;
        }
    }
}
