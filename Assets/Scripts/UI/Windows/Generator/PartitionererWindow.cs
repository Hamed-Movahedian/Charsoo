using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class PartitionererWindow : UIWindowBase
{
    public Partitioner Partitioner;

    #region Partition 

    public IEnumerator Partition()
    {
        yield return Hide();

        Partitioner.MinSize = int.Parse(GetComponentByName<InputField>("Min").text);
        Partitioner.MaxSize = int.Parse(GetComponentByName<InputField>("Max").text);

        // Show inProgress window
        yield return UIController.ShowProgressbarWindow(LanguagePack.Inprogress_GenerateWordSet);

        // Generate words
        yield return MgsCoroutine.StartCoroutineRuntime(
            Partitioner.PortionLetters(),
            () => UIController.SetProgressbar(MgsCoroutine.Percentage,MgsCoroutine.Info),
            .1);

        // Hide in-progress window
        yield return UIController.HideProgressbarWindow();

        // Show this window again
        yield return Show();
    }

    #endregion

    protected override void OnShow()
    {
        base.OnShow();
        GetComponentByName<InputField>("Min").text = "1";
        GetComponentByName<InputField>("Max").text = "5";
    }
}
