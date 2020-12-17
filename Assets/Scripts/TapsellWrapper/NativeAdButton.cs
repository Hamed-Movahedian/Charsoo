using System;
using TapsellSDK;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NativeAdButton : MonoBehaviour
{
    [Header("Filled Ad")] public Text Title;
    public Text Description;
    public Text Content;
    public Image VerticalImage;
    public Image HorizentalImage;
    public Image Icon;
    public Button AdButton;

    [Range(0,1)]
    public float chance=0.5f;

    public void SetVisual(TapsellNativeBannerAd nativeAd, Action getNative)
    {
        float range =  Random.Range(0f,1f);
        if (range>chance)
            return;
        
        gameObject.SetActive(true);
        
        if (Title != null) Title.text = PersianFixer.Fix(nativeAd.getTitle(), true, true);
        if (Description != null) Description.text = PersianFixer.Fix(nativeAd.getDescription());
        if (Content != null) Content.text = PersianFixer.Fix(nativeAd.getCallToAction());
        if (VerticalImage != null) VerticalImage.sprite = TextureToSprite(nativeAd.getPortraitBannerImage());
        if (HorizentalImage != null) HorizentalImage.sprite = TextureToSprite(nativeAd.getLandscapeBannerImage());
        if (Icon != null) Icon.sprite = TextureToSprite(nativeAd.getIcon());


        gameObject.SetActive(true);
        
        nativeAd.onShown();

        AdButton.onClick.RemoveAllListeners();
        AdButton.onClick.AddListener(() =>
        {
            getNative.Invoke();
            nativeAd.onClicked();
        });
    }

    private void OnEnable()
    {
        transform.SetSiblingIndex(
            Random.Range(
                Mathf.Min(1,transform.parent.childCount),
                Mathf.Min(5,transform.parent.childCount)
                )
            );
    }

    private Sprite TextureToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(1f, 1f, texture.width - 1, texture.height - 1), Vector2.one / 2, 100f);
    }
}