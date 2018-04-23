using Soomla.Store;
using Soomla.Store.Charsoo;
using UnityEngine;
using UnityEngine.Events;

public class PurchaseManager : BaseObject
{
    public int CurrentCoin = 0;
    public int WordsetSolveReward = 8;
    public UnityEvent OnCurrencyChange;
    public UnityEvent OnReward;
    public AudioClip PayCoinAudioClip;
    public AudioClip GiveCoinAudioClip;

    private int _rewardMultiplier = 1;
    // Use this for initialization
    void Start()
    {
        CurrentCoin = StoreInventory.GetItemBalance("charsoo_coin");
        ZPlayerPrefs.SetInt("Coin", CurrentCoin);
        _rewardMultiplier = 1+ ZPlayerPrefs.GetInt("Doubler", 0);
        _rewardMultiplier = Mathf.Clamp(_rewardMultiplier, 1, 2);
    }

    public void GiveSolveReward()
    {
        if (ContentManager.SelectedPuzzleItem.IsSolved)
            return;

        OnReward.Invoke();
        GiveCoin(_rewardMultiplier* WordsetSolveReward);
    }


    public void BuyItem(string itemId)
    {
        StoreInventory.BuyItem(itemId);
    }

    public bool PayCoin(int amount)
    {
        if (amount > CurrentCoin)
            return false;

        StoreInventory.TakeItem("charsoo_coin", amount);
        SoundManager.PlayAudioClip(PayCoinAudioClip);
        return true;
    }

    // Test In Editor
    public void Charge(int amount)
    {
        bool b = PayCoin(amount);
    }

    public void GiveCoin(int amount)
    {
        StoreInventory.GiveItem("charsoo_coin", amount);
    }

    public void CurrencyChanged()
    {
        CurrentCoin = StoreInventory.GetItemBalance("charsoo_coin");
        ZPlayerPrefs.SetInt("Coin", CurrentCoin);
        ZPlayerPrefs.SetInt("Doubler", StoreInventory.GetItemBalance("charsoo_doubler"));
        Start();
        OnCurrencyChange.Invoke();
        SoundManager.PlayAudioClip(GiveCoinAudioClip);
    }
}
