using TapsellSDK;
using UnityEngine;
using UnityEngine.Events;

public class AdManager : BaseObject
{
    #region Enums

    public enum AdZone
    {
        MainMenu, Playing
    }
    public enum ErrorTypeEnum
    {
        None, NoNetwork, Other
    }

    #endregion

    #region Public

    public AdZone SelectZone;

    #endregion


    #region Events

    [Header("Ready")]
    public UnityEvent OnReady;
    public UnityEvent OnNotReady;

    [Header("No Error")]
    public UnityEvent OnReward;
    public UnityEvent OnFinish;

    [Header("Error")]
    public UnityEvent OnUnavalable;
    public UnityEvent OnNoNetwork;

    #endregion

    #region private

    private string _zoneId = "5a0702a473916d0001daa04c";
    private TapsellAd _adid;
    private ErrorTypeEnum _errorType = ErrorTypeEnum.None;
    private bool _available;
    private TapsellShowOptions _showOptions;
    private bool _cache = true;

    #endregion


    // ******************

    public void GetAdVideo(UnityEvent onReady)
    {
        OnReady.RemoveAllListeners();
        OnReady.AddListener(onReady.Invoke);

        if (_available)
            OnReady.Invoke();
        else
            RequestAd();
    }

    public void PlayAdVideo(UnityEvent onReward)
    {
        OnReward.RemoveAllListeners();
        OnReward.AddListener(onReward.Invoke);
        if (_available)
            ShowAd();
    }

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
                OnFinish.Invoke();

                if (result.completed)
                {
                    OnReward.Invoke();
                    OnNotReady.Invoke();
                }
                RequestAd();

            }
        );
    }

    public void RequestAd()
    {
        _available = false;
        _adid = null;

        OnNotReady.Invoke();

        Tapsell.requestAd(_zoneId, _cache,

            // onAdAvailable
            (TapsellAd result) =>
            {
                _available = true;
                _adid = result;
                _errorType = ErrorTypeEnum.None;
                OnReady.Invoke();
            },

            // onNoAdAvailable
            (string zoneId) =>
            {
                _errorType = ErrorTypeEnum.Other;
                OnUnavalable.Invoke();
                Invoke("RequestAd", 20);
            },

            // onError
            (TapsellError error) =>
            {
                _errorType = ErrorTypeEnum.Other;
                OnUnavalable.Invoke();
                Invoke("RequestAd", 20);
            },

            // onNoNetwork
            (string zoneId) =>
            {
                _errorType = ErrorTypeEnum.NoNetwork;
                OnNoNetwork.Invoke();
                Invoke("RequestAd", 20);
            },

            //onExpiring
            (TapsellAd result) =>
            {
                _errorType = ErrorTypeEnum.Other;
                RequestAd();
            }

        );
    }

    public void ShowAd()
    {
        Tapsell.showAd(_adid, _showOptions);
    }
}