using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using MgsCommonLib.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class RuntimeWordSetGenerator : MonoBehaviour
{
    [Header("Windows")]
    public MgsUIWindow GetWordsWindow;
    public MgsUIWindow WordCountWindow;
    public MgsUIWindow WordsetApproval;
    public MgsUIWindow PartiotionFaildWindow;
    public MgsUIWindow CategorySelectionWindow;

    [Header("Components")]
    public WordSetGenerator Generator;
    public Partitioner Partitioner;

    public IEnumerator GenerateWordSet()
    {
        // set test words
        GetWordsWindow
            .GetComponentByName<InputField>("InputWords").text =
            "فسنجان سمبوسه سوپ کشک خورشقيمه قرمهسبزي قیمه بادمجان سبزی‌پلو شیربرنج کلهپاچه باقالی‌پلو شیشلیک رشته‌پلو";

        // Delete all letters and words
        Singleton.Instance.LetterController.DeleteAllLetters();
        Singleton.Instance.WordManager.DeleteAllWords();

        // Disable letter selection
        Singleton.Instance.RayCaster.TriggerRaycast(false);
        Singleton.Instance.RayCaster.EnablePan(false);

        yield return GetWords();
    }

    private IEnumerator GetWords()
    {
        start:

        // show get words window and wait to close
        yield return GetWordsWindow.ShowWaitForCloseHide();

        if (GetWordsWindow.Result == "Next")
        {
            // Setup word generator
            Generator.AllWords = GetWordsWindow
                .GetComponentByName<InputField>("InputWords").text
                .Replace(' ', '\n');

            Generator.Initialize();

            yield return GetWordCount();

            if (WordCountWindow.Result == "Back")
                goto start;
        }
    }

    private IEnumerator GetWordCount()
    {
        // setup window
        Slider slider = WordCountWindow.GetComponentByName<Slider>("Slider");
        slider.minValue = 2;
        slider.maxValue = Generator.WordStrings.Count;
        slider.value = Generator.WordStrings.Count;

        startGetWordCount:

        // Show get word count window
        yield return WordCountWindow.ShowWaitForCloseHide();

        if (WordCountWindow.Result == "Generate")
        {
            GenerateWordSet:
            // Show inProgress window
            yield return UIController.Instance
                .ShowProgressbarWindow(ThemeManager.Instance.LanguagePack.Inprogress_GenerateWordSet);

            // Setup generator
            Generator.UsedWordCount = (int)slider.value;
            Generator.MaxResults = 1000;

            // Set application fast mode
            Application.targetFrameRate = 0;
            QualitySettings.vSyncCount = 0;

            // Generate words
            yield return MgsCoroutine.StartCoroutineRuntime(
                Generator.MakeWordSet(),
                () => UIController.Instance.SetProgressbar(
                    MgsCoroutine.Percentage,
                    ThemeManager.Instance.LanguagePack.Inprogress_GenerateWordSet),
                0.1);

            // Setup partitioner
            Partitioner.MaxSize = 5;
            Partitioner.MinSize = 1;
            Partitioner.MaxTry = 100;

            for (Generator.NextResultIndex = 0; Generator.NextResultIndex < 21; Generator.NextResultIndex += 10)
            {
                // Spawn words
                Generator.SpawnWordSet();

                // Run partitioner
                yield return MgsCoroutine.StartCoroutineRuntime(
                    Partitioner.PortionLetters(),
                    () => UIController.Instance.SetProgressbar(
                        MgsCoroutine.Percentage,
                        ThemeManager.Instance.LanguagePack.Inprogress_PartitionWordSet),
                    .1);

                // if partition successfully break
                if (Partitioner.PartitionSuccessfully)
                    break;
            }

            // Hide in-progress window
            StartCoroutine(UIController.Instance.HideProgressbarWindow());

            // Partition fails
            if (!Partitioner.PartitionSuccessfully)
            {
                PartiotionFaildWindow.SetTextMessage(ThemeManager.Instance.LanguagePack.Error_GenerateWordSet);
                PartiotionFaildWindow.SetIcon(ThemeManager.Instance.IconPack.GeneralError);

                PartiotionFaildWindow.ShowWaitForCloseHide();

                switch (PartiotionFaildWindow.Result)
                {
                    case "Regenerate":
                        goto GenerateWordSet;
                    case "Back":
                        goto startGetWordCount;
                }
            }
            else // Partition successfully
            {
                // Show puzzle selection window - Back,Select,Regenerate
                yield return WordsetApproval.ShowWaitForCloseHide();

                switch (WordsetApproval.Result)
                {
                    case "Back":
                        goto startGetWordCount;
                    case "Regenerate":
                        goto GenerateWordSet;
                    case "Select":

                        // Shuffle letters
                        yield return Partitioner.Shuffle();

                        // Show category selection
                        yield return CategorySelectionWindow.ShowWaitForCloseHide();


                        break;
                }
            }

            //yield return RegenerateWindow.ShowWaitForCloseHide();
        }

    }
}
