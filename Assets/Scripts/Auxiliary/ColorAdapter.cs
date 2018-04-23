using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextColorAdapter : MonoBehaviour
{
    public Color CurrentColor=Color.white;
    private Text _text;
    private TextMesh _textMesh;

    // Use this for initialization
    void Start()
    {
        _textMesh = GetComponent<TextMesh>();
        _text = GetComponent<Text>();
    }

    // UpdateData is called once per frame
    void Update()
    {
        if (_text != null) _text.color = CurrentColor;
        if (_textMesh != null) _textMesh.color = CurrentColor;
    }
}
