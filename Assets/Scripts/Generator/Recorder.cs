using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Recorder : BaseObject
{
    public string Clue = "No title";
    public CategoryComponent Category;
	// Use this for initialization
    public WordSet Save()
    {
        
        WordSet wordSet = new WordSet();

        wordSet.Clue = Clue;
        wordSet.Words = new List<SWord>();

        foreach (var word in WordManager.GetComponentsInChildren<Word>())
        {
            wordSet.Words.Add(new SWord(word));
        }

        return wordSet;
    }
}
