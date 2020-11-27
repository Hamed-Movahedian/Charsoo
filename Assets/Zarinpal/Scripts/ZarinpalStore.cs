using System;
using UnityEngine;
using UnityEngine.UI;

public class ZarinpalStore : MonoBehaviour
{
    public Text DebugText;

    public static event Action<string> OnPurchaseDone; 
    // Start is called before the first frame update
    void Start()
    {
        Zarinpal.StoreInitialized += () =>
        {
            if (DebugText != null) DebugText.text = ("Initialize Succeed");
        };
        Zarinpal.StoreInitializeFailed +=(s) =>
        {
            if (DebugText != null) DebugText.text = (s);
        };
        Zarinpal.Initialize();
        Zarinpal.PurchaseSucceed += (productID, authority) =>
        {
            if (DebugText != null)
                DebugText.text = ($"Product with ID {productID} purchase succeed with authority : {authority} ");
            OnPurchaseDone?.Invoke(productID);
        };
    }

    public static void Purchase(int amount,string decs,string id)
    {
        Zarinpal.Purchase(amount,decs , id) ;
    }
}
