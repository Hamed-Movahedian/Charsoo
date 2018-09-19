﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using UnityEngine;

public class StoreMenuItemList : UIMenuItemList
{
    public PurchaseManager PurchaseManager;
    public List<StoreItem> StoreItems;

    public override void Refresh()
    {

        UpdateItems(StoreItems.Cast<object>());
    }

    public override void Select(object data)
    {
        StoreItem sItem = (StoreItem) data;
        if (sItem != null) PurchaseManager.BuyItem(sItem.ItemId);

        Close("Purchased");
    }
}
[System.Serializable]
public class StoreItem
{
    public bool IsVirtualGood;
    public string ItemId;
    public string ItemTitle;
    public int Price;
    public Sprite Icon;
}

