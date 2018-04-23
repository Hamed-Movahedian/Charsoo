using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InversPos : MonoBehaviour
{
    public RectTransform Aim;
    public Transform Objectt;
    // Use this for initialization
    void Start()
    {

    }

    public void ShowPos()
    {
        Objectt.gameObject.SetActive(false);
        Objectt.position= Camera.main.ScreenToWorldPoint(Aim.position);
        Objectt.gameObject.SetActive(true);
    }

    // UpdateData is called once per frame
    void Update()
    {

    }
}
