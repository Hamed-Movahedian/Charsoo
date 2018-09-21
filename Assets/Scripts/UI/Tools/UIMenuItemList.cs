using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuItemList : MgsUIWindow
{
    public UIMenuItem Prefab;
    public RectTransform ContentParent;

    private readonly List<UIMenuItem> _menuItems=new List<UIMenuItem>();
    private UIMenuItem _selectedItem;
    private float _lastSelectionTime;
    private object _selectedData;

    public void UpdateItems(IEnumerable<object> data)
    {
        _selectedItem = null;
        var dataList = data.ToList();

        for (int i = 0; i < dataList.Count; i++)
        {
            // not enough items - create new one
            if (i >= _menuItems.Count)
                // get new item from pool manager and set its parent
                _menuItems.Add((UIMenuItem) PoolManager.Instance.Get(Prefab, ContentParent));

            // Update item
            _menuItems[i].UpdateItems(dataList[i],this);
        }

        // return extra items to pool manager
        while (dataList.Count<_menuItems.Count)
        {
            var lastItem = _menuItems[_menuItems.Count-1];
            _menuItems.Remove(lastItem);
            PoolManager.Instance.Return(lastItem);
        }

        ListContentSizeSetter setter = ContentParent.GetComponent<ListContentSizeSetter>();

        if (setter != null) setter.SetupSize(dataList.Count,Prefab);
    }

    public virtual void Select([CanBeNull] object data)
    {
        _selectedData = data;
        Close("Select");

    }


    public void Select(UIMenuItem uiMenuItem)
    {
        Select(uiMenuItem.Data);
        return;
        if (_selectedItem == uiMenuItem)
            if (Time.time - _lastSelectionTime < .5f)
                Close("ItemSelected");

        _lastSelectionTime = Time.time;

        if (_selectedItem == uiMenuItem)
            return;

        if (_selectedItem != null)
            _selectedItem.OnDeselected.Invoke();

        _selectedItem = uiMenuItem;
        _selectedItem.OnSelected.Invoke();
        Close("Select");
    }


    public object GetSelectedItem()
    {
        return _selectedData;
        return _selectedItem != null ? _selectedItem.Data : null;
    }

}