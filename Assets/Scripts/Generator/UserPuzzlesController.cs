using System;
using System.Collections;
using System.Collections.Generic;
using MgsCommonLib;
using UnityEngine;

internal class UserPuzzlesController : MgsSingleton<UserPuzzlesController>
{
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
        // Cache
        var upLocalDB = LocalDBController.Instance.UserPuzzles;
        var upUI = UIController.Instance.UserPuzzles;

        // Sync with server - if possible
        yield return _sync.Sync();

        // Get user puzzles from local 
        var userPuzzles = upLocalDB.GetUserPuzzles();

        // Initialize user puzzle selection window
        upUI.InitializeUserPuzzleSelectinWindow(userPuzzles);

        ShowUserPuzzles:
        // Show user puzzle selection window 
        yield return upUI.ShowPuzzleSelectionWindow();

        // if user select back return
        if (upUI.Back)
            yield break;

        // Get selected puzzle 
        var selectedPuzzle = upUI.GetSelectedPuzzle();

        // Show selected puzzle info
        yield return upUI.ShowPuzzleInfo(selectedPuzzle);

        // if back => goto last window
        if (upUI.Back) goto ShowUserPuzzles;

        // switch all other results
        switch (upUI.Result)
        {
            case UserPuzzleUI.ResultEnum.Play:
                yield return GameController.Instance.PlayPuzzle(selectedPuzzle);
                break;

            case UserPuzzleUI.ResultEnum.Share:
                yield return ShareController.Instance.ShareUserPuzzle(selectedPuzzle);
                break;

            case UserPuzzleUI.ResultEnum.Register:
                // Sync
                yield return _sync.Sync();

                // Show sync result
                yield return upUI.ShowSyncResult(_sync.Successfull);
                break;

            case UserPuzzleUI.ResultEnum.Add:

                break;
        }

        goto ShowUserPuzzles;

    }

}

#region Interfaces

public interface IUpdatedUserPuzzle
{
    int ServerID { get; set; }
    string CategoryName { get; set; }
    int? Rate { get; set; }
    int? PlayCount { get; set; }
}
public interface IRegisterPuzzleInfo
{
    int ServerID { get; set; }
    int ID { get; set; }
}

#endregion