using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class RTFixText : MonoBehaviour
{
    public Text TargetText;
    private string _content;

    public void Start()
    {
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
            GetComponent<Text>().text = PersianFixer.Fix(TargetText.text);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
