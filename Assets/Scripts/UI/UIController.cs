using System;
using System.Collections;
using MgsCommonLib;
using MgsCommonLib.UI;
using UnityEngine;

public class UIController : MgsSingleton<UIController>
{
    [Header("Specific windows")]
    public PhoneNumberWindow PhoneNumberWindow;
    public InputCodeWindow InputCodeWindow;
    public PlayerInfoEditor PlayerInfoEditor;


    [Header("General windows")]
    public UIWindow InprogressWindow;
    public UIWindow ErrorWindow;
    public UIWindow MessageWindow;


    internal IEnumerator ShowInprogressWindow(string message)
    {
        // Set text of lable in "Message" game object
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
        // Set text of lable in "Message" game object
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
}