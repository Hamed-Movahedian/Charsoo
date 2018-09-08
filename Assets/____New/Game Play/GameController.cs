using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib;
using UnityEditor;
using UnityEngine;

internal class GameController : MgsSingleton<GameController>
{
    public IEnumerator PlayPuzzle(IPlayablePuzzle puzzle)
    {
        throw new System.NotImplementedException();
    }

    public void SpawnWordSet(WordSet wordSet)
    {
        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.EditorInstatiate = null;
        Singleton.Instance.WordSpawner.SpawnWords();
    }

    public WordSet GetWordSet()
    {
        WordSet wordSet = new WordSet();

        wordSet.Clue = Singleton.Instance.WordSpawner.WordSet.Clue;

        wordSet.Words = new List<SWord>();

        foreach (var word in Singleton.Instance.WordManager.GetComponentsInChildren<Word>())
            wordSet.Words.Add(new SWord(word));

        return wordSet;
    }

    public void ClearWords()
    {
        // Delete all letters and words
        Singleton.Instance.LetterController.DeleteAllLetters();
        Singleton.Instance.WordManager.DeleteAllWords();

    }
}