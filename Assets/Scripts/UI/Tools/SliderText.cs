using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderText : MonoBehaviour
{
    private Text _textComponent;

    public Single SetText
    {
        set
        {
            if (_textComponent == null)
                _textComponent = GetComponent<Text>();

            _textComponent.text = value.ToString();
        }
    }
}
