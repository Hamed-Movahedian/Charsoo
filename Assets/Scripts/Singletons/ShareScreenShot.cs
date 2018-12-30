using UnityEngine;
using System.Collections;
using System.IO;
using FMachine;
using FollowMachineDll.Attributes;

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

        public int X
        {
            get { return (int)Input.x; }
        }

        public int Y
        {
            get { return (int)Input.y; }
        }
    }

    public void ShareScreenshot()
    {
        if (Application.isEditor)
            return;
        if (!_isBusy)
            StartCoroutine(ShareHintWondow());
    }

    public IEnumerator ShareHintWondow()
    {
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
        int random = Random.Range(1, 55465463);
        byte[] dataToSave = screenShot.EncodeToJPG();
        string fileName = "Hint" + WordSpawner.Clue + random + ".jpg";
        string destination = Path.Combine(Application.persistentDataPath, fileName);


        if (File.Exists(destination))
            File.Delete(destination);

        while (File.Exists(destination))
            yield return new WaitForSeconds(0.1f);

        File.WriteAllBytes(destination, dataToSave);

        while (!File.Exists(destination))
            yield return new WaitForSeconds(0.1f);

        Debug.Log(destination + "\n" + Application.persistentDataPath);

        string body = "برای حل این معما به کمک نیاز دارم." +
              "\n" +
              " بازی چارسو رو میتونی از بازار دانلود کنی:" +
              "\n" +
              "https://cafebazaar.ir/app/com.Matarsak.charsoo/?l=fa";

        NativeShare.Share(body, destination);
    }

    [FollowMachine("Share Puzzle", "NotOnline,Success")]
    public void SharePuzzle(int? puzzleID)
    {
        if (!puzzleID.HasValue|| Application.isEditor)
        {
            FollowMachine.SetOutput("NotOnline");
            return;
        }
        StartCoroutine(ShareUserPuzzle(puzzleID.Value));
    }

    public IEnumerator ShareUserPuzzle(int puzzleID)
    {
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
        int random = Random.Range(1, 55465463);
        byte[] dataToSave = screenShot.EncodeToJPG();
        string fileName = "sharedpuzzle "+WordSpawner.Clue + random + ".jpg";
        string destination = Path.Combine(Application.persistentDataPath, fileName);


        if (File.Exists(destination))
            File.Delete(destination);

        while (File.Exists(destination))
            yield return new WaitForSeconds(0.1f);

        File.WriteAllBytes(destination, dataToSave);

        while (!File.Exists(destination))
            yield return new WaitForSeconds(0.1f);

        Debug.Log(destination + "\n" + Application.persistentDataPath);

        string body = "حدس میزنم از حل کردن این جدول لذت ببری.\n خودم این جدول رو ساختم. امتحانش کن." +
                   "\n" +
                   "http://charsoogame.ir/inapp.html?sup&" + puzzleID + "&" + PlayerController.PlayerID ;

        NativeShare.Share(body, destination);
        FollowMachine.SetOutput("Success");

    }


    [FollowMachine("Is Puzzle Online?", "No,Yes")]
    public void CheckPuzzleID(int? puzzleID)
    {
        FollowMachine.SetOutput(puzzleID.HasValue ? "Yes" : "No");
    }

}