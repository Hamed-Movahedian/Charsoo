using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BazaarIntents : MonoBehaviour
{

    public void Like()
    {
        int liked = ZPlayerPrefs.GetInt("Liked", 0);

        if (liked>0)
            if (LocalDBController.Table<PlayPuzzles>().Select(pp => pp.Success).Count() > 15)
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
                
            }
    }

}
