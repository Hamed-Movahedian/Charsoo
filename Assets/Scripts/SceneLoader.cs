using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image ProgressBar;
    private AsyncOperation _asyncLoad;
    //public BazaarInitializer Initializer;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(LoadYourAsyncScene(1));
    }

    IEnumerator LoadYourAsyncScene(int level)
    {
        yield return new WaitForEndOfFrame();
        _asyncLoad = SceneManager.LoadSceneAsync(level);
        _asyncLoad.allowSceneActivation = true;
        while (!_asyncLoad.isDone)
        {
            ProgressBar.fillAmount = _asyncLoad.progress / .9f;
            yield return null;
        }
        _asyncLoad.allowSceneActivation = true;
    }

}
