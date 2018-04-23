using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InvokeEvents : MonoBehaviour
{
    public UnityEvent[] Events;
    // Use this for initialization
    void Start()
    {

    }

    public void RunEvent(int index)
    {
        index = Mathf.Min(index, Events.Length);
        Events[index].Invoke();
    }

    // UpdateData is called once per frame
    void Update()
    {

    }
}
