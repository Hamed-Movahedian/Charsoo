#if UNITY_IOS
using System.Runtime.InteropServices;
using System;
#else
using UnityEngine;
#endif


/// <summary>
/// https://github.com/ChrisMaire/unity-native-sharing
/// </summary>
public static class NativeShare
{
    /// <summary>
    /// Shares on file maximum
    /// </summary>
    /// <param name="body"></param>
    /// <param name="filePath">The path to the attached file</param>
    /// <param name="url"></param>
    /// <param name="subject"></param>
    /// <param name="mimeType"></param>
    /// <param name="chooser"></param>
    /// <param name="chooserText"></param>
    public static void Share(
        string body,
        string filePath = null)
    {
#if UNITY_ANDROID
        ShareAndroid(body, filePath);
#elif UNITY_IOS
		ShareIOS(body, filePath);
#else
        Debug.Log("No sharing set up for this platform.");
        Debug.Log("Subject: " + subject);
        Debug.Log("Body: " + body);
#endif
    }

#if UNITY_ANDROID
    public static void ShareAndroid(string body, string filePath)
    {
        Debug.Log($"{body}\n{filePath}");
        //AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");

        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        Debug.Log("Attach Body");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), body);

        if (filePath.Length != 0)
        {
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", filePath);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
        }
        else
            intentObject.Call<AndroidJavaObject>("setType", "text/html");


        // finally start application
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);

    }

#endif

#if UNITY_IOS
	public struct ConfigStruct
	{
		public string title;
		public string message;
	}

	[DllImport ("__Internal")] private static extern void showAlertMessage(ref ConfigStruct conf);

	public struct SocialSharingStruct
	{
		public string text;
		public string subject;
		public string filePaths;
	}

	[DllImport ("__Internal")] private static extern void showSocialSharing(ref SocialSharingStruct conf);


	public static void ShareIOS(string body, string filePath)
	{
		SocialSharingStruct conf = new SocialSharingStruct();
		conf.text = body;
    
        if (filePath.Length != 0)
            conf.filePaths = filePath;

		showSocialSharing(ref conf);
	}
#endif

}
