using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using UnityEngine;

public class SetButtonSize : MonoBehaviour
{
    public RectTransform ButtonsParrent;
    public RectTransform Message;

    private MgsDialougWindow _window;

    public MgsDialougWindow Window
    {
        get
        {
            if (_window == null)
                _window = transform.parent.GetComponent<MgsDialougWindow>();
            return _window;
        } 
    }

    public void OnEnable()
    {
/*      
        Vector2 sizeDelta = ButtonsParrent.sizeDelta;
        sizeDelta.y = Window.Buttons.Count*155 + 15;
        ButtonsParrent.sizeDelta= sizeDelta;
        Vector2 messageSizeDelta = Message.sizeDelta;
        messageSizeDelta.y = _window.GetComponent<RectTransform>().sizeDelta.y - sizeDelta.y - 15;
        Message.sizeDelta = messageSizeDelta;*/


        Rect messageRect = Message.rect;
        Rect buttonsParrentRect = ButtonsParrent.rect;

        int buttonsCount =0;

        foreach (Transform b in ButtonsParrent.transform)
        {
            if (b.gameObject.activeSelf)
                buttonsCount++;
        }



        buttonsParrentRect.height = buttonsCount*155 + 15;
        messageRect.height = 975 - buttonsParrentRect.height;

        Message.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, messageRect.height);
        ButtonsParrent.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, buttonsParrentRect.height);
    }


}
