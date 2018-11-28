using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResetInput : MonoBehaviour
{
    private InputField _field;

    public void ResetText()
    {
        _field = GetComponent<InputField>();

        if (_field != null)
            _field.text = "";
    }
}
