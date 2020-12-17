using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{

    public void Exit()
    {
        Application.Quit();
    }

    public void Vibrate()
    {
        if (false && Application.isMobilePlatform)
            Handheld.Vibrate();
    }

    public void LoadLevel(int level)
    {
        SceneManager.LoadScene(level);
    }

}
