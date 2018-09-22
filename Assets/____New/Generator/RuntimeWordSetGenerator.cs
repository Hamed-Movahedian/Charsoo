using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using MgsCommonLib.Theme;
using MgsCommonLib.UI;
using MgsCommonLib.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RuntimeWordSetGenerator : MonoBehaviour
{
    [Header("Windows")]
    public WindowGetWords WordsWindow;
    public WindowGetClue ClueWindow;
    public WindowGetWordCount WordCountWindow;

    [Header("Components")]
    public WordSetGenerator Generator;
    public Partitioner Partitioner;

    private int _targetFrameRate;
    private int _vSyncCount;

    public void Finish()
    {
        //**************************** Clear screen
        GameController.Instance.ClearWords();

        // Set application to normal mode
        Application.targetFrameRate = _targetFrameRate;
        QualitySettings.vSyncCount = _vSyncCount;

    }

    public void Initialize()
    {
        
        GameController.Instance.ClearWords();

        // Disable letter selection
        Singleton.Instance.RayCaster.TriggerRaycast(false);
        Singleton.Instance.RayCaster.EnablePan(false);

        // Set application fast mode
        _targetFrameRate = Application.targetFrameRate;
        _vSyncCount = QualitySettings.vSyncCount;
        Application.targetFrameRate = 0;
        QualitySettings.vSyncCount = 0;
        WordsWindow.WordsText.text="فسنجان سمبوسه سوپ کشک خورشقيمه قرمهسبزي قیمه بادمجان شیربرنج کلهپاچه باقالی‌پلو شیشلیک رشته‌پلو";

    }

    [FollowMachine("Generate words","Success,Fail")]
    public IEnumerator Generate()
    {
       FollowMachine.SetOutput("Fail");

        #region Setup word generator

        // Setup generator
        Generator.AllWords = WordsWindow.WordsText.text.Replace(' ', '\n');
        Generator.Clue = ClueWindow.ClueInputField.text;
        Generator.Initialize();
        Generator.UsedWordCount = (int) WordCountWindow.WordCountSlider.maxValue;
        Generator.MaxResults = 100;

        #endregion

        #region Generate words ...

        // Generate words
        yield return MgsCoroutine.StartCoroutineRuntime(
            Generator.MakeWordSet(),
            () => UIController.Instance.SetProgressbar(
                MgsCoroutine.Percentage,
                ThemeManager.Instance.LanguagePack.GetLable("Inprogress/GenerateWordSet")),
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
            FollowMachine.SetOutput("Success");

        #endregion


    }

    [FollowMachine("Generate words", "Success,Fail")]
    public IEnumerator Generate(string clue, string words, float count)
    {
        return null;
    }

    [FollowMachine("Save puzzle")]
    public void Save()
    {
        WordSet wordSet = GameController.Instance.GetWordSet();

        var puzzle = new UserPuzzle
        {
            Clue = wordSet.Clue,
            Content = StringCompressor.CompressString(JsonUtility.ToJson(wordSet))
        };

        LocalDBController.Instance.UserPuzzles.AddPuzzle(puzzle);
    }

}
