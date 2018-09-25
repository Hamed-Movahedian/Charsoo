using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using UnityEngine;

public class AndroidBack : MonoBehaviour
{
    static MgsUIWindow _currentWindow;

    public static void SetCurrentWindow(MgsUIWindow window)
    {
        _currentWindow = window;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            if (_currentWindow != null && _currentWindow.ActionList.Contains("Back"))
                _currentWindow.Close("Back");
    }
}
