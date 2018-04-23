using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class CoinCounter : BaseObject
{
    private Text _text;
    private int _coinCount = 0;
    public float ChangeTextTime = 2f;
    private int _step = 6;
    private Vector3 _scale;
    // Use this for initialization
    void Start()
    {
        _scale = GetComponent<RectTransform>().localScale;
        _text = GetComponent<Text>();
        _coinCount = ZPlayerPrefs.GetInt("Coin");
        _text.text = ArabicFixer.Fix(_coinCount.ToString("D"), false, true);
        PurchaseManager.OnCurrencyChange.AddListener(SetCounter);
    }

    public void SetCounter()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeCoin(PurchaseManager.CurrentCoin));
    }

    IEnumerator ChangeCoin(int newCount)
    {
        int cc = _coinCount;
        _step = (int)((Mathf.Abs(newCount - cc) / ChangeTextTime) / 60);
        _step = Mathf.Max(_step, 1);
        if (cc < newCount)
            while (cc + _step < newCount)
            {
                GetComponent<RectTransform>().localScale = _scale*Random.Range(0.8f, 1.2f);
                cc += _step;
                _text.text = ArabicFixer.Fix(cc.ToString("D"), false, true);
                yield return new WaitForEndOfFrame();
            }
        else
            while (cc - _step > newCount)
            {
                GetComponent<RectTransform>().localScale = _scale*Random.Range(0.8f, 1.2f);
                cc -= _step;
                _text.text = ArabicFixer.Fix(cc.ToString("D"), false, true);
                yield return new WaitForEndOfFrame();
            }

        _text.text = ArabicFixer.Fix(newCount.ToString("D"), false, true);
        GetComponent<RectTransform>().localScale = _scale;
        _coinCount = newCount;
        yield return null;

    }


    // UpdateData is called once per frame
    void Update()
    {

    }
}
