using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using Soomla.Store;
using Soomla.Store.Charsoo;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PurchaseManager : BaseObject
{
    public int CurrentCoin = 0;
    public int WordsetSolveReward = 8;
    public UnityEvent OnCurrencyChange;
    public UnityEvent OnReward;
    public AudioClip PayCoinAudioClip;
    public AudioClip GiveCoinAudioClip;
    public List<Button> BuyDublerButtons;
    private int _rewardMultiplier = 1;
    private PlayerInfo _playerInfo;
    // Use this for initialization
    void Start()
    {
        _playerInfo = Singleton.Instance.PlayerController.PlayerInfo;
        bool hasDubler = _playerInfo?.HasDubler ?? false;
        _rewardMultiplier = 1 + (hasDubler ? 1 : 0);
        _rewardMultiplier = Mathf.Clamp(_rewardMultiplier, 1, 2);
        BuyDublerButtons.ForEach(b => b.interactable = !hasDubler);
    }

    public void GiveSolveReward()
    {
        OnReward.Invoke();
        GiveCoin(_rewardMultiplier * WordsetSolveReward);
    }

    public void BuyItem(string itemId)
    {
        int cBalance = StoreInventory.GetItemBalance("charsoo_coin");

        StoreInventory.BuyItem(itemId);

        if (StoreInventory.GetItemBalance("charsoo_coin") > cBalance)
            SoundManager.PlayAudioClip(GiveCoinAudioClip);
    }

    [FollowMachine("Pay Coin", "Payed,NotEnough")]
    public void PayCoins(int amount)
    {
        if (amount > _playerInfo.CoinCount)
        {
            FollowMachine.SetOutput("NotEnough");
            return;
        }
        StoreInventory.TakeItem("charsoo_coin", amount);

        SoundManager.PlayAudioClip(PayCoinAudioClip);

        FollowMachine.SetOutput("Payed");
    }

    public void GiveCoin(int amount)
    {
        SoundManager.PlayAudioClip(GiveCoinAudioClip);
        StoreInventory.GiveItem("charsoo_coin", amount);
    }

    public IEnumerator CurrencyChanged()
    {
        OnCurrencyChange.Invoke();
        yield return null;
        Start();
    }

    public void HcurrencyChanged()
    {
        StartCoroutine(CurrencyChanged());
    }
}
