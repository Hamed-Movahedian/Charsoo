using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetScaleByCameraSize : MonoBehaviour
{
    private Camera _camera;

    // Use this for initialization
    void Start()
    {
        _camera= GetComponentInParent<Camera>();
    }

    private void OnDisable()
    {
        GetComponentInChildren<Animator>().SetTrigger("Reset");
    }

    // UpdateData is called once per frame
    void Update()
    {
        Vector3 effectScale = Vector3.one * _camera.orthographicSize;
        effectScale.z = 10;
        transform.localScale = effectScale;
    }
}
