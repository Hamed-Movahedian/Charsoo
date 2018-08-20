using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using MgsCommonLib;
using MgsCommonLib.Theme;
using MgsCommonLib.UI;
using UnityEngine;

internal class UserPuzzlesController : MgsSingleton<UserPuzzlesController>
{
    public UserPuzzleSelectionWindow UserPuzzlesSelectionWindow;
    public UserPuzzleInfoWindow PuzzleInfoWindow;

    private readonly UserPuzzleSynchronizer _sync = new UserPuzzleSynchronizer();

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

    public IEnumerator Sync()
    {
        yield return  _sync.Sync();

        if(_sync.Successfull)
            FollowMachine.SetOutput("Success");
        else
            FollowMachine.SetOutput("Fail");
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
                        ThemeManager.Instance.LanguagePack.GetLable("SuccesfullOperation"));
                }
                else
                    yield return UIController.Instance.DisplayError(
                        ThemeManager.Instance.LanguagePack.GetLable("Error_InternetAccess"),
                        ThemeManager.Instance.IconPack.GetIcon("NetworkError"));

                goto WaitForPuzzleSelectionWindow;
            case "Add":
                // Hide last window
                yield return UserPuzzlesSelectionWindow.Hide();

                // gnereate new puzzle
                yield return RuntimeWordSetGenerator.Instance.StartProcess();

                // Sync
                yield return _sync.Sync();

                // refresh window
                UserPuzzlesSelectionWindow.Refresh();

                // Show last window
                yield return UserPuzzlesSelectionWindow.Show();

                // back to waiting
                goto WaitForPuzzleSelectionWindow;
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
                        ThemeManager.Instance.LanguagePack.GetLable("SuccesfullOperation"));
                }
                else
                    yield return UIController.Instance.DisplayError(
                        ThemeManager.Instance.LanguagePack.GetLable("Error_InternetAccess"),
                        ThemeManager.Instance.IconPack.GetIcon("NetworkError"));

                break;
        }

        goto puzzleInfo;
    }

}

