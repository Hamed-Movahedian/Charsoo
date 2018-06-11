using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordSetValidator1 : BaseObject
{
    private Dictionary<char, List<Letter>> _charToLetter;
    private List<List<Letter>> _partitions;
    private Dictionary<Letter, List<Letter>> _letterToPartition;
    private string _word;
    private List<List<Letter>> _usedPartitions;
    private WordDirection _wordDirection;
    private int _foundSequence;

    private Letter[] _wordLetters = new Letter[30];


    // Allow next partition up/down/left/right
    private bool _allowUp, _allowDown, _allowRight, _allowLeft;

    // for test only
    private List<string> _logList;

    #region ValidateWordSet - Validate current partitioned word set

    [ContextMenu("Validate")]
    public void ValidateWordSet()
    {

        #region initialize

        var wordComponents = WordManager.GetComponentsInChildren<Word>();

        var words = wordComponents
            .Select(wc => wc.Name)
            .ToList();

        _partitions = GetComponent<Partioner>().Paritions;

        #region LetterToPartition dic

        // **************** initialize LetterToPartition dic
        _letterToPartition = new Dictionary<Letter, List<Letter>>();

        foreach (var partition in _partitions)
            foreach (Letter letter in partition)
                _letterToPartition.Add(letter, partition);

        #endregion

        #region CharToLetter Dic

        _charToLetter = new Dictionary<char, List<Letter>>();
        LetterController.AllLetters.ForEach(l =>
        {
            if (!_charToLetter.ContainsKey(l.Char))
                _charToLetter.Add(l.Char, new List<Letter>());
            _charToLetter[l.Char].Add(l);
        });

        #endregion

        
        _usedPartitions = new List<List<Letter>>();

        // Setup log string list for TEST
        _logList = new List<string>();

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

            // for test only
            _logList.Clear();

            _foundSequence = 0;
            _wordDirection = WordDirection.Horizontal;
            FindSequence(0);


            _foundSequence = 0;
            _wordDirection = WordDirection.Vertical;
            FindSequence(0);

            // for test only
            if(_logList.Count>1)
                _logList.ForEach(Debug.Log);
        }

        #endregion


    }

    #endregion

    #region FindSequence

    private void FindSequence(int i)
    {
        foreach (Letter letter in _charToLetter[_word[i]])
        {
            _wordLetters[i] = letter;

            int j = ProcessLetter(i);

            if (j >= 0)
            {
                if (j == _word.Length)
                    SequenceFound();
                else
                    FindSequence(j);

                _usedPartitions.Remove(_letterToPartition[letter]);
            }
        }
    }

    #endregion

    #region ProcessLetter

    private int ProcessLetter(int i)
    {
        Letter firstLetter = _wordLetters[i];
        var partition = _letterToPartition[firstLetter];

        // This partition already used!
        if (_usedPartitions.Contains(partition))
            return -1;

        #region Check direction permission for this partition

        SetDirectionPermissions(i);

        if (!_allowUp && firstLetter.UpLetter != null)
            return -1;

        if (!_allowDown && firstLetter.DownLetter != null)
            return -1;

        if (!_allowLeft && firstLetter.LeftLetter != null)
            return -1;

        if (!_allowRight && firstLetter.RightLetter != null)
            return -1;
        
        #endregion

        while (true)
        {
            // Go to Next letter & next character
            i++;
            _wordLetters[i] = NextLetter(_wordLetters[i-1]);

            // Sequence successfully end
            if (_wordLetters[i] == null)
                break;

            // Letter sequence continues but word ends!
            if (i== _word.Length)
                return -1;

            // next letter dost match with next char
            if (_wordLetters[i].Char != _word[i])
                return -1;
        }
        // ******** Successfully end letter sequence

        // add partition to used partitions
        _usedPartitions.Add(partition);

        // return new index
        return i;
        
    }

    private void SetDirectionPermissions(int i)
    {
        if (i > 0)
        {
            var lastLetter = _wordLetters[i - 1];
            if (_wordDirection == WordDirection.Horizontal)
            {
                _allowUp = lastLetter.UpLetter == null;
                _allowDown = lastLetter.DownLetter == null;
                _allowRight = false;
                _allowLeft = true;
            }
            else
            {
                _allowRight = lastLetter.RightLetter == null;
                _allowLeft = lastLetter.LeftLetter == null;
                _allowUp = false;
                _allowDown = true;
            }
        }
        else if (_wordDirection == WordDirection.Horizontal)
        {
            _allowUp = true;
            _allowDown = true;
            _allowRight = false;
            _allowLeft = true;
        }
        else
        {
            _allowRight = true;
            _allowLeft = true;
            _allowUp = false;
            _allowDown = true;
        }
    }

    #endregion

    #region Next/Previous letter
    private Letter NextLetter(Letter letter)
    {
        if (_wordDirection == WordDirection.Horizontal)
            return letter.LeftLetter;
        else
            return letter.DownLetter;
    }

    private object PreLetter(Letter letter)
    {
        if (_wordDirection == WordDirection.Horizontal)
            return letter.RightLetter;
        else
            return letter.UpLetter;
    }
    #endregion

    #region SequenceFound
    private void SequenceFound()
    {
        _foundSequence++;

        string s = ArabicSupport.ArabicFixer.Fix(_word) + " " + (_wordDirection == WordDirection.Horizontal ? "H" : "V") + " ";

        _usedPartitions.ForEach(p => s += PartitionIndex(p) + ", ");

        _logList.Add(s);
    }

    private string PartitionIndex(List<Letter> p)
    {
        for (int i = 0; i < _partitions.Count; i++)
        {
            if (_partitions[i] == p)
                return i.ToString();
        }
        return "-1";
    }
    #endregion
    
}
