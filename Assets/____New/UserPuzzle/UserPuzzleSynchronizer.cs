using System;
using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using MgsCommonLib.Theme;
using UnityEngine;

internal class UserPuzzleSynchronizer : MgsSingleton<UserPuzzleSynchronizer>
{
    [FollowMachine("Sync UserPuzzles", "Success,Fail,Not Registered")]
    public IEnumerator Syncing()
    {
        FollowMachine.SetOutput("Fail");

        // Cache ...
        var upLocalDB = LocalDBController.Instance.UserPuzzles;
        var upServer = ((ServerController) ServerController.Instance).UserPuzzles;

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

    public void RestoreUserPuzzles(List<UserPuzzle> userPuzzles)
    {
        // clear user puzzle table
        LocalDBController.DataService.Connection.DeleteAll<UserPuzzle>();

        foreach (UserPuzzle puzzle in userPuzzles)
            LocalDBController.InsertOrReplace(puzzle);
    }

    #region PuzzleCount

    public int PuzzleCount
    {
        get
        {
            var userPuzzles = LocalDBController
                .Table<UserPuzzle>();
            if (userPuzzles == null)
                return 0;

            return userPuzzles.Count();
        }
    }

    public string PuzzleCountText => PuzzleCount.ToString();


    #endregion

}