using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class ShareScreenShot : BaseObject
{
    public Rect ScalerRect;
    //public Text Address;

    #region New Version of sharing

    private bool _isBusy = false;
    private string _fileName;
    private bool _isFocus;

    #endregion



    public struct PixleSize
    {
        public Vector2 Input;
        public int X { get { return (int)Input.x; } }
        public int Y { get { return (int)Input.y; } }
    }

    public void ShareScreenshot()
    {
        if (!_isBusy)
            StartCoroutine(CaptureScreenshot());
    }



    /*
            public IEnumerator CaptureScreenshot()
            {
                _isBusy = true;
                yield return new WaitForEndOfFrame();

                _fileName = "screenshot.png";
                ScreenCapture.CaptureScreenshot(_fileName,2);
                string destination = Path.Combine(Application.persistentDataPath, _fileName);

                yield return new WaitForSeconds(0.3f);
                Address.text = destination + "\n" + Application.persistentDataPath;



                AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
                AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
                intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
                    " بازی چارسو رو میتونی از بازار دانلود کنی:" +
                    "\n" +
                    "https://cafebazaar.ir/app/com.Matarsak.charsoo/?l=fa"
                );
                intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_SUBJECT"),
                    "برای حل این معما به کمک نیاز دارم.");
                intentObject.Call<AndroidJavaObject>("setType", "image/*");
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject,
                    "Share your problem");

                currentActivity.Call("startActivity", chooser);

                yield return new WaitForSecondsRealtime(1f);

                yield return new WaitUntil(()=>_isFocus);

                _isBusy = false;




            }

            private void OnApplicationFocus(bool focus)
            {
                _isFocus = focus;
            }


        */



    public IEnumerator CaptureScreenshot()
    {
        Debug.Log("hi");

        Vector2 screen = new Vector2(Screen.width, Screen.height);
        PixleSize picSize = new PixleSize()
        {
            Input = Vector2.Scale(ScalerRect.size / 100, screen)
        };
        Rect rectToScreenShot =
            new Rect(
                Vector2.Scale(ScalerRect.position / 100, screen),
                picSize.Input
            );

        Texture2D screenShot = new Texture2D(picSize.X, picSize.Y, TextureFormat.RGB24, true);
        screenShot.ReadPixels(rectToScreenShot, 0, 0);
        screenShot.Apply();

        byte[] dataToSave = screenShot.EncodeToJPG();
        string fileName = "Hint" + WordSpawner.Clue + ".jpg";
        string destination = Path.Combine(Application.persistentDataPath, fileName);


        if (File.Exists(destination))
            File.Delete(destination);

        while (File.Exists(destination))
            yield return new WaitForSeconds(0.1f);

        File.WriteAllBytes(destination, dataToSave);

        while (!File.Exists(destination))
            yield return new WaitForSeconds(0.1f);

        Debug.Log(destination + "\n" + Application.persistentDataPath);

        StartCoroutine(SaveAndShare(destination));

    }

    IEnumerator SaveAndShare(string destination)
    {
        yield return new WaitForEndOfFrame();
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
        AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse",  destination);
        intentObject.Call<AndroidJavaObject>(
            "putExtra",
            intentClass.GetStatic<string>("EXTRA_STREAM"),
            uriObject
            );

        Debug.Log(destination);

        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
            "برای حل این معما به کمک نیاز دارم." +
            "\n" +
            " بازی چارسو رو میتونی از بازار دانلود کنی:" +
            "\n" +
            "https://cafebazaar.ir/app/com.Matarsak.charsoo/?l=fa"
        );


        intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");

        //AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);

        /*
                AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", destination);

                bool fileExist = fileObject.Call<bool>("exists");
                if (fileExist)
                {
                    AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromFile", fileObject);
                    intentObject.Call<AndroidJavaObject>(
                        "putExtra",
                        intentClass.GetStatic<string>("EXTRA_STREAM"),
                        uriObject
                    );
                }
        */

        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        //AndroidJavaObject chooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject,"Share your problem");

        currentActivity.Call("startActivity", intentObject);
    }
}
