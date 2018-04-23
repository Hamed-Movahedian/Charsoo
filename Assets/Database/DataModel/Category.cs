using System;
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
}
