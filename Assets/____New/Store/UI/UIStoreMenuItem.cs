using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
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
            hasItem = Singleton.Instance.PlayerController.PlayerInfo?.HasDubler??false;

        HasItemIcon.gameObject.SetActive(hasItem);
        PriceText.gameObject.SetActive(!hasItem);
        GetComponent<Button>().interactable = !hasItem;
        int price = storeItem.Price;
        PriceText.text = PersianFixer.Fix(price > 0 ? " تومان " + price : "رایگان", true, true);
        GetComponent<RectTransform>().localScale=Vector3.one;
    }

    public override void Select()
    {
        Debug.Log("selected");
        ((StoreMenuItemList)_list).Select(_data);
    }
}
