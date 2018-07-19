using System.Collections;
using MgsCommonLib;
using MgsCommonLib.UI;
using UnityEngine;

public class UIController : MgsSingleton<UIController>
{
    public GeneratorUI Generator;
    public UserPuzzleUI UserPuzzles;

    [Header("Specific windows")]
    public PhoneNumberWindow PhoneNumberWindow;
    public InputCodeWindow InputCodeWindow;
    public PlayerInfoEditor PlayerInfoEditor;


    [Header("General windows")]
    public MgsUIWindow InprogressWindow;
    public MgsUIWindow ProgressbarWindow;
    public MgsUIWindow ErrorWindow;
    public MgsUIWindow MessageWindow;


    internal IEnumerator ShowInprogressWindow(string message)
    {
        // Set text of label in "Message" game object
        InprogressWindow.SetTextMessage(message);

        // Show in-progress window
        yield return InprogressWindow.Show();
    }

    internal IEnumerator HideInprogressWindow()
    {
        yield return InprogressWindow.Hide();
    }

    internal IEnumerator DisplayError(string message, Sprite icon)
    {
        // Set text of label in "Message" game object
        ErrorWindow.SetTextMessage(message);

        // Set sprite of Image in "Icon" game object
        ErrorWindow.SetIcon(icon);

        // Show in-progress window => wait for close => hide
        yield return ErrorWindow.ShowWaitForCloseHide();
    }

    public IEnumerator DisplayMessage(string message)
    {
        // Set text of label in "Message" game object
        MessageWindow.SetTextMessage(message);

        // Show in-progress window => wait for close => hide
        yield return MessageWindow.ShowWaitForCloseHide();

    }

    public IEnumerator ShowProgressbarWindow(string message)
    {
        // Set text of label in "Message" game object
        ProgressbarWindow.SetTextMessage(message);

        // Show in-progress window
        yield return ProgressbarWindow.Show();
    }
    internal IEnumerator HideProgressbarWindow()
    {
        yield return ProgressbarWindow.Hide();
    }

    public bool SetProgressbar(float percentage, string info)
    {
        ProgressbarWindow.SetFillAmount(percentage);
        ProgressbarWindow.SetTextMessage(info);
        return true;
    }
}