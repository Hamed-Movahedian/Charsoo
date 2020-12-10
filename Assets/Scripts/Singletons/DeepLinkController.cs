using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.Theme;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class DeepLinkController : MonoBehaviour
{
    public MgsDialougWindow MessageWindow;
    public string Data { get; set; }//"sup&31&1074";

    [FollowMachine(
        "Check Lunch Method",
        "Normal,Unkown Command,Show User Puzzle,Generate Puzzle,Get Invite Reward,Register Phone")]
    public void GetDeepLinkInfo()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");

            var action = intent.Call<string>("getAction");

            if (action.Contains("VIEW"))
            {
                Data = intent.Call<string>("getDataString");
                Data = Data.Split('?').Last();
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
            FollowMachine.SetOutput("Normal");
            //CheckData();
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
                    break;
                }

                PuzzleID = parts[1];
                UserID = parts[2];

                FollowMachine.SetOutput("Show User Puzzle");
                break;

            case "generatepuzzle":
                FollowMachine.SetOutput("Generate Puzzle");
                break;

            case "RegisterPhone":
                FollowMachine.SetOutput("Register Phone");
                break;

            case "getinvitereward":
                if (parts.Length != 3)
                {
                    FollowMachine.SetOutput("Unkown Command");
                    break;
                }

                InvitedUser = parts[1];
                RewartAmount = int.Parse(parts[2]);
                FollowMachine.SetOutput("Unkown Command");
                break;
                FollowMachine.SetOutput("Get Invite Reward");
                StartCoroutine(ShowInviteRewardMessage(MessageWindow));
                break;

            default:
                FollowMachine.SetOutput("Unkown Command");
                break;
        }
    }

    public IEnumerator ShowInviteRewardMessage(MgsDialougWindow window)
    {
        Debug.Log("hiiiiiiiiiiiiiiiii sjdlkjflksjdf");
        yield return null;
        yield return null;
        window.Message.text =
            window.Message.text.
            Replace("****", PersianFixer.Fix(InvitedUser)).
            Replace("***", RewartAmount.ToString("D"));
    }

    public int RewartAmount { get; set; }

    public string InvitedUser { get; set; }

    public string UserID { get; set; }

    public string PuzzleID { get; set; }
}
