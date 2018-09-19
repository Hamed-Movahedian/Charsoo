using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreMenuItemList : UIMenuItemList
{
    [System.Serializable]
    public class StoreItem
    {
        public string ItemTitle;
        public int Price;
        public Sprite Icon;
    }

    public List<StoreItem> StoreItems;

    void Update()
    {

    }
}
