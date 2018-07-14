using System;
using System.Collections;
using System.Linq;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class GeneratorUI : MonoBehaviour
{
    [Header("Windows")]
    public MgsUIWindow GetClueWindow;
    public MgsUIWindow GetWordsWindow;
    public MgsUIWindow WordCountWindow;
    public MgsUIWindow WordsetApproval;
    public MgsUIWindow PartiotionFaildWindow;
    public MgsUIWindow CategorySelectionWindow;
    public MgsUIWindow FinalizeWindow;
    public bool Back
    {
        get { return _lastWindow.Result == "Back"; }
    }

    public string Result
    {
        get
        {
            return _lastWindow.Result;
        }
    }

    private MgsUIWindow _lastWindow;

    public IEnumerator ShowClue()
    {
        _lastWindow = GetClueWindow;
        yield return GetClueWindow.ShowWaitForCloseHide();
    }

    public IEnumerator ShowWords()
    {
        _lastWindow = GetWordsWindow;
        yield return GetWordsWindow.ShowWaitForCloseHide();

        // get word count
        int wordCount = GetWordsWindow
            .GetComponentByName<InputField>("InputWords").text
            .Split(' ', ',')
            .Count(s => s.Trim() != "");

        // setup window
        Slider slider = WordCountWindow.GetComponentByName<Slider>("Slider");
        slider.minValue = 2;
        slider.maxValue = wordCount;
        slider.value = wordCount;
    }

    public IEnumerator ShowWordCount()
    {
        _lastWindow = WordCountWindow;
        yield return WordCountWindow.ShowWaitForCloseHide();
    }

    public IEnumerator ShowGenerationFailed()
    {
        _lastWindow = PartiotionFaildWindow;
        yield return PartiotionFaildWindow.ShowWaitForCloseHide();

    }

    public void SetWords(string text)
    {
        GetWordsWindow
            .GetComponentByName<InputField>("InputWords").text = text;
    }

    public IEnumerator ShowSelection()
    {
        _lastWindow = WordsetApproval;
        yield return WordsetApproval.ShowWaitForCloseHide();

    }
}