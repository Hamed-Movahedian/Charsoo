using System.Collections;
using System.Collections.Generic;
using Soomla.Store;
using UnityEngine;

namespace Soomla.Store.Charsoo
{
    public class CharsooStoreEventHandler
    {
        public CharsooStoreEventHandler()
        {
            StoreEvents.OnMarketPurchase += OnMarketPurchase;
            StoreEvents.OnItemPurchased += OnItemPurchased;
            StoreEvents.OnCurrencyBalanceChanged += OnCurrencyBalanceChanged;
            //Singleton.Instance.DebugerText.text = StoreInventory.GetItemBalance("chasoo_currency").ToString();
#if UNITY_ANDROID && !UNITY_EDITOR
			StoreEvents.OnIabServiceStarted += onIabServiceStarted;
			StoreEvents.OnIabServiceStopped += onIabServiceStopped;
#endif
        }

        public void OnMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
        {

        }

        public void OnItemPurchased(PurchasableVirtualItem pvi, string payload)
        {
            string pviItemId = pvi.ItemId;
            if (pviItemId.Contains("_CoinsPack"))
                return;

            switch (pviItemId)
            {
                case "no_uw_ads":
                    ZPlayerPrefs.SetInt("NoAds",1);
                    break;

                case "charsoo_doubler":
                    ZPlayerPrefs.SetInt("Doubler", 1);
                    break;

            }
        }

        public void OnCurrencyBalanceChanged(VirtualCurrency virtualCurrency, int balance, int amountAdded)
        {
            if (virtualCurrency.ItemId== "charsoo_coin")
            {
                if(ZPlayerPrefs.GetInt("Coin")!=balance)
                {
                    ZPlayerPrefs.SetInt("Coin", balance);
                    Singleton.Instance.PurchaseManager.HcurrencyChanged();
                }
            }
        }

#if UNITY_ANDROID && !UNITY_EDITOR
		public void onIabServiceStarted() {

		}
		public void onIabServiceStopped() {

		}
#endif
    }
}