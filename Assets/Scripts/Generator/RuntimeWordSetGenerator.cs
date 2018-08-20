using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib;
using MgsCommonLib.Theme;
using MgsCommonLib.UI;
using MgsCommonLib.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RuntimeWordSetGenerator : MgsSingleton<RuntimeWordSetGenerator>
{
    [Header("Windows")]
    public MgsUIWindow GetClueWindow;
    public MgsUIWindow GetWordsWindow;
    public MgsUIWindow WordCountWindow;
    public MgsUIWindow WordsetApproval;
    public MgsUIWindow PartiotionFaildWindow;

    [Header("Components")]
    public WordSetGenerator Generator;
    public Partitioner Partitioner;

    [Header("Events")]
    public UnityEvent OnExit;

    public bool GenerationFaild=true;

    public IEnumerator StartProcess()
    {
        #region Prepare Wordset generation process

        // Cache 
        GeneratorUI gui = UIController.Instance.Generator;

        GameController.Instance.ClearWords();

        // Disable letter selection
        Singleton.Instance.RayCaster.TriggerRaycast(false);
        Singleton.Instance.RayCaster.EnablePan(false);

        // Set application fast mode
        var targetFrameRate = Application.targetFrameRate;
        var vSyncCount = QualitySettings.vSyncCount;
        Application.targetFrameRate = 0;
        QualitySettings.vSyncCount = 0;

        // Set words for test
        gui.SetWords("فسنجان سمبوسه سوپ کشک خورشقيمه قرمهسبزي قیمه بادمجان شیربرنج کلهپاچه باقالی‌پلو شیشلیک رشته‌پلو");
        #endregion

        // ***************************** Clue
        clue:
        yield return gui.ShowClue();
        if (gui.Back)
            yield break;


        // ***************************** Words
        words:
        yield return gui.ShowWords();
        if (gui.Back)
            goto clue;


        // ***************************** Count
        count:
        yield return gui.ShowWordCount();
        if (gui.Back)
            goto words;

        //****************************** Generate
        generate:
        yield return Generate();
        if (GenerationFaild)
        {
            yield return gui.ShowGenerationFailed();

            if (gui.Back)
                goto count;
            else
                goto generate;
        }

        // ***************************** Selection
        yield return gui.ShowSelection();
        if (gui.Back)
            goto count;
        if (gui.Result == "Regenerate")
            goto generate;


        //***************************** Shuffle
        yield return Partitioner.Shuffle();

        //***************************** Save
        UserPuzzlesController.Instance.Save();

        //***************************** Success message
        yield return UIController.Instance.DisplayMessage(ThemeManager.Instance.LanguagePack.GetLable("SuccesfullOperation"));

        //**************************** Clear screen
        GameController.Instance.ClearWords();

        // Set application to normal mode
         Application.targetFrameRate = targetFrameRate;
         QualitySettings.vSyncCount = vSyncCount;

    }

    private IEnumerator Generate()
    {
        yield return UIController.Instance
            .ShowProgressbarWindow(ThemeManager.Instance.LanguagePack.GetLable("Inprogress_GenerateWordSet"));

        #region Setup word generator

        // Setup generator
        Generator.AllWords = UIController.Instance.Generator.GetWords().Replace(' ', '\n');
        Generator.Clue = UIController.Instance.Generator.GetClue();
        Generator.Initialize();
        Generator.UsedWordCount = UIController.Instance.Generator.GetWordCount();
        Generator.MaxResults = 100;

        #endregion

        #region Generate words ...

        // Generate words
        yield return MgsCoroutine.StartCoroutineRuntime(
            Generator.MakeWordSet(),
            () => UIController.Instance.SetProgressbar(
                MgsCoroutine.Percentage,
                ThemeManager.Instance.LanguagePack.GetLable("Inprogress_GenerateWordSet")),
            0.1);

        #endregion

        #region Setup partitioner

        // Setup partitioner
        Partitioner.MaxSize = 5;
        Partitioner.MinSize = 1;
        Partitioner.MaxTry = 200;
        Partitioner.Validate = false;

        #endregion


        // Spawn words
        var bestWordSet = Generator.GetBestWordSet();

        GameController.Instance.SpawnWordSet(bestWordSet);

        #region Partition word set ...

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

        #endregion

        GenerationFaild = false;
        // Hide in-progress window
        StartCoroutine(UIController.Instance.HideProgressbarWindow());
    }
}
