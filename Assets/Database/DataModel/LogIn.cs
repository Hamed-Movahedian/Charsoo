using System;
using SQLite4Unity3d;
using UnityEngine;

public class LogIn 
{
    public int? PlayerID { get; set; }

    public string DeviceID { get; set; }
    public DateTime LoginTime { get; set; }
    public float Latitude { get; set; }
    public float Longitude { get; set; }
 
}
