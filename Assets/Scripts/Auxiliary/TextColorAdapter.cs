using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ColorAdapter : MonoBehaviour
{
    public Color CurrentColor=Color.white;

    // Use this for initialization
    void Start()
    {

    }

    // UpdateData is called once per frame
    void Update()
    {
        GetComponent<Image>().material.SetColor("_TintColor",CurrentColor);
    }
}
