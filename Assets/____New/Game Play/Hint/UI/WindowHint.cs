using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class WindowHint : MgsUIWindow
{
    public Button ShowStartOfWordButton;

    public override void Refresh()
    {
        ShowStartOfWordButton.interactable = AdManager.Instance.IsAvailable;
    }
}
