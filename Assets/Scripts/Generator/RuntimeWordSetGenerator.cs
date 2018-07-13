using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using MgsCommonLib.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RuntimeWordSetGenerator : MonoBehaviour
{
    [Header("Windows")]
    public MgsUIWindow GetClueWindow;
    public MgsUIWindow GetWordsWindow;
    public MgsUIWindow WordCountWindow;
    public MgsUIWindow WordsetApproval;
    public MgsUIWindow PartiotionFaildWindow;
    public MgsUIWindow CategorySelectionWindow;
    public MgsUIWindow FinalizeWindow;

    [Header("Components")]
    public WordSetGenerator Generator;
    public Partitioner Partitioner;

    [Header("Events")]
    public UnityEvent OnExit;

    public IEnumerator StartProcess()
    {
        #region Prepare Wordset generation process

        // Delete all letters and words
        Singleton.Instance.LetterController.DeleteAllLetters();
        Singleton.Instance.WordManager.DeleteAllWords();

        // Disable letter selection
        Singleton.Instance.RayCaster.TriggerRaycast(false);
        Singleton.Instance.RayCaster.EnablePan(false);

        // Set application fast mode
        Application.targetFrameRate = 0;
        QualitySettings.vSyncCount = 0;

        #endregion

        // ************************ Start process

        #region GetClueWindow 

        GetClueWindow:

        yield return GetClueWindow.ShowWaitForCloseHide();

        #endregion

        if (GetClueWindow.Result == "Back")
        {
            OnExit.Invoke();
            yield break;
        }

        #region CategorySelectionWindow

        CategorySelectionWindow:

        // Show category selection
        yield return CategorySelectionWindow.ShowWaitForCloseHide();

        #endregion

        if (CategorySelectionWindow.Result == "Back")
            goto GetClueWindow;

        #region Get Selected category

        // Get Selected category
        var selectedToggle = CategorySelectionWindow
            .GetComponentInChildren<MgsUIToggle>(true)
            .GetActiveToggle();

        #endregion

        #region GetWordsWindow 

        // set test words
        GetWordsWindow
                .GetComponentByName<InputField>("InputWords").text =
            "فسنجان سمبوسه سوپ کشک خورشقيمه قرمهسبزي قیمه بادمجان شیربرنج کلهپاچه باقالی‌پلو شیشلیک رشته‌پلو";

        GetWordsWindow:

        // show get words window and wait to close
        yield return GetWordsWindow.ShowWaitForCloseHide();

        #endregion

        if (GetWordsWindow.Result == "Back")
            goto CategorySelectionWindow;

        #region WordCountWindow 

        // setup window
        Slider slider = WordCountWindow.GetComponentByName<Slider>("Slider");
        slider.minValue = 2;
        slider.maxValue = Generator.WordStrings.Count;
        slider.value = Generator.WordStrings.Count;

        WordCountWindow:

        // Show get word count window
        yield return WordCountWindow.ShowWaitForCloseHide();

        #endregion

        if (WordCountWindow.Result == "Back")
            goto GetWordsWindow;

        #region Generate word sets and partition ...

        GenerateWordSet:

        // Show inProgress window
        yield return UIController.Instance
            .ShowProgressbarWindow(ThemeManager.Instance.LanguagePack.Inprogress_GenerateWordSet);

        #region Setup word generator

        // Setup generator
        Generator.AllWords = GetWordsWindow
            .GetComponentByName<InputField>("InputWords").text
            .Replace(' ', '\n');

        Generator.Initialize();
        Generator.UsedWordCount = (int) slider.value;
        Generator.MaxResults = 500;

        #endregion

        #region Generate words ...

        // Generate words
        yield return MgsCoroutine.StartCoroutineRuntime(
            Generator.MakeWordSet(),
            () => UIController.Instance.SetProgressbar(
                MgsCoroutine.Percentage,
                ThemeManager.Instance.LanguagePack.Inprogress_GenerateWordSet),
            0.1);

        #endregion

        #region Setup partitioner

        // Setup partitioner
        Partitioner.MaxSize = 5;
        Partitioner.MinSize = 1;
        Partitioner.MaxTry = 200;

        #endregion

        #region Partition word set ...

        for (Generator.NextResultIndex = 0; Generator.NextResultIndex < 21; Generator.NextResultIndex += 10)
        {
            // Spawn words
            Generator.SpawnWordSet();

            // Run partitioner
            yield return MgsCoroutine.StartCoroutineRuntime(
                Partitioner.PortionLetters(),
                () => UIController.Instance.SetProgressbar(
                    MgsCoroutine.Percentage,
                    //ThemeManager.Instance.LanguagePack.Inprogress_PartitionWordSet),
                    MgsCoroutine.Info),
                .1);

            // if partition successfully break
            if (Partitioner.PartitionSuccessfully)
                break;
        }

        #endregion

        // Hide in-progress window
        StartCoroutine(UIController.Instance.HideProgressbarWindow());

        #endregion


        // ************************************** Partition fails
        if (!Partitioner.PartitionSuccessfully)
        {
            #region PartiotionFaildWindow

            PartiotionFaildWindow.SetTextMessage(ThemeManager.Instance.LanguagePack.Error_GenerateWordSet);
            PartiotionFaildWindow.SetIcon(ThemeManager.Instance.IconPack.GeneralError);

            yield return PartiotionFaildWindow.ShowWaitForCloseHide();

            #endregion

            switch (PartiotionFaildWindow.Result)
            {
                case "Regenerate":
                    goto GenerateWordSet;
                case "Back":
                    goto WordCountWindow;
            }
        }

        // ************************************* Partition successfully

        #region WordsetApproval

        WordsetApproval:
        // Show puzzle selection window - Back,Select,Regenerate
        yield return WordsetApproval.ShowWaitForCloseHide();

        #endregion

        switch (WordsetApproval.Result)
        {
            case "Back":
                goto WordCountWindow;
            case "Regenerate":
                goto GenerateWordSet;
        }

        // ******************** Select Button

        // Shuffle letters
        yield return Partitioner.Shuffle();

        #region WordsetApproval

        SaveWordSet:
        // Show puzzle selection window - Back,Select,Regenerate
        yield return FinalizeWindow.ShowWaitForCloseHide();

        #endregion

        switch (FinalizeWindow.Result)
        {
            case "Exit":
                OnExit.Invoke();
                yield break;
            case "Save":
                Debug.Log("Puzzle Saved");
                OnExit.Invoke();
                yield break;
        }



        // Save wordset to selected category

    }
}
