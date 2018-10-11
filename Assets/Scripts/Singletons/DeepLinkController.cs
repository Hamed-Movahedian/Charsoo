using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepLinkController : MonoBehaviour
{
    public string Data { get; set; }
    public string Action { get; set; }

    public void GetDeepLinkInfo()
    {
        if (Application.platform==RuntimePlatform.Android)
        {
            AndroidJavaClass UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            if (currentActivity == null)
            {
                Data = "Error : currentActivity = null";
                return;
            }

            AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("getIntent");
            if (intent == null)
            {
                Data = "Error : intent = null";
                return;
            }

            //Action = intent.Call<string>("getAction");
            Data = intent.Call<string>("getDataString"); 
        }
    }
}
