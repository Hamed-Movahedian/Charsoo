using System;
using System.Collections.Generic;
using ArabicSupport;
using TapsellSDK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NativeBannerCaller : MonoBehaviour
{
    private string _zoneId = "5a3fe609fca4f0000126e631";
    private TapsellNativeBannerAd _nativeAd;

    [Header("Ad Buttons")] public List<NativeAdButton> Buttons;

    public UnityEvent OnAdReady;


    private void Start()
    {
        GetNative();
    }

    // Use this for initialization
    public void GetNative()
    {
        Action request = () => { Invoke(nameof(GetNative), 1f); };

        Buttons.ForEach(b => b.gameObject.SetActive(false));

        _nativeAd = null;
        
        Tapsell.requestNativeBannerAd(this, _zoneId,
            (result) =>
            {
                // onRequestFilled
                if (result.landscapeBannerImage == null)
                {
                    Invoke(nameof(GetNative), 1f);
                    return;
                }

                _nativeAd = result; // store this to show the ad later
                OnAdReady.Invoke();

                foreach (NativeAdButton button in Buttons)
                    button.SetVisual(_nativeAd, request);
            },
            (string zoneId) =>
            {
                // onNoAdAvailable
            },
            (TapsellError error) =>
            {
                // onError
            },
            (string zoneId) =>
            {
                // onNoNetwork
            }
        );
    }

    private void SetAdVisual()
    {
    }
}