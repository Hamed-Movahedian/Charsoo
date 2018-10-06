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
            string pID = "C-P-" + ID;
            return LocalDBController.Table<Purchases>().FirstOrDefault(p => p.PurchaseID == pID) != null;
        }
    }

    public bool IsPurchased
    {
        get
        {
            string pID = "C-" + ID;
            return LocalDBController.Table<Purchases>().FirstOrDefault(p => p.PurchaseID == pID)!=null;
        }
    }

    public bool Completed
    {
        get
        {
            foreach (Puzzle p in LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == ID))
                if (!p.Solved) return false;
            return true;
        }
    }
}
