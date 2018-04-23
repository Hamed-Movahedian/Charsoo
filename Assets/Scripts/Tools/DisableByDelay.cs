using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableByDelay : MonoBehaviour
{
    public float Delay=0.5f;

    private void OnEnable()
    {
        Invoke("Disable",Delay);
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}
