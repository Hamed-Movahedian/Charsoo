﻿using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class OnlinePuzzleController : MgsSingleton<OnlinePuzzleController>
{
    public JObject ServerRespond { get; set; }

    [FollowMachine("Prepare online puzzle for spawn","Success,Fail")]
    public IEnumerator SetForSpawn(int ID)
    {
        UserPuzzle selectedPuzzle=null;

        // Ask command center to connect to account
         yield return ServerController
            .Get<UserPuzzle>($@"UserPuzzles/{ID}",
                  puzzle => { selectedPuzzle = (UserPuzzle) puzzle; } );

        if (selectedPuzzle == null)
        {
            FollowMachine.SetOutput("Fail");
            yield break;
        }

        var json = StringCompressor.DecompressString(selectedPuzzle.Content);

        WordSet wordSet = new WordSet();

        JsonUtility.FromJsonOverwrite(json, wordSet);

        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.Clue = selectedPuzzle.Clue;
        Singleton.Instance.WordSpawner.PuzzleID = -1;
        Singleton.Instance.WordSpawner.PuzzleRow = "";

        Singleton.Instance.WordSpawner.EditorInstatiate = null;
        FollowMachine.SetOutput("Success");

    }



    [FollowMachine("Prepare online puzzle for spawn", "Success,Fail")]
    public IEnumerator SetForSpawn(string ID)
    {
        yield return SetForSpawn(int.Parse(ID));
    }


    [FollowMachine("Get invited user puzzle from server", "Success,Network Error,Fail")]
    public IEnumerator GetUserPuzzle(string puzzleID, string senderID)
    {
        ServerRespond=null;

        yield return ServerController
            .Post<string>($@"UserPuzzles/GetInviteData?puzzleID={puzzleID}&senderID={senderID}",
                null,
                r =>
                {
                    ServerRespond =JObject.Parse(r);
                    FollowMachine.SetOutput("Success");
                },
                request =>
                {
                    if(request.isNetworkError)
                        FollowMachine.SetOutput("Network Error");
                    else
                        FollowMachine.SetOutput("Fail");
                }
                );

        
    }

}
