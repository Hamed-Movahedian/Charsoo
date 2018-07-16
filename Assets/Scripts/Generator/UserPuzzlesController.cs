using System;
using System.Collections.Generic;
using MgsCommonLib;
using UnityEngine;

internal class UserPuzzlesController : MgsSingleton<UserPuzzlesController>
{
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
}