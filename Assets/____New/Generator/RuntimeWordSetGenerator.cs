using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.Animation;
using UnityEngine;

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
    private NewPartitioner _partitioner;

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

    [FollowMachine("Generate words","Success,Fail")]
    public IEnumerator Generate()
    {
       FollowMachine.SetOutput("Fail");

        // Setup generator
        Generator.AllWords = WordsWindow.WordsText.text.Replace(' ', '\n');
        Generator.Clue = ClueWindow.ClueInputField.text;
        Generator.Initialize();
        Generator.UsedWordCount = (int) WordCountWindow.WordCountSlider.maxValue;
        Generator.MaxResults = 100;

        // Generate Word set
        yield return Generator.MakeWordSet();

        // fail to generate word sets
        if (!Generator.Successful)
        {
            FollowMachine.SetOutput("Fail");
            yield break;
        }

        // Setup partitioner
        Partitioner.MaxSize = 4;
        Partitioner.MinSize = 1;
        Partitioner.MaxTry = 200;
        Partitioner.Validate = false;

        // Spawn words
        var bestWordSet = Generator.GetBestWordSet();

        Singleton.Instance.WordSpawner.SpawnPartByPart = false;
        GameController.Instance.SpawnWordSet(bestWordSet);
        Singleton.Instance.WordSpawner.SpawnPartByPart = true;


        // Run partitioner
        //yield return Partitioner.PortionLetters();
        if(_partitioner==null)
        _partitioner =new NewPartitioner();
        _partitioner.Portion();
        
        // if partition successfully break
        //if (Partitioner.PartitionSuccessfully)
            FollowMachine.SetOutput("Success");
    }

 

    public IEnumerator Shuffle()
    {
        List<List<Letter>> paritions = new List<List<Letter>>();

        var allLetters =
            new List<Letter>(Singleton.Instance.LetterController.AllLetters);

        while (allLetters.Count > 0)
        {
            var letters = new List<Letter>();
            allLetters[0].GetConnectedLetters(letters);
            paritions.Add(letters);
            letters.ForEach(l => allLetters.Remove(l));
        }

        #region Shuffle partions

        for (int i = 0; i < paritions.Count * 2; i++)
        {
            int p1 = Random.Range(0, paritions.Count);
            int p2 = Random.Range(0, paritions.Count);

            var tp = paritions[p1];
            paritions[p1] = paritions[p2];
            paritions[p2] = tp;
        }

        #endregion

        #region Get bounds

        List<LetterBound> letterBounds = new List<LetterBound>();

        foreach (List<Letter> letters in paritions)
            letterBounds.Add(new LetterBound(letters));

        #endregion

        #region Place as a grid

        int columns = Mathf.RoundToInt(Mathf.Sqrt(letterBounds.Count));

        int x = 0, y = 0;

        for (int i = 0; i <= columns; i++)
        {
            int width = 0;
            int height = 0;

            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;

                if (index >= paritions.Count)
                    break;

                width += letterBounds[index].Width;

                if (height < letterBounds[index].Height)
                    height = letterBounds[index].Height;
            }

            x = -width / 2;

            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;

                if (index >= paritions.Count)
                    break;

                var bounds = letterBounds[index];

                bounds.SetTarget(x, y + (height - bounds.Height) / 2);

                x += bounds.Width;

            }

            y += height;
        }

        #endregion

        #region Move to center

        foreach (var letterBound in letterBounds)
            letterBound.TargetY += -y / 2 - 2;

        #endregion

        if (Application.isPlaying)
        {
            // Get target bound
            Bounds targetBounds = letterBounds[0].GetBounds();

            for (int i = 1; i < letterBounds.Count; i++)
                letterBounds[i].AddBounds(ref targetBounds);

            StartCoroutine(Singleton.Instance.CameraController.FocusToBound(targetBounds));

            yield return MsgAnimation.RunAnimation(
                1f,
                (v) =>
                {
                    foreach (var letterBound in letterBounds)
                        letterBound.MoveTowardTarget(v);
                });
        }
        else
            foreach (var letterBound in letterBounds)
                letterBound.MoveTowardTarget(1f);
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
