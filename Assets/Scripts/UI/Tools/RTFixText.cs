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
            string s = TargetText.text.Replace("ی", "ي");
            GetComponent<Text>().text = ArabicFixer.Fix(s);
            GetComponentInParent<InputField>().text = s;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
