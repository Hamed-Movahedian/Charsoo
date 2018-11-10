using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class OnlinePuzzleController : MgsSingleton<OnlinePuzzleController>
{
    public JObject ServerRespond { get; set; }
    private int _pID = -1;
    [FollowMachine("Prepare online puzzle for spawn", "Success,Fail")]
    public IEnumerator SetForSpawn(int ID)
    {
        UserPuzzle selectedPuzzle = null;
        _pID = ID;
        // Ask command center to connect to account
        /*
                yield return ServerController
                   .Get<UserPuzzle>($@"UserPuzzles/{ID}",
                         puzzle => { selectedPuzzle = (UserPuzzle)puzzle; });
        */
        ServerRespond = null;
        yield return ServerController
            .Post<string>($@"UserPuzzles/GetInviteData?puzzleID={ID}&senderID={Singleton.Instance.PlayerController.PlayerID}",
                null,
                r =>
                {
                    ServerRespond = JObject.Parse(r);
                },
                request =>
                {
                    FollowMachine.SetOutput("Fail");
                }
                );

        if (ServerRespond == null)
        {
            FollowMachine.SetOutput("Fail");
            yield break;
        }

        var json = StringCompressor.DecompressString(ServerRespond["Content"].ToString());

        WordSet wordSet = new WordSet();

        JsonUtility.FromJsonOverwrite(json, wordSet);

        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.Clue = ServerRespond["Clue"].ToString();
        Singleton.Instance.WordSpawner.PuzzleID = -1;
        Singleton.Instance.WordSpawner.PuzzleRow = ServerRespond["Creator"].ToString();

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
        ServerRespond = null;

        yield return ServerController
            .Post<string>($@"UserPuzzles/GetInviteData?puzzleID={puzzleID}&senderID={senderID}",
                null,
                r =>
                {
                    ServerRespond = JObject.Parse(r);
                    FollowMachine.SetOutput("Success");
                },
                request =>
                {
                    if (request.isNetworkError)
                        FollowMachine.SetOutput("Network Error");
                    else
                        FollowMachine.SetOutput("Fail");
                }
                );


    }

    public void SpawnInvitedPuzzle()
    {
        var json = StringCompressor.DecompressString(ServerRespond["Content"].ToString());

        WordSet wordSet = new WordSet();

        JsonUtility.FromJsonOverwrite(json, wordSet);

        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.Clue = ServerRespond["Clue"].ToString();
        Singleton.Instance.WordSpawner.PuzzleID = -1;
        Singleton.Instance.WordSpawner.PuzzleRow = ServerRespond["Creator"].ToString();

        Singleton.Instance.WordSpawner.EditorInstatiate = null;
        FollowMachine.SetOutput("Success");
    }

    [FollowMachine("Send FeedBack To Server", "Success,Network Error,Fail")]
    public IEnumerator FeedBack(float star)
    {
        string s = ((int)star).ToString();
        int? id = Singleton.Instance.PlayerController.GetPlayerID;
        if (!id.HasValue)
        {
            FollowMachine.SetOutput("Fail");
            yield break;
        }

        string trim = ServerRespond["Creator"].ToString().Trim();
        if (LocalDBController.Table<PlayerInfo>().FirstOrDefault().Name.Trim()==trim)
        {
            FollowMachine.SetOutput("Fail");
            yield break;
        }

        yield return ServerController
    .Post<string>($@"PuzzleRates/RegisterFeedback?puzzleID={_pID}&playerID={id}&star={s}",
        null,
        r => { FollowMachine.SetOutput("Success"); },
        request => { FollowMachine.SetOutput(request.isNetworkError ? "Network Error" : "Fail"); }
        );




    }

}
