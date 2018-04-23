using System;
using ArabicSupport;
using TapsellSDK;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NativeBannerCaller : MonoBehaviour
{
    private string _zoneId = "5a3fe609fca4f0000126e631";
    private TapsellNativeBannerAd _nativeAd;


    public Text DebugText;

    [Header("Filled Ad")]
    public Text Title;
    public Text Description;
    public Text Content;
    public Image VerticalImage;
    public Image HorizentalImage;
    public Image Icon;
    public Button AdButton;
    public GameObject BannerGameObject;

    public UnityEvent OnAdReady;
    public UnityEvent OnReward;



    private float _unFocusTime;
    private float _reFocusTime;
    // Use this for initialization
    void Start()
    {
        Application.runInBackground = false;
    }

    public void GetNative()
    {
        _nativeAd = null;
        Tapsell.requestNativeBannerAd(this, _zoneId,
            (TapsellNativeBannerAd result) =>
            {
                // onRequestFilled
                DebugText.text = "Request Filled";
                _nativeAd = result; // store this to show the ad later
                OnAdReady.Invoke();
                SetAdVisual();
            },

            (string zoneId) =>
            {
                // onNoAdAvailable
                DebugText.text = "No Ad Available";
            },

            (TapsellError error) =>
            {
                // onError
                DebugText.text = error.error;
            },

            (string zoneId) =>
            {
                // onNoNetwork
                DebugText.text = "No Network";
            }
        );

    }

    private void SetAdVisual()
    {
        if (Title != null) Title.text = ArabicFixer.Fix(_nativeAd.getTitle(), true, true);
        if (Description != null) Description.text = ArabicFixer.Fix(_nativeAd.getDescription());
        if (Content != null) Content.text = ArabicFixer.Fix(_nativeAd.getCallToAction());

        if (VerticalImage != null) VerticalImage.sprite = TextureToSprite(_nativeAd.getPortraitBannerImage());
        if (HorizentalImage != null) HorizentalImage.sprite = TextureToSprite(_nativeAd.getLandscapeBannerImage());
        if (Icon != null) Icon.sprite = TextureToSprite(_nativeAd.getIcon());
        //BannerGameObject.transform.SetSiblingIndex(UnityEngine.Random.Range(0, 4));
        BannerGameObject.SetActive(true);
        _nativeAd.onShown();

        AdButton.onClick.RemoveAllListeners();
        AdButton.onClick.AddListener(() =>
        {
            //_unFocusTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
            Invoke("GiveReward", 1f);
            _nativeAd.onClicked();
        });
    }

    public void GiveReward()
    {
        if (_reFocusTime - _unFocusTime > 4)
        {
            OnReward.Invoke();
            DebugText.text = "Player Rewarded";
            BannerGameObject.SetActive(false);
            GetNative();
        }
        else
            DebugText.text = "Fast Back";

    }

    public void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            _reFocusTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
            DebugText.text = "Application Focus \n " + _reFocusTime;
        }
        if (!focus)
        {
            _unFocusTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
            DebugText.text = "Application UnFocus \n " + _reFocusTime;
        }
    }

    public void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _unFocusTime = DateTime.Now.Hour * 3600 + DateTime.Now.Minute * 60 + DateTime.Now.Second;
            DebugText.text = "Application UnFocus \n " + _reFocusTime;
        }
    }



    public Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(1f, 1f, texture.width - 1, texture.height - 1), Vector2.one / 2, 100f);
    }



    // Update is called once per frame
    void Update()
    {

    }
}
