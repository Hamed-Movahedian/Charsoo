using System.Collections;
using System.Collections.Generic;
using BazaarPlugin;
using UnityEngine;

public class CharsooStoreInitializer : MonoBehaviour
{
    #region Key

    private string key =
        "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwDIFHbmF19C0fpkum4/dnvmMIVIJKcbOwH2LRDDYmPedEsnM5aPRW00kqCpBUAVAsnlmXHNCXSb/Jv7ux5n9d4K/4PY0fI+37RgYHhNki+Rz0d4XiuUiG3Rdlo6XV9paVcCUDiP1OUVtw7IEf0imXWzjIGuAkvL2oCWMhZqDBwZQXoOwWmvb18SMYxZcvIuz72RxbUdzDKzN0ug8ZFatFcIXV2MSjSqNhLKzR9un78CAwEAAQ==";

    private string[] skues = new string[] { };

    #endregion

    public static bool IsBazaarSuported = true;
    public bool support = true;

    private void Start()
    {
        IABEventManager.purchaseSucceededEvent += OnPurchased;
        IABEventManager.billingNotSupportedEvent += notAvalaible;
        IABEventManager.billingSupportedEvent += Support;
        ZarinpalStore.OnPurchaseDone += s => { OnPurchase(s,false); };

        DontDestroyOnLoad(gameObject);
        BazaarIAB.init(key);
        BazaarIAB.enableLogging(true);
    }

    private void Support()
    {
        Debug.Log("Init Done");
        BazaarIAB.queryInventory(skues);
        Debug.Log("Skus:\n" + skues);
    }

    private void notAvalaible(string obj)
    {
        IsBazaarSuported = false;
        support = false;
        Debug.Log("Init Error");
    }

    private void OnPurchased(BazaarPurchase obj)
    {
        string productId = obj.ProductId;
        OnPurchase(productId, true);
    }

    private static void OnPurchase(string productId, bool bazaar)
    {
        string s = productId.ToLower();
        if (s.Trim().Contains("doubler"))
        {
            var playerController = Singleton.Instance.PlayerController;
            PlayerInfo playerInfo = playerController.PlayerInfo;
            playerInfo.HasDubler = true;
            playerInfo.Dirty = true;
            Singleton.Instance.PlayerController.ChangePlayerInfo(playerInfo);
        }

        if (s.ToLower().Contains("coin"))
        {
            string id = s.Replace("coin", "").Trim();
            Debug.Log(id);
            id = id.Replace("coin", "").Trim();
            Debug.Log(id);

            int count = int.Parse(id);
            Singleton.Instance.PlayerController.ChangeCoin(count);
        }

        if (bazaar)
            BazaarIAB.consumeProduct(s);
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