using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib;
using MgsCommonLib.UI;
using UnityEngine;

internal class UserPuzzlesController : MgsSingleton<UserPuzzlesController>
{
    public UserPuzzleSelectionWindow UserPuzzlesSelectionWindow;
    public UserPuzzleInfoWindow PuzzleInfoWindow;

    private readonly UserPuzzleSynchronizer _sync = new UserPuzzleSynchronizer();

    public void Save()
    {
        WordSet wordSet = new WordSet();

        wordSet.Clue = UIController.Instance.Generator.GetClue();
        wordSet.Words = new List<SWord>();

        WordManager wordManager = Singleton.Instance.WordManager;

        foreach (var word in wordManager.GetComponentsInChildren<Word>())
            wordSet.Words.Add(new SWord(word));

        var puzzle = new Puzzle
        {
            ID = 1,
            CategoryID = null,
            Clue = wordSet.Clue,
            Row = 1,
            Content = StringCompressor.CompressString(JsonUtility.ToJson(wordSet)),
            Solved = false,
            Paid = false,
            LastUpdate = DateTime.Now
        };
        //LocalDBController.Table<Pu>()
    }

    public IEnumerator ShowUserPuzzles()
    {
        // Sync with server - if possible
        yield return _sync.Sync();

        // Initialize user puzzle selection window
        UserPuzzlesSelectionWindow.Refresh();

        // Show user puzzle selection window 
        yield return UserPuzzlesSelectionWindow.Show();

        WaitForPuzzleSelectionWindow:

        // wait for puzzle selectin window
        yield return UserPuzzlesSelectionWindow.WaitForClose();

        // switch selection window results
        switch (UserPuzzlesSelectionWindow.Result)
        {
            case "Back":
                yield return UserPuzzlesSelectionWindow.Hide();
                yield break;
            case "Update":
                // Sync
                yield return _sync.Sync();

                // Show sync result
                if (_sync.Successfull)
                {
                    UserPuzzlesSelectionWindow.Refresh();

                    yield return UIController.Instance.DisplayMessage(
                        ThemeManager.Instance.LanguagePack.SuccesfullOperation);
                }
                else
                    yield return UIController.Instance.DisplayError(
                        ThemeManager.Instance.LanguagePack.Error_InternetAccess,
                        ThemeManager.Instance.IconPack.NetworkError);

                goto WaitForPuzzleSelectionWindow;
            case "Add":
                break;
        }
        
        // Get selected puzzle 
        UserPuzzle selectedPuzzle = (UserPuzzle)UserPuzzlesSelectionWindow.GetSelectedItem();

        // Refresh puzzle info
        PuzzleInfoWindow.Refresh(selectedPuzzle);

        // Show selected puzzle info
        yield return PuzzleInfoWindow.Show();

        puzzleInfo:

        yield return PuzzleInfoWindow.WaitForClose();

        // switch all other results
        switch (PuzzleInfoWindow.Result)
        {
            case "Back":
                yield return PuzzleInfoWindow.Hide();
                goto WaitForPuzzleSelectionWindow;

            case "Play":
                yield return GameController.Instance.PlayPuzzle(selectedPuzzle);
                break;

            case "Share":
                yield return ShareController.Instance.ShareUserPuzzle(selectedPuzzle);
                break;

            case "Register":
                // Sync
                yield return _sync.Sync();

                // Show sync result
                if (_sync.Successfull)
                {
                    UserPuzzlesSelectionWindow.Refresh();

                    selectedPuzzle=LocalDBController.Instance.UserPuzzles.Refresh(selectedPuzzle);

                    PuzzleInfoWindow.Refresh(selectedPuzzle);

                    yield return UIController.Instance.DisplayMessage(
                        ThemeManager.Instance.LanguagePack.SuccesfullOperation);
                }
                else
                    yield return UIController.Instance.DisplayError(
                        ThemeManager.Instance.LanguagePack.Error_InternetAccess,
                        ThemeManager.Instance.IconPack.NetworkError);

                break;
        }

        goto puzzleInfo;
    }

}

