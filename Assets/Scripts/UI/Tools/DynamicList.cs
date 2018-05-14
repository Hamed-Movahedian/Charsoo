using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicList : MonoBehaviour
{
    #region Public

    public RectTransform CacheMenuItems;

    public VerticalLayoutGroup ContentLayout;

    #endregion

    #region Private

    #endregion

    #region Start
    void Start()
    {
    }
    #endregion

    #region Clear

    public void Clear()
    {
        while (ContentLayout.transform.childCount > 0)
        {
            ContentLayout.transform.GetChild(0).GetComponent<RectTransform>()
                .SetParent(CacheMenuItems);
        }

    }
    #endregion

    #region GetFreeItem

    public T GetFreeItem<T>() where T : MonoBehaviour 
    {
        var components = CacheMenuItems.GetComponentsInChildren<T>();

        if (components.Length > 1)
            return components[0];

        if (components.Length == 0)
        {
            Debug.LogError("Menu Item "+typeof(T).Name+" not found!!");
            return null;
        }

        return Instantiate(components[0].gameObject,CacheMenuItems).GetComponent<T>();
    }
    #endregion

    #region Add

    public void Add(RectTransform item)
    {
        // set parent
        item.SetParent(ContentLayout.transform);
    }


    #endregion

    #region End
    
    public void End()
    {
        Vector2 contentSize = ContentLayout.GetComponent<RectTransform>().sizeDelta;

        contentSize.y = ContentLayout.padding.top + ContentLayout.padding.bottom;

        foreach (RectTransform item in ContentLayout.transform)
            contentSize.y += item.sizeDelta.y + ContentLayout.spacing;

        ContentLayout.GetComponent<RectTransform>().sizeDelta = contentSize;
    }
    #endregion

    #region GetActiveItems

    public T[] GetActiveItems<T>()
    {
        return ContentLayout.GetComponentsInChildren<T>();
    }
    

    #endregion
}
