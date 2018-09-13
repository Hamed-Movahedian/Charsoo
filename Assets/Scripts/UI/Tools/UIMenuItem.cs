using UnityEngine;
using UnityEngine.Events;

public abstract class UIMenuItem : MonoBehaviour
{
    public UnityEvent OnSelected;
    public UnityEvent OnDeselected;

    internal object _data;
    internal UIMenuItemList _list;

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


    public virtual void Select()
    {
        _list.Select(this);
    }
}