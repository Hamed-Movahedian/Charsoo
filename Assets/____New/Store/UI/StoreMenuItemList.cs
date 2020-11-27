using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using BazaarPlugin;
using UnityEngine;

public class StoreMenuItemList : UIMenuItemList
{
    public List<StoreItem> StoreItems;
    private StoreItem _sItem;

    public override void Refresh()
    {

        UpdateItems(StoreItems.Cast<object>());
    }

    public override void Select(object data)
    {
        _sItem = (StoreItem) data;
        if (_sItem != null)
        {
            if(CharsooStoreInitializer.IsBazaarSuported)
            {
                Close("Select IAB Method");
                return;
            }
            ZarinpalStore.Purchase(_sItem.Price,_sItem.ItemDecs,_sItem.ItemId);
        }

        Close("Purchased");
    }

    public void PurchaseWhitBazaar()
    {
        BazaarIAB.purchaseProduct(_sItem.ItemId);
        Close("Purchased");
    }
    public void PurchaseWhitZarin()
    {
        ZarinpalStore.Purchase(_sItem.Price,_sItem.ItemDecs,_sItem.ItemId);
        Close("Purchased");
    }
}


[System.Serializable]
public class StoreItem
{
    public bool IsVirtualGood;
    public string ItemId;
    public string ItemTitle;
    public string ItemDecs;
    public int Price;
    public Sprite Icon;
}

