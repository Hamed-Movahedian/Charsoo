using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.Animation;
using UnityEditor;
using UnityEngine;

public class RuntimeWordSetGenerator : MonoBehaviour
{
    [Header("Windows")]
    public WindowGetWords WordsWindow;
    public WindowGetClue ClueWindow;
    public WindowGetWordCount WordCountWindow;

    [Header("Components")]
    public WordSetGenerator Generator;

    private int _targetFrameRate;
    private int _vSyncCount;
    private NewPartitioner _partitioner;
    private Shuffler _shuffler;

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
        WordsWindow.WordsText.text = "فسنجان سمبوسه سوپ کشک خورشقيمه قرمهسبزي قیمه بادمجان شیربرنج کلهپاچه باقالی‌پلو شیشلیک رشته‌پلو";

    }

    [FollowMachine("Generate words", "Success,Fail")]
    public IEnumerator Generate()
    {
        // Setup generator
        Generator.AllWords = WordsWindow.WordsText.text.Replace(' ', '\n');
        Generator.Clue = ClueWindow.ClueInputField.text;
        Generator.Initialize();
        Generator.UsedWordCount = (int)WordCountWindow.WordCountSlider.maxValue;
        Generator.MaxResults = 100;

        // Generate Word set
        yield return Generator.MakeWordSet();

        // fail to generate word sets
        if (!Generator.Successful)
        {
            FollowMachine.SetOutput("Fail");
            yield break;
        }

        // Spawn words
        var bestWordSet = Generator.GetBestWordSet();

        Singleton.Instance.WordSpawner.SpawnPartByPart = false;
        GameController.Instance.SpawnWordSet(bestWordSet);
        Singleton.Instance.WordSpawner.SpawnPartByPart = true;

        // Run partitioner
        if (_partitioner == null)
            _partitioner = new NewPartitioner();
        yield return _partitioner.Portion();

        FollowMachine.SetOutput("Success");
    }

    public IEnumerator Suffle()
    {
        if (_shuffler == null)
            _shuffler = new Shuffler();


        yield return _shuffler.ShuffleRuntime(this);
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
