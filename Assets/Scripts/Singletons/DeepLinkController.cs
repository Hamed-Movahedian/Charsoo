using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;

public class DeepLinkController : MonoBehaviour
{
    public string Data { get; set; } = "sup&31&1074";

    [FollowMachine("Check Lunch Method", "Normal,Unkown Command,Show User Puzzle")]
    public void GetDeepLinkInfo()
    {
        if (Application.platform==RuntimePlatform.Android)
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
 
            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
 
            var action = intent.Call<string>("getAction");

            if (action.Contains("VIEW"))
            {
                Data = intent.Call<string>("getDataString");
                Data = Data.Split('/').Last();
                //"ShowPuzzle,u8217,hosein"

                CheckData();
            }
            else
            {
                FollowMachine.SetOutput("Normal");
            }
        }
        else
        {
            CheckData();
        }
    }

    private void CheckData()
    {
        var parts = Data.Split('&');

        if (parts.Length == 0)
        {
            FollowMachine.SetOutput("Unkown Command");
            return;
        }

        switch (parts[0])
        {
            case "sup":
                if (parts.Length != 3)
                {
                    FollowMachine.SetOutput("Unkown Command");
                    return;
                }

                PuzzleID = parts[1];
                UserID = parts[2];

                FollowMachine.SetOutput("Show User Puzzle");
                break;

            default:
                FollowMachine.SetOutput("Unkown Command");
                break;
        }
    }

    public string UserID { get; set; }

    public string PuzzleID { get; set; }
}
