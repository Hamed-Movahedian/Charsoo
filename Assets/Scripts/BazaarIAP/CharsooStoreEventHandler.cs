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
            StoreEvents.OnGoodBalanceChanged += OnGoodPurchased;
            StoreEvents.OnCurrencyBalanceChanged += OnCurrencyBalanceChanged;
            //Singleton.Instance.DebugerText.text = StoreInventory.GetItemBalance("chasoo_currency").ToString();
#if UNITY_ANDROID && !UNITY_EDITOR
			StoreEvents.OnIabServiceStarted += onIabServiceStarted;
			StoreEvents.OnIabServiceStopped += onIabServiceStopped;
#endif
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
            if (balance==0 || amountAdded<0)
                return;
            
            Singleton.Instance.PlayerController.ChangeCoin(amountAdded);
            
            StoreInventory.TakeItem(virtualCurrency.ItemId,balance);
        }

#if UNITY_ANDROID && !UNITY_EDITOR
		public void onIabServiceStarted() {

		}
		public void onIabServiceStopped() {

		}
#endif
    }
}