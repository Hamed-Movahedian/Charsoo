using System;
using System.Collections;
using MgsCommonLib;
using MgsCommonLib.UI;
using UnityEngine;

public class UIController : MgsSingleton<UIController>
{
    [Header("Specific windows")]
    public PhoneNumberWindow PhoneNumberWindow;
    public InputCodeWindow inputCodeWindow;

    [Header("General windows")]
    public UIWindow InprogressWindow;
    public UIWindow ErrorWindow;


    internal IEnumerator ShowInprogressWindow(string message)
    {
        InprogressWindow.SetTextMessage(message);

        yield return InprogressWindow.Show();
    }

    internal IEnumerator HideInprogressWindow()
    {
        yield return InprogressWindow.Hide();
    }

    internal void DisplayError(string v)
    {
        throw new NotImplementedException();
    }
}