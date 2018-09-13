using System;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class UnlockItem : MonoBehaviour
{
    public MgsDialougWindow AskForBuyPanel;
    public UIMenuItemList BuyCoinPanel;


    [Header("Default Prices")]
    public int FreePackUnlock = 100;

    public Text PanelText;

    private int _price;

    public void UnlockPuzzles(int price)
    {

        AskForBuyPanel.Show();
    }


   /* public void ShowPackUnlock(PackPanelItem pack)
    {
        _lItem = pack;
        bool free = pack.Price == 0;
        BuyCoinPanel.SetActive(false);
        AskForBuyPanel.SetActive(true);
        _price = free ? FreePackUnlock : pack.Price;
        PanelText.text = ArabicFixer.Fix(UnlockPack.Replace("---", _price.ToString()), false, true);
        gameObject.SetActive(true);
    }


    public void ShowPuzzleUnlock(WordSetPanelItem wordSet)
    {
        _lItem = wordSet;
        BuyCoinPanel.SetActive(false);
        AskForBuyPanel.SetActive(true);
        _price = PackPuzzles;
        PanelText.text = ArabicFixer.Fix(UnlockPuzzles.Replace("---", _price.ToString()), false, true);
        gameObject.SetActive(true);
    }




    public void Unlock()
    {
        switch (_lItem.GetType() == typeof(PackPanelItem))
        {
            case true:

                if (PurchaseManager.PayCoin(_price))
                {
                    ((PackPanelItem)_lItem).Unlock();
                    gameObject.SetActive(false);
                }
                else
                {
                    AskForBuyPanel.SetActive(false);
                    BuyCoinPanel.SetActive(true);
                    StartCoroutine(CheckUnlock(() => ((PackPanelItem)_lItem).Unlock()));
                }

                break;

            case false:

                if (PurchaseManager.PayCoin(_price))
                {
                    _lItem.GetComponentInParent<PackPanelItem>().UnlockAll();
                    _lItem.OnSelect();
                    gameObject.SetActive(false);
                }
                else
                {
                    AskForBuyPanel.SetActive(false);
                    BuyCoinPanel.SetActive(true);
                    StartCoroutine(CheckUnlock(() =>
                    {
                        _lItem.GetComponentInParent<PackPanelItem>().UnlockAll();
                        _lItem.OnSelect();

                    }));
                }

                break;

        }

    }
*/

/*

    public IEnumerator CheckUnlock(Action onDoneAction)
    {
        while (!PurchaseManager.PayCoin(_price))
        {
            yield return new WaitForSeconds(0.1f);
        }

        onDoneAction.Invoke();

        BuyCoinPanel.SetActive(false);
        gameObject.SetActive(false);
    }

*/

}
