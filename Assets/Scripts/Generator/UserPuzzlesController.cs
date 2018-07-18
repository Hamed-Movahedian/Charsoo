using System;
using System.Collections;
using System.Collections.Generic;
using MgsCommonLib;
using UnityEngine;

internal class UserPuzzlesController : MgsSingleton<UserPuzzlesController>
{
    public void Save()
    {
        WordSet wordSet = new WordSet();

        wordSet.Clue = UIController.Instance.Generator.GetClue();
        wordSet.Words = new List<SWord>();

        WordManager wordManager = Singleton.Instance.WordManager;

        foreach (var word in wordManager.GetComponentsInChildren<Word>())
            wordSet.Words.Add(new SWord(word));

        var puzzle = new Puzzle
        {
            ID = 1,
            CategoryID = null,
            Clue = wordSet.Clue,
            Row = 1,
            Content = StringCompressor.CompressString(JsonUtility.ToJson(wordSet)),
            Solved = false,
            Paid = false,
            LastUpdate = DateTime.Now
        };
        //LocalDBController.Table<Pu>()
    }

    public IEnumerator Sync()
    {
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
        if(upServer.UnsuccessfullSync)
            yield break;

        // Register new puzzles
        upLocalDB.RegisterPuzzles(upServer.GetServerRegisterPuzzles());


    }
}