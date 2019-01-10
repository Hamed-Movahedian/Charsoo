using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using Newtonsoft.Json.Linq;
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
    private PlayerInfo _playerInfo;
    private int _rewardMultiplier;

    // Use this for initialization
    void Start()
    {
        //CommandController.AddListenerForCommand("GiveReward",GiveReward);
        _playerInfo = Singleton.Instance.PlayerController.PlayerInfo;
        bool hasDubler = _playerInfo?.HasDubler ?? false;
        BuyDublerButtons.ForEach(b => b.interactable = !hasDubler);
    }

    public int RewardMultiplier
    {
        get
        {
            _rewardMultiplier = _playerInfo!=null ? 1 + (_playerInfo.HasDubler? 1 : 0):1;
            return _rewardMultiplier;
        } 
    }

    private void GiveReward(JToken dataToken)
    {
        // Get new categories from json
        List<JToken> newRewards = dataToken.Select(ct => ct.ToObject<JToken>()).ToList();

        // Add or update local db
        foreach (JToken newReward in newRewards)
        {
            int amount= int.Parse(newReward["RewardAmount"].ToString().Trim());
        }

    }

    public void GiveSolveReward()
    {
        OnReward.Invoke();
        GiveCoin(RewardMultiplier * WordsetSolveReward);
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
        if (_playerInfo == null)
            _playerInfo = Singleton.Instance.PlayerController.PlayerInfo;

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
