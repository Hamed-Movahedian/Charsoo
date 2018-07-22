using UnityEngine;
using UnityEngine.Events;

public abstract class UIMenuItem : MonoBehaviour
{
    public UnityEvent OnSelected;
    public UnityEvent OnDeselected;

    private object _data;
    private UIMenuItemList _list;

    public object Data
    {
        get { return _data; }
    }

    public void UpdateItems(object data, UIMenuItemList menuItemList)
    {
        _data = data;
        _list = menuItemList;

        Refresh(data);
    }

    protected abstract void Refresh(object data);


    public void Select()
    {
        _list.Select(this);
    }
}