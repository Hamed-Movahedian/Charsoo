﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordSetValidator : BaseObject
{
    private Dictionary<char, List<Letter>> _charToLetter;
    private List<List<Letter>> _partitions;
    private Dictionary<Letter, List<Letter>> _letterToPartition;
    private string _word;
    private List<List<Letter>> _usedPartitions;
    private Direction _direction;
    private int _foundSequence;

    // Allow next partition up/down/left/right
    private bool _allowUp, _allowDown, _allowRight, _allowLeft;

    // for test only
    private List<string> _logList;

    #region ValidateWordSet - Validate current partitioned word set

    [ContextMenu("Validate")]
    void ValidateWordSet()
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
            _direction = Direction.Horizontal;
            FindSequence(0);


            _foundSequence = 0;
            _direction = Direction.Vertical;
            FindSequence(0);

            // for test only
            if(_logList.Count>0)
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
            #region Setup direction permission for first partition direction

            // Setup direction permission for next partition direction
            if (_direction == Direction.Horizontal)
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

            #endregion

            int j = ProcessLetter(letter, i);

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

    private int ProcessLetter(Letter letter, int i)
    {
        var partition = _letterToPartition[letter];

        // This partition already used!
        if (_usedPartitions.Contains(partition))
            return -1;

        #region Check direction permission for this partition

        if (!_allowUp && letter.UpLetter != null)
            return -1;

        if (!_allowDown && letter.DownLetter != null)
            return -1;

        if (!_allowLeft && letter.LeftLetter != null)
            return -1;

        if (!_allowRight && letter.RightLetter != null)
            return -1;
        
        #endregion

        while (true)
        {
            // Go to Next letter & next character
            var nextLetter = NextLetter(letter);
            i++;

            // Sequence successfully end
            if (nextLetter == null)
                break;

            // Letter sequence continues but word ends!
            if (i== _word.Length)
                return -1;

            // next letter dost match with next char
            if (nextLetter.Char != _word[i])
                return -1;

            letter = nextLetter;
        }
        // ******** Successfully use partition

        // add partition to used partitions
        _usedPartitions.Add(partition);

        #region Setup direction permission for next partition direction

        // Setup direction permission for next partition direction
        if (_direction==Direction.Horizontal)
        {
            _allowUp = (letter.UpLetter == null);
            _allowDown = (letter.DownLetter == null);
            _allowRight = false;
            _allowLeft = true;
        }
        else
        {
            _allowRight = (letter.RightLetter == null);
            _allowLeft = (letter.LeftLetter == null);
            _allowUp = false;
            _allowDown = true;
        }
        
        #endregion

        // return new index
        return i;
        
    }

    #endregion

    #region Next/Previous letter
    private Letter NextLetter(Letter letter)
    {
        if (_direction == Direction.Horizontal)
            return letter.LeftLetter;
        else
            return letter.DownLetter;
    }

    private object PreLetter(Letter letter)
    {
        if (_direction == Direction.Horizontal)
            return letter.RightLetter;
        else
            return letter.UpLetter;
    }
    #endregion

    #region SequenceFound
    private void SequenceFound()
    {
        _foundSequence++;

        string s = _word + " " + (_direction == Direction.Horizontal ? "H" : "V") + " ";

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
