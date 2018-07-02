using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.Utilities;
using UnityEngine;

public class RegenerateWindow : UIWindowBase
{
    public WordSetGenerator WordSetGenerator;
    public PartitionererWindow PartitionererWindow;


    #region Regenerate 

    public IEnumerator Regenerate()
    {
        // Delete all letters and words
        Singleton.Instance.LetterController.DeleteAllLetters();
        Singleton.Instance.WordManager.DeleteAllWords();

        // Hide this window
        yield return Hide();

        // Show inProgress window
        yield return UIController.ShowProgressbarWindow(LanguagePack.Inprogress_GenerateWordSet);

        // Generate words
        yield return MgsCoroutine.StartCoroutineRuntime(
            WordSetGenerator.MakeWordSet(),
            () => UIController.SetProgressbar(MgsCoroutine.Percentage,MgsCoroutine.Info),
            0.1);

        // Hide in-progress window
        yield return UIController.HideProgressbarWindow();

        // Spawn words
        WordSetGenerator.SpawnWordSet();

        // Show this window
        yield return Show();

    }

    #endregion

    #region Continue - go to partitioner

    public IEnumerator Continue()
    {
        yield return Hide();

        yield return PartitionererWindow.ShowWaitForCloseHide();

        Close();
    }

    #endregion
}
