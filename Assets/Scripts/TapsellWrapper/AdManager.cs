using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using TapsellSDK;
using UnityEngine;
using UnityEngine.Events;

public class AdManager : MgsSingleton<AdManager>
{

    #region private

    private string _zoneId = "5a0702a473916d0001daa04c";
    private TapsellAd _adid;
    private TapsellShowOptions _showOptions;
    private bool _cache = true;
    private bool _endShow;
    private bool _completWatch;

    #endregion

    public UnityEvent OnAvalable;
    public UnityEvent OnUnavalable;

    private bool _isAvailable;

    public bool IsAvailable
    {
        get { return _isAvailable; }
        private set
        {
            _isAvailable = value;

            if (value)
                OnAvalable.Invoke();
            else
                OnUnavalable.Invoke();
        }
    }

    // ******************

    void Start()
    {
        #region _showOptions

        _showOptions = new TapsellShowOptions();
        _showOptions.backDisabled = false;
        _showOptions.immersiveMode = false;
        _showOptions.rotationMode = TapsellShowOptions.ROTATION_UNLOCKED;
        _showOptions.showDialog = true;

        #endregion

        Tapsell.setRewardListener((TapsellAdFinishedResult result) =>
            {
                _endShow = true;
                _completWatch = result.completed;
                RequestAd();
            }
        );

        RequestAd();
    }

    public void RequestAd()
    {
        IsAvailable = false;
        _adid = null;

        Tapsell.requestAd(_zoneId, _cache,

            // onAdAvailable
            (TapsellAd result) =>
            {
                IsAvailable = true;
                _adid = result;
            },

            // onNoAdAvailable
            (string zoneId) =>
            {
                Invoke("RequestAd", 20);
            },

            // onError
            (TapsellError error) =>
            {
                Invoke("RequestAd", 20);
            },

            // onNoNetwork
            (string zoneId) =>
            {
                Invoke("RequestAd", 20);
            },

            //onExpiring
            (TapsellAd result) =>
            {
                RequestAd();
            }

        );
    }

    [FollowMachine("Show Ad","Rewarded,Cancel")]
    public IEnumerator ShowAd()
    {
        _endShow = false;
        Tapsell.showAd(_adid, _showOptions);

        yield return new WaitUntil(()=>_endShow);

        FollowMachine.SetOutput(_completWatch ? "Rewarded": "Cancel");
    }

    [FollowMachine("Is Ad Avaliable","Yes,No")]
    public void IsAvalable()
    {
        FollowMachine.SetOutput(IsAvailable ? "Yes":"No");
    }
}