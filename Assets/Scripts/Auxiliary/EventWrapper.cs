using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventWrapper : MonoBehaviour
{
    public UnityEvent Action;

    public void RunAction()
    {
        Action.Invoke();
    }
}
