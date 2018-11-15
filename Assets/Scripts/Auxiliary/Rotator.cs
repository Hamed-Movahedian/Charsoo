using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float Speed;
    public float Step = 0;
    private float _currentAngle=0;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _currentAngle += Speed * Time.deltaTime;
        if (Mathf.Abs(_currentAngle) > Step)
        {
            transform.Rotate(Vector3.forward, Step*Mathf.Sign(Speed));
            _currentAngle = 0;
        }
    }
}
