using System;
using System.Collections;
using System.Collections.Generic;
using BazaarPlugin;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class BazaarIntents : MonoBehaviour
{
    public void Like()
    {
        string PackageName = "com.Matarsak.charsoo";
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_EDIT"));
        intentObject.Call<AndroidJavaObject>("setData",
            uriClass.CallStatic<AndroidJavaObject>("parse", "bazaar://details?id=" + PackageName));
        intentObject.Call<AndroidJavaObject>("setPackage", "com.farsitel.bazaar");

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);

        PlayerPrefs.SetInt("Liked", 1);
    }

    [FollowMachine("Should Like", "Yes,No")]
    public void ShouldLike()
    {
        if (PlayerPrefs.GetInt("Liked", 0) == 0 &&
            LocalDBController.Table<PlayPuzzles>().Count() > 20 &&
            Random.Range(0.1f,1f)>0.5f &&
            GetComponent<CheckMarket>().BazzarMarket)
        {
            FollowMachine.SetOutput("Yes");
            return;
        }

        FollowMachine.SetOutput("No");
    }
}