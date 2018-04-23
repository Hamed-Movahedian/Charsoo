using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    private int _count = 0;

    // Use this for initialization
    void Start()
    {
        GetComponent<Text>().text = _count.ToString();
    }

    public void AddCounter()
    {
        _count++;
        GetComponent<Text>().text = _count.ToString();

    }

    // UpdateData is called once per frame
    void Update()
    {

    }
}
