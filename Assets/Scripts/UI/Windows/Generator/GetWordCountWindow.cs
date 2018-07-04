using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class GetWordCountWindow : UIWindowBase
{
    public WordSetGenerator WordSetGenerator;
    public RegenerateWindow RegenerateWindow;

    private Slider _slider;

    public IEnumerator Continue()
    {
        // Set slider value as word count
        WordSetGenerator.UsedWordCount = (int) _slider.value;

        // Hide this window
        StartCoroutine(Hide());

        // Show inProgress window
        yield return UIController.ShowProgressbarWindow(LanguagePack.Inprogress_GenerateWordSet);

        Application.targetFrameRate = 0;
        QualitySettings.vSyncCount = 0;
        // Generate words
        yield return MgsCoroutine.StartCoroutineRuntime(
            WordSetGenerator.MakeWordSet(),
            () => UIController.SetProgressbar(MgsCoroutine.Percentage,MgsCoroutine.Info),
            0.1);

        // Hide in-progress window
        StartCoroutine(UIController.HideProgressbarWindow());

        // Spawn words
        WordSetGenerator.SpawnWordSet();

        // Show Regenerate window
        yield return RegenerateWindow.ShowWaitForCloseHide();
    }

    protected override void OnShow()
    {
        base.OnShow();

        if(_slider==null)
            _slider= GetComponentByName<Slider>("Slider");

        _slider.minValue = 2;
        _slider.maxValue = WordSetGenerator.WordStrings.Count;
    }
}
