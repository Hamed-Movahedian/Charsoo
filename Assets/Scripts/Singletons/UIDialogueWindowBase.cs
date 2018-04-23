using System;
using System.Collections;
using UnityEngine;

public class UIDialogueWindowBase<T> : BaseObject where T : UnityEngine.Object
{
    public GameObject WindowGameObject;
    private bool _isDone;
    private static T _window;
 
    public static T Window
    {
        get
        {
            if (_window == null)
                _window = FindObjectOfType<T>();

            if (_window == null)
                Debug.LogError(typeof(T).Name + " window not found!!!");

            return _window;
        }
    }

    public void Complete()
    {
        _isDone = true;
    }

    public IEnumerator ShowWindowAndWaitToClose()
    {
        // show window
        WindowGameObject.SetActive(true);

        // Wait
        _isDone = false;
        while (!_isDone)
            yield return null;

        // Hide window
        WindowGameObject.SetActive(false);
    }
}