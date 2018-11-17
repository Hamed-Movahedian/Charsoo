using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;

public class Tutarial : MonoBehaviour
{
    private int _puzzleIndex = 0;
    private Puzzle _puzzle;
    public HUD HUD;

    public void SetPuzzle()
    {
        _puzzle = LocalDBController
            .Table<Puzzle>()
            .SqlWhere(p => p.CategoryID == 2044 && p.Row == _puzzleIndex)
            .Single();

        SetForSpawn(_puzzle);
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
        HUD.StartClueText.text = _puzzleIndex == 0? PersianFixer.Fix("به چارسو \n خوش آمدید"): PersianFixer.Fix(_puzzle.Clue);
        HUD.HeaderClueText.text = PersianFixer.Fix(_puzzle.Clue);
        HUD.HintClueText.text = PersianFixer.Fix(_puzzle.Clue);
        HUD.IndexText.text = PersianFixer.Fix((_puzzle.Row+1).ToString(),true,true);
    }


    [FollowMachine("Check Tutorial Index","0,1,2")]
    public void TutorialState()
    {
        FollowMachine.SetOutput(_puzzleIndex.ToString());
    }
}
