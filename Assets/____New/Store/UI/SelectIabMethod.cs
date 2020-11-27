using System.Collections;
using System.Collections.Generic;
using BazaarPlugin;
using MgsCommonLib.UI;
using UnityEngine;

public class SelectIabMethod : MgsUIWindow
{
    public StoreMenuItemList store;
    public override void Close(string result)
    {
        if (result.ToLower().Trim().Contains("bazaar"))
            store.PurchaseWhitBazaar();
        else if (result.ToLower().Trim().Contains("zarinpal"))
            store.PurchaseWhitZarin();
        
        base.Close(result);
    }
}
