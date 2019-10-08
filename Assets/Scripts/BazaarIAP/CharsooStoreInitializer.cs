using System.Collections;
using System.Collections.Generic;
using BazaarPlugin;
using UnityEngine;

public class CharsooStoreInitializer : MonoBehaviour
{
    #region Key

    private string key =
        "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwDIFHbmF19C0fpkum4/dnvmMIVIJKcbOwH2LRDDYmPedEsnM5aPRW00kqCpBUAVAsnlmXHNCXSb/Jv7ux5n9d4K/4PY0fI+37RgYHhNki+Rz0d4XiuUiG3Rdlo6XV9paVcCUDiP1OUVtw7IEf0imXWzjIGuAkvL2oCWMhZqDBwZQXoOwWmvb18SMYxZcvIuz72RxbUdzDKzN0ug8ZFatFcIXV2MSjSqNhLKzR9un78CAwEAAQ==";

    #endregion

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        BazaarIAB.init(key);
        IABEventManager.purchaseSucceededEvent += OnPurchased;
    }

    private void OnPurchased(BazaarPurchase obj)
    {
        if (obj.ProductId.Trim() == "charsoo_doubler")
        {
            var playerController = Singleton.Instance.PlayerController;
            PlayerInfo playerInfo = playerController.PlayerInfo;
            playerInfo.HasDubler = true;
            playerInfo.Dirty = true;
            Singleton.Instance.PlayerController.ChangePlayerInfo(playerInfo);
        }

        if (obj.ProductId.Contains("coin"))
        {
            string id = obj.ProductId.Replace("coin", "").Trim();
            Debug.Log(id);
            id = id.Replace("coin", "").Trim();
            Debug.Log(id);

            int count = int.Parse(id);
            Singleton.Instance.PlayerController.ChangeCoin(count);
        }

        BazaarIAB.consumeProduct(obj.ProductId);
    }


/* OLD Bazaar IAB

    

    public void OnGoodPurchased(VirtualGood virtualGood, int balance, int amountAdded)
    {
        string pviItemId = virtualGood.ItemId;
        if (pviItemId.Contains("_CoinsPack"))
            return;

        if (pviItemId == "charsoo_doubler")
        {
            var playerController = Singleton.Instance.PlayerController;
            PlayerInfo playerInfo = playerController.PlayerInfo;
            playerInfo.HasDubler = true;
            playerInfo.Dirty = true;
            Singleton.Instance.PlayerController.ChangePlayerInfo(playerInfo);
        }
    }

    public void OnCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded)
    {
        if (balance == 0 || amountAdded < 0)
            return;

        Singleton.Instance.PlayerController.ChangeCoin(amountAdded);

        StoreInventory.TakeItem(virtualCurrency.ItemId, balance);
    }
    
    */


}