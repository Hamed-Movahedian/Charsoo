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
            StoreEvents.OnGoodUpgrade += GoodUpgrade;
            StoreEvents.OnGoodEquipped += GoodEq;
            StoreEvents.OnGoodBalanceChanged += OnGoodPurchased;
            StoreEvents.OnGoodUnEquipped += OnGoodUnEq;
            StoreEvents.OnCurrencyBalanceChanged += OnCurrencyBalanceChanged;
            StoreEvents.OnItemPurchased += OnItem;
            //Singleton.Instance.DebugerText.text = StoreInventory.GetItemBalance("chasoo_currency").ToString();
#if UNITY_ANDROID && !UNITY_EDITOR
			StoreEvents.OnIabServiceStarted += onIabServiceStarted;
			StoreEvents.OnIabServiceStopped += onIabServiceStopped;
#endif
        }

        private void OnItem(PurchasableVirtualItem arg1, string arg2)
        {
            Debug.Log(arg1);
        }

        private void OnGoodUnEq(EquippableVG obj)
        {
            Debug.Log(obj);
        }

        private void GoodEq(EquippableVG obj)
        {
            Debug.Log(obj);
        }

        private void GoodUpgrade(VirtualGood arg1, UpgradeVG arg2)
        {

            Debug.Log(arg1);
        }

        public void OnMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
        {
            Debug.Log(pvi);
        }

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
            if (virtualCurrency.ItemId == "charsoo_coin")
            {
                var playerInfo = Singleton.Instance.PlayerController.PlayerInfo;
                int coinCount = playerInfo.CoinCount;
                if (coinCount != balance)
                {
                    playerInfo.CoinCount = balance;
                    playerInfo.Dirty = true;
                    Singleton.Instance.PlayerController.ChangePlayerInfo(playerInfo);
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