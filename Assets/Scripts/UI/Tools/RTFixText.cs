using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class RTFixText : MonoBehaviour
{
    public Text TargetText;
    private Text _text;
    private string _content;

    public void Start()
    {
        _text = GetComponent<Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(FixText());

    }


    private void OnDisable()
    {
        StopCoroutine(FixText());

    }
    

    private IEnumerator FixText()
    {
        while (true)
        {
           _text.text = ArabicFixer.Fix(TargetText.text.Replace("ی", "ي"));
            yield return new WaitForSeconds(0.1f);
        }
    }

}
