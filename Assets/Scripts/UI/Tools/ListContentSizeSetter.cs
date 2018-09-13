using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ListContentSizeSetter : MonoBehaviour
{
    private VerticalLayoutGroup _layoutGroup;
    //private RectTransform _rectTransform;

    public void SetupSize(int itemCount, UIMenuItem prefab)
    {
        _layoutGroup = GetComponent<VerticalLayoutGroup>();

        float itemSize = prefab.GetComponent<RectTransform>().rect.height;
        float contentHeight = itemCount*(itemSize + _layoutGroup.spacing) + _layoutGroup.padding.top +
                              _layoutGroup.padding.bottom;

        Vector2 size = GetComponent<RectTransform>().sizeDelta;
        size.y= contentHeight;
        Debug.Log(size);
        GetComponent<RectTransform>().sizeDelta=size;
    }

}
