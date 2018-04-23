using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Image ProgressBar;
    private AsyncOperation _asyncLoad;
    // Use this for initialization
    void Start()
    {
        LoadScene(1);
    }

    public void LoadScene(int level)
    {
        StartCoroutine(LoadYourAsyncScene(level));
    }

    IEnumerator LoadYourAsyncScene(int level)
    {
        _asyncLoad = SceneManager.LoadSceneAsync(level);
        _asyncLoad.allowSceneActivation = true;
        while (!_asyncLoad.isDone)
        {
            ProgressBar.fillAmount = _asyncLoad.progress/.9f;
            yield return null;
        }
        //_asyncLoad.allowSceneActivation = true;
    }

    private void Startlevel()
    {
        _asyncLoad.allowSceneActivation = true;
    }

    // UpdateData is called once per frame
    void Update()
    {

    }
}
