using System;
using SQLite4Unity3d;
using UnityEngine;

public class PlayerInfo 
{
    public PlayerInfo()
    {
        PlayerID = null;
        Name = "No Name";
        Avatar = "No Avatar";
        Telephone = "";
        Email = "";
    }

    public int? PlayerID { get; set; }
    public string Name { get; set; }
    public string Avatar { get; set; }
    public string Telephone { get; set; }
    public string Email { get; set; }

    public bool HasAccount()
    {
        return Telephone != "" || Email != "";
    }
}
