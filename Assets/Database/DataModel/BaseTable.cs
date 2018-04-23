using System;
using SQLite4Unity3d;

public class BaseTable : object
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }

    public DateTime LastUpdate { get; set; }
}
