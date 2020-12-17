using System;
using System.Collections;
using System.Linq;
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
        //LocationProvider.LocationWraper locationWraper = new LocationProvider.LocationWraper();

        //yield return LocationProvider.GetLocation(locationWraper);
        yield return null;
        DataService.Connection.Insert(new LogIn
        {
            PlayerID = playerID,
            DeviceID = SystemInfo.deviceUniqueIdentifier,
            Latitude = 0,
            Longitude = 0,
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

    #region SetLastUpdate
    public void SetLastUpdate(DateTime lastUpdate, string tableName)
    {
        var lastUpdateRecord = Table<LastTableUpdates>()
            .FirstOrDefault(l => l.TableName == tableName);

        if (lastUpdateRecord == null)
        {
            InsertOrReplace(new LastTableUpdates
            {
                TableName = tableName,
                LastUpdate = lastUpdate
            });
        }
        else
        {
            lastUpdateRecord.LastUpdate = lastUpdate;
            InsertOrReplace(lastUpdateRecord);
        }
    }
    #endregion
}
