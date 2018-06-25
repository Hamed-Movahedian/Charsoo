using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib.Utilities;
using UnityEngine;

public class WordSetValidator
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

    private Word[] _wordComponents;
    private List<string> _words;
    public bool IsValid;

    #region ValidateWordSet - Validate current partitioned word set

    public IEnumerator ValidateWordSet(List<List<Letter>> partitions)
    {
        MgsCoroutine.Info += "\n Validating...";

        #region initialize

        IsValid = false;

        _partitions = partitions;

        #region LetterToPartition dic

        // **************** initialize LetterToPartition dic
        if (_letterToPartition == null)
            _letterToPartition = new Dictionary<Letter, List<Letter>>();
        else
            _letterToPartition.Clear();

        foreach (var partition in _partitions)
            foreach (Letter letter in partition)
                _letterToPartition.Add(letter, partition);

        #endregion

        _usedPartitions.Clear();

        #endregion

        #region check whole word in one partition

        // check whole word in one partition
        foreach (Word word in _wordComponents)
        {
            var partition = _letterToPartition[word.Letters[0]];

            if (partition.Intersect(word.Letters).ToList().Count == word.Letters.Count)
                yield break;
        }

        #endregion

        #region Check for word with more than one sequence

        foreach (var word in _words)
        {
            _word = word;

            _foundSequence = 0;

            _wordDirection = WordDirection.Horizontal;
            yield return FindSequence(0);


            _wordDirection = WordDirection.Vertical;
            yield return FindSequence(0);

            if (_foundSequence != 1)
                yield break;

        }

        #endregion

        IsValid = true;
    }

    #endregion

    #region FindSequence

    private IEnumerator FindSequence(int i)
    {
        foreach (Letter letter in _charToLetter[_word[i]])
        {
            _wordLetters[i] = letter;

            int j = ProcessLetter(i);

            if (j >= 0)
            {
                if (j == _word.Length)
                    _foundSequence++;
                else
                    yield return FindSequence(j);

                _usedPartitions.Remove(_letterToPartition[letter]);
            }
            else
                yield return null;
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
            _wordLetters[i] = NextLetter(_wordLetters[i - 1]);

            // Sequence successfully end
            if (_wordLetters[i] == null)
                break;

            // Letter sequence continues but word ends!
            if (i == _word.Length)
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
    
    #region Initialize

    public void Initialize(Partioner partitioner)
    {
        _wordComponents = partitioner.WordManager.GetComponentsInChildren<Word>();

        _words = _wordComponents
            .Select(wc => wc.Name)
            .ToList();

        #region CharToLetter Dic

        _charToLetter = new Dictionary<char, List<Letter>>();

        partitioner.LetterController.AllLetters.ForEach(l =>
        {
            if (!_charToLetter.ContainsKey(l.Char))
                _charToLetter.Add(l.Char, new List<Letter>());
            _charToLetter[l.Char].Add(l);
        });

        #endregion

        _usedPartitions = new List<List<Letter>>();
    }
    

    #endregion
}
