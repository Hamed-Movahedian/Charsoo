using System;
using System.Collections;
using System.Collections.Generic;
using MgsCommonLib;
using SQLite4Unity3d;
using UnityEngine;

public class LocalDBController: MgsSingleton<LocalDBController>
{
    #region DataService

    private static DataService _dataService;
    public UserPuzzleLocalDB UserPuzzles= new UserPuzzleLocalDB();

    public static DataService DataService
    {
        get
        {
            return _dataService ?? (_dataService = new DataService("OldDB.db"));
        }
    }

    #endregion

    #region Connection methods

    public static void DeleteAll<T>()
    {
        DataService.Connection.DeleteAll<T>();
    }

    public static void InsertOrReplace(object obj)
    {
        DataService.Connection.InsertOrReplace(obj);
    }

    public static TableQuery<T> Table<T>() where T : new()
    {
        return DataService.Connection.Table<T>();
    }

    public static void Update(object obj)
    {
        DataService.Connection.Update(obj);
    }

    #endregion

    #region Logins
    public static IEnumerator AddLogin(int? playerID)
    {
        LocationProvider.LocationWraper locationWraper = new LocationProvider.LocationWraper();

        yield return LocationProvider.GetLocation(locationWraper);

        DataService.Connection.Insert(new LogIn
        {
            PlayerID = playerID,
            DeviceID = SystemInfo.deviceUniqueIdentifier,
            Latitude = locationWraper.Latitude,
            Longitude = locationWraper.Longitude,
            LoginTime = DateTime.Now
        });
    }
    #endregion

    #region Missing PlayerID

    public static void FillMissingPlayerIDs(int playerID)
    {
        var command = new SQLiteCommand(DataService.Connection);

        command.CommandText = string.Format("Update LogIn set [PlayerID]={0}", playerID);
        command.ExecuteNonQuery();
        command.CommandText = string.Format("Update PlayerInfo set [PlayerID]={0}", playerID);
        command.ExecuteNonQuery();
    }

    #endregion
    
}
