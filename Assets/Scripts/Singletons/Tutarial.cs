using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.Animation;
using UnityEngine;

public class Tutarial : MonoBehaviour
{
    private int _puzzleIndex = 0;
    private Puzzle _puzzle;
    public HUD HUD;
    public int CurrentWord = 0;
    private bool _break = false;
    private List<Vector3> _letterPos;


    [FollowMachine("Prepare next puzzle for spawn", "Play Next,No Next Puzzle")]
    public void SetPuzzle()
    {
        _puzzle = LocalDBController
            .Table<Puzzle>()
            .FirstOrDefault(p => p.CategoryID == 2044 && p.Row == _puzzleIndex);
        if (_puzzle==null)
        {
            FollowMachine.SetOutput("No Next Puzzle");
            return;
        }
        
        SetForSpawn(_puzzle);

        _puzzleIndex++;

        if (Singleton.Instance.WordSpawner.PuzzleReward)
            ZPlayerPrefs.SetInt("LastPlayedPuzzle", _puzzle.ID);

        FollowMachine.SetOutput("Play Next");

    }

    [FollowMachine("Prepare next puzzle for spawn", "Play Next,No Next Puzzle")]
    public void PuzzleSolved()
    {


        var puzzles = LocalDBController.Table<Puzzle>().
            SqlWhere(p => p.CategoryID == _puzzle.CategoryID);

        Puzzle nextPuzzle = puzzles.FirstOrDefault(p => p.Row == _puzzleIndex);

        if (nextPuzzle == null || nextPuzzle.Solved)
        {
            FollowMachine.SetOutput("No Next Puzzle");
            return;
        }

        FollowMachine.SetOutput("Play Next");
    }
    
    public void SetForSpawn(Puzzle selectedPuzzle)
    {
        WordSet wordSet = selectedPuzzle.GetWordSet();
        Singleton.Instance.WordSpawner.ClearTable();
        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.Clue = selectedPuzzle.Clue;
        Singleton.Instance.WordSpawner.PuzzleRow = (selectedPuzzle.Row + 1).ToString();
        Singleton.Instance.WordSpawner.PuzzleReward = !selectedPuzzle.Solved;
        Singleton.Instance.WordSpawner.PuzzleID = selectedPuzzle.ID;

        Singleton.Instance.WordSpawner.EditorInstatiate = null;
    }

    public void SetupHUD()
    {
        HUD.StartClueText.text = _puzzleIndex == 0 ? PersianFixer.Fix("به چارسو \n خوش آمدید") : PersianFixer.Fix(_puzzle.Clue);
        HUD.HeaderClueText.text = PersianFixer.Fix(_puzzle.Clue);
        HUD.HintClueText.text = PersianFixer.Fix(_puzzle.Clue);
        HUD.IndexText.text = PersianFixer.Fix((_puzzle.Row + 1).ToString(), true, true);
    }


    [FollowMachine("Check Word Index", "Break,NextWord,Continue")]
    public void CheckWord()
    {
        if (_puzzleIndex >1)
        {
            FollowMachine.SetOutput("Continue");
            return;
        }

        Word currentWord = Singleton
            .Instance
            .WordManager
            .Words[CurrentWord];
        Debug.Log(currentWord.Name);

        bool correctWord = currentWord
            .IsComplete;

        if (_break || !correctWord)
        {
            FollowMachine.SetOutput("Break");
            _break = true;
            return;
        }
        CurrentWord++;
        FollowMachine.SetOutput("NextWord");
    }

    public void SaveLetterPositions()
    {
        _letterPos = Singleton.Instance.LetterController.AllLetters
            .Select(l => l.transform.position)
            .ToList();
    }

    public IEnumerator ResetLettersToSaveLocaion(float duration)
    {

        var letters = Singleton.Instance.LetterController.AllLetters;

        var startPos = letters.Select(l=>l.transform.position).ToList();

        yield return MsgAnimation.RunAnimation(
            duration,
            v =>
            {
                for (int i = 0; i < letters.Count; i++)
                {
                    letters[i].transform.position = Vector3.Lerp(startPos[i], _letterPos[i], v);
                }
            });
    }

    [FollowMachine("Check Tutorial Index", "0,1,2,Other")]
    public void TutorialState()
    {
        FollowMachine.SetOutput(_puzzleIndex<4?(_puzzleIndex - 1).ToString():"Other");
    }
}
