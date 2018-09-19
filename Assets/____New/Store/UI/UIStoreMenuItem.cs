using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using Soomla.Store;
using UnityEngine;
using UnityEngine.UI;

public class UIStoreMenuItem : UIMenuItem
{
    public Image Icon;
    public Image HasItemIcon;
    public Text ItemTitle;
    public Text PriceText;
    protected override void Refresh(object data)
    {
        _data = data;
        StoreItem storeItem = (StoreItem)_data;
        Icon.sprite = storeItem.Icon;
        ItemTitle.text = storeItem.ItemTitle;
        bool hasItem = false;

        if (storeItem.IsVirtualGood)
            hasItem = (StoreInventory.GetItemBalance(storeItem.ItemId) > 0);

        HasItemIcon.gameObject.SetActive(hasItem);
        PriceText.gameObject.SetActive(!hasItem);
        GetComponent<Button>().interactable = !hasItem;
        int price = storeItem.Price;
        PriceText.text = ArabicFixer.Fix(price > 0 ? "تومان" + price : "رایگان", true, true);
        GetComponent<RectTransform>().localScale=Vector3.one;
    }

    public override void Select()
    {
        ((StoreMenuItemList)_list).Select(_data);
    }
}
