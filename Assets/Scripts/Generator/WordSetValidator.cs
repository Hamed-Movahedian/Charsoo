using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordSetValidator : BaseObject
{
    private Dictionary<char, List<Letter>> _charToLetter;
    private Dictionary<Letter, List<Letter>> _letterToPartition;
    private string _word;
    private List<List<Letter>> _usedPartitions;

    #region ValidateWordSet - Validate current partitioned word set

    [ContextMenu("Validate")]
    void ValidateWordSet()
    {
        var wordComponents = WordManager.GetComponentsInChildren<Word>();

        var words = wordComponents
            .Select(wc => wc.Name)
            .ToList();

        var partitions = GetComponent<Partioner>().Paritions;

        #region initialize LetterToPartition dic

        // **************** initialize LetterToPartition dic
        _letterToPartition = new Dictionary<Letter, List<Letter>>();

        foreach (var partition in partitions)
            foreach (Letter letter in partition)
                _letterToPartition.Add(letter, partition);
        
        #endregion

        #region Initialize CharToLetter Dic

        _charToLetter= new Dictionary<char, List<Letter>>();
        LetterController.AllLetters.ForEach(l =>
        {
            if(!_charToLetter.ContainsKey(l.Char))
                _charToLetter.Add(l.Char,new List<Letter>());
            _charToLetter[l.Char].Add(l);
        });

        #endregion

        #region Initialize UsedPartitions

        _usedPartitions=new List<List<Letter>>();

        #endregion

        #region check whole word in one partition

        // check whole word in one partition
        foreach (Word word in wordComponents)
        {
            var partition = _letterToPartition[word.Letters[0]];

            if (partition.Intersect(word.Letters).ToList().Count == word.Letters.Count)
            {
                Debug.LogError("word "+word.Name+" is in one partition !!!!");
                return ;
            }
        }

        #endregion

        #region Find sequences for words

        foreach (var word in words)
        {
            _word = word;
            FindSequence(0);
        }

        #endregion


    }

    #endregion

    #region FindSequence

    private void FindSequence(int i)
    {
        foreach (Letter letter in _charToLetter[_word[i]])
        {
            int j = ProcessLetter(letter, i);

            if (j >= 0)
            {
                if (_word[j] == null)
                    SequenceFound();
                else
                    FindSequence(j);

                _usedPartitions.Remove(_letterToPartition[letter]);
            }
        }
    }

    #endregion

    #region ProcessLetter

    private int ProcessLetter(Letter letter, int i)
    {
        var partition = _letterToPartition[letter];

        // This partition already used!
        if (_usedPartitions.Contains(partition))
            return -1;

        // Before first letter must be empty
        if (PreLetter(letter) != null)
            return -1;

        while (true)
        {
            // Go to Next letter & next character
            letter = NextLetter(letter);
            i++;

            // Sequence successfully end
            if (letter == null)
                break;
        }
    }
    

    #endregion
}
