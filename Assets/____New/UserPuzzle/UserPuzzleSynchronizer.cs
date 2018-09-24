using System;
using System.Collections;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.Theme;
using UnityEngine;

internal class UserPuzzleSynchronizer : MonoBehaviour
{
    [FollowMachine("Sync UserPuzzles","Success,Fail,Not Registered")]
    public IEnumerator Syncing()
    {
        FollowMachine.SetOutput("Fail");

        // Cache ...
        var upLocalDB = LocalDBController.Instance.UserPuzzles;
        var upServer = ServerController.Instance.UserPuzzles;

        // If playerID==null exit!!
        int? playerID = Singleton.Instance.PlayerController.GetPlayerID;
        if (playerID == null)
        {
            FollowMachine.SetOutput("Not Registered");
            yield break;
        }

        // Get Unregisterd puzzles and lastUpdate from localDB
        var unregisteredPuzzles = upLocalDB.GetUnregisteredPuzzles();

        var lastUpdate = upLocalDB.GetLastUpdate();

        // Sync with server 
        yield return upServer.Sync(playerID.Value, unregisteredPuzzles, lastUpdate);

        if (upServer.UnsuccessfullSync)
            yield break;

        // Update localDB with updated puzzles
        upLocalDB.UpdatePuzzles(upServer.GetUpdatedPuzzles());

        // Set last update in local db
        LocalDBController.Instance.SetLastUpdate(upServer.GetLastUpdate(), "UserPuzzles");

        FollowMachine.SetOutput("Success");
    }

    [FollowMachine("Restore UserPuzzles", "Success,Fail,Not Registered")]
    public IEnumerator RestoreUserPuzzles()
    {
        // clear user puzzle table
        LocalDBController.DataService.Connection.DeleteAll<UserPuzzle>();

        // set min value for last update
        LocalDBController.Instance.SetLastUpdate(DateTime.MinValue, "UserPuzzles");

        return Syncing();
    }
}