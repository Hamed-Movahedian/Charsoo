using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Soomla.Store.Charsoo
{
    public class CharsooStoreAsset : IStoreAssets
    {
        public int GetVersion()
        {
            return 1;
        }

        public VirtualCurrency[] GetCurrencies()
        {
            return new VirtualCurrency[] { CHARSOO_CURRENCY };
        }

        public VirtualGood[] GetGoods()
        {
            return new VirtualGood[] { CHARSOO_DOUBLER, NO_ADS_LTVG };
        }

        public VirtualCurrencyPack[] GetCurrencyPacks()
        {
            return new VirtualCurrencyPack[]
            {
                FFTYCoin_PACK,HUNDFFTYCoin_PACK, FIVHUNDCoin_PACK, THOUSANDCoin_PACK, THRTHOUSNDCoin_PACK, TENTHOUSNDCoin_PACK
            };
        }

        public VirtualCategory[] GetCategories()
        {
            return new VirtualCategory[] { };
        }

        #region Static Final Members

        public const string CHARSOO_CURRENCY_ITEM_ID = "charsoo_coin";      //ItemID in BAZAAR    شناسهٔ کالا

        public const string FFTYCoin_PACK_PRODUCT_ID = "50coin";            //ItemID in BAZAAR    شناسهٔ کالا

        public const string HUNDFFTYCoin_PACK_PRODUCT_ID = "150coin";            //ItemID in BAZAAR    شناسهٔ کالا

        public const string FIVHUNDCoin_PACK_PRODUCT_ID = "500coin";        //ItemID in BAZAAR    شناسهٔ کالا

        public const string THOUSANDCoin_PACK_PRODUCT_ID = "1000coin";      //ItemID in BAZAAR    شناسهٔ کالا

        public const string THRTHOUSNDCoin_PACK_PRODUCT_ID = "3000coin";    //ItemID in BAZAAR    شناسهٔ کالا

        public const string TENTHOUSNDCoin_PACK_PRODUCT_ID = "10000coin";   //ItemID in BAZAAR    شناسهٔ کالا

        public const string CHARSOO_DOUBLER_LIFETIME_ID = "doubler";        //ItemID in BAZAAR    شناسهٔ کالا

        public const string NO_ADS_LIFETIME_PRODUCT_ID = "no_ads";          //ItemID in BAZAAR    شناسهٔ کالا

        #endregion

        #region Define Virtual Currency

        public static VirtualCurrency CHARSOO_CURRENCY = new VirtualCurrency(
            "coin",                                         // name
            "",                                             // description
            CHARSOO_CURRENCY_ITEM_ID                         // item id
        );

        #endregion

        #region Virtual Currency Packs

        //50 Coin Pack     0tuman           ************ TEST *************
        public static VirtualCurrencyPack FFTYCoin_PACK = new VirtualCurrencyPack(
            "50 coin",                                                  // name
            "Buy 50 coins",                                             // description
            "50_CoinsPack",                                             // item id
            50,                                                         // number of currencies in the pack
            CHARSOO_CURRENCY_ITEM_ID,                                   // the currency associated with this pack
            new PurchaseWithMarket(FFTYCoin_PACK_PRODUCT_ID, 0)
        );

        //150 Coin Pack     1000tuman           ************ TEST *************
        public static VirtualCurrencyPack HUNDFFTYCoin_PACK = new VirtualCurrencyPack(
            "150 coin",                                                  // name
            "Buy 150 coins",                                             // description
            "150_CoinsPack",                                             // item id
            150,                                                         // number of currencies in the pack
            CHARSOO_CURRENCY_ITEM_ID,                                   // the currency associated with this pack
            new PurchaseWithMarket(HUNDFFTYCoin_PACK_PRODUCT_ID, 980)
            );

        //500 Coin Pack     2500tuman
        public static VirtualCurrencyPack FIVHUNDCoin_PACK = new VirtualCurrencyPack(
            "500 coin",                                                // name
            "Buy 500 coins",                                           // description
            "500_CoinsPack",                                           // item id
            500,                                                       // number of currencies in the pack
            CHARSOO_CURRENCY_ITEM_ID,                                  // the currency associated with this pack
            new PurchaseWithMarket(FIVHUNDCoin_PACK_PRODUCT_ID, 24500)
        );

        //1000 Coin Pack    5000tuman
        public static VirtualCurrencyPack THOUSANDCoin_PACK = new VirtualCurrencyPack(
            "1000 coin",                                                 // name
            "Buy 1000 coins",                                            // description
            "1000_CoinsPack",                                            // item id
            1000,                                                        // number of currencies in the pack
            CHARSOO_CURRENCY_ITEM_ID,                                    // the currency associated with this pack
            new PurchaseWithMarket(THOUSANDCoin_PACK_PRODUCT_ID, 49000)
        );

        //3000 Coin Pack    10000tuman
        public static VirtualCurrencyPack THRTHOUSNDCoin_PACK = new VirtualCurrencyPack(
            "3000 coin",                                         // name
            "Buy 3000 coins",                                    // description
            "3000_CoinsPack",                                    // item id
            3000,                                                // number of currencies in the pack
            CHARSOO_CURRENCY_ITEM_ID,                            // the currency associated with this pack
            new PurchaseWithMarket(THRTHOUSNDCoin_PACK_PRODUCT_ID, 9900)
        );

        //10000 Coin Pack    5900tuman
        public static VirtualCurrencyPack TENTHOUSNDCoin_PACK = new VirtualCurrencyPack(
            "10000 coin",                                         // name
            "Buy 10000 coins",                                    // description
            "10000_CoinsPack",                                    // item id
            10000,                                                // number of currencies in the pack
            CHARSOO_CURRENCY_ITEM_ID,                             // the currency associated with this pack
            new PurchaseWithMarket(TENTHOUSNDCoin_PACK_PRODUCT_ID, 59000)
        );

        #endregion

        #region LifeTimeVGs

        //Remove Unwanted Ads   1500tuman
        public static VirtualGood NO_ADS_LTVG = new LifetimeVG(
            "No Ads",                                                       // name
            "No More Ads!",                                                 // description
            "no_uw_ads",                                                       // item id
            new PurchaseWithMarket(NO_ADS_LIFETIME_PRODUCT_ID, 15000));  // the way this virtual good is purchased


        //Double Coin Earn      5000tuman
        public static VirtualGood CHARSOO_DOUBLER = new LifetimeVG(
            "Double Earn Coins",                                               // name
            "Customers get double coin for solve each puzzle", // description
            "charsoo_doubler",                                               // item id
            new PurchaseWithMarket(CHARSOO_DOUBLER_LIFETIME_ID, 25000)); // the way this virtual good is purchased

        #endregion
    }

}