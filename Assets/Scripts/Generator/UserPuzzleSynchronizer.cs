using System.Collections;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.Theme;
using UnityEngine;

internal class UserPuzzleSynchronizer : MonoBehaviour
{
    [FollowMachine("Sync UserPuzzles","Success,Fail")]
    public IEnumerator Syncing()
    {
        FollowMachine.SetOutput("Fail");
        // Cache ...
        var upLocalDB = LocalDBController.Instance.UserPuzzles;
        var upServer = ServerController.Instance.UserPuzzles;

        // If playerID==null exit!!
        int? playerID = Singleton.Instance.PlayerController.GetPlayerID();
        if (playerID == null)
            yield break;

        // Get Unregisterd puzzles and lastUpdate from localDB
        var unregisteredPuzzles = upLocalDB.GetUnregisteredPuzzles();
        var lastUpdate = upLocalDB.GetLastUpdate();

        // Sync with server 
        yield return upServer.Sync(playerID.Value, unregisteredPuzzles, lastUpdate);
        if (upServer.UnsuccessfullSync)
            yield break;

        // Register new puzzles
        upLocalDB.RegisterPuzzles(upServer.GetServerRegisterPuzzles());

        // Update localDB with updated puzzles
        upLocalDB.UpdatePuzzles(upServer.GetUpdatedPuzzles());

        // Set last update in local db
        upLocalDB.SetLastUpdate(upServer.GetLastUpdate());

        FollowMachine.SetOutput("Success");
    }

}