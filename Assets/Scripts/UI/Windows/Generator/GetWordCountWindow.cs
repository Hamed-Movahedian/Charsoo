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
        yield return Hide();

        // Show inProgress window
        yield return UIController.ShowProgressbarWindow(LanguagePack.Inprogress_GenerateWordSet);

        // Generate words
        yield return MgsCoroutine.StartCoroutineRuntime(
            WordSetGenerator.MakeWordSet(),
            () => UIController.SetProgressbar(MgsCoroutine.Percentage),
            0.1);

        // Hide in-progress window
        yield return UIController.HideProgressbarWindow();

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
