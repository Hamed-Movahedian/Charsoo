using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class Solver : BaseObject
{
    public Word Word;
    public bool AllowFourSquare = true;
    public Func<double> GetTime;

    private List<List<Letter>> _seqList;
    private List<Letter> _curSeq;
    public int NextSeq = 0;
    private Direction _direction;

    private List<Vector3> _letterOriginalPositions;

    public string ProgressTitle;
    private double _lastTime;
    private bool _cancel;


    public void GenerateSequences(Word word)
    {

        NextSeq = 0;
        _direction = Direction.Vertical;

        if (_seqList == null)
            _seqList = new List<List<Letter>>();
        _seqList.Clear();

        if (_curSeq == null)
            _curSeq = new List<Letter>();
        _curSeq.Clear();

        ProcessNextLetter(word, 0);

    }

    private void ProcessNextLetter(Word word, int index)
    {
        if (index >= word.name.Length)
        {
            _seqList.Add(new List<Letter>(_curSeq));
            return;
        }

        foreach (Letter letter in LetterController
                                        .AllLetters
                                        .Where(l =>
                                                    l.GetCharacter() == word.name[index] &&
                                                    !_curSeq.Contains(l)))
        {
            _curSeq.Add(letter);
            ProcessNextLetter(word, index + 1);
            _curSeq.Remove(letter);
        }
    }

    public int PlaceNextSequence(bool countAll, float percent=0, Func<string, float, bool> showProgressBar = null)
    {
        if (_seqList == null)
            return 0;

        if (_seqList.Count == 0)
            return 0;

        int correctStateCount = 0;

        int count = 0;

        // place each letter
        while (true)
        {
            bool succes = PlaceLetters();

            // prepare next seq
            if (_direction == Direction.Vertical)
                _direction = Direction.Horizontal;
            else
            {
                _direction = Direction.Vertical;
                NextSeq++;
                if (NextSeq >= _seqList.Count)
                    NextSeq = 0;
            }

            if (succes)
                if (countAll)
                    correctStateCount++;
                else
                    return 0;

            if (++count >= _seqList.Count * 2)
            {
                return correctStateCount;
            }
            // Show Progress Bar
            if (showProgressBar != null)
                if (GetTime() - _lastTime > .2)
                {
                    if (!showProgressBar(ProgressTitle + " validating ...", count * percent / ((float)_seqList.Count * 2)))
                    {
                        _cancel = true;
                        return 100;
                    }
                    _lastTime = GetTime();
                }
            
        }

    }

    private bool PlaceLetters()
    {

        //print(NextSeq);
        // disable all
        LetterController.AllLetters.ForEach(l => l.Active=false);

        // place letters
        int index = 0;
        foreach (Letter l in _seqList[NextSeq])
            if (!PlaceLetter(l, _direction == Direction.Vertical ? new Vector2(0, index--) : new Vector2(index--, 0)))
                return false;

        // active letters
        List<Letter> placedLetters = LetterController.AllLetters
            .Where(l => l.Active).ToList();

        List<Vector3> positionList = placedLetters.Select(l => l.transform.position).ToList();


        // four adjacent letters
        if (!AllowFourSquare)
            foreach (Letter placedLetter in placedLetters)
            {
                Vector3 pos = placedLetter.transform.position;
                if (positionList
                        .Count(p =>
                            p == pos + Vector3.up ||
                            p == pos + Vector3.right ||
                            p == pos + new Vector3(1, 1, 0)) == 3)
                    return false;
            }

        // no two letters in same position
        for (int i = 0; i < placedLetters.Count; i++)
            for (int j = i + 1; j < placedLetters.Count; j++)
                if (placedLetters[i].transform.position == placedLetters[j].transform.position)
                    return false;

        // No letter before first and after last
        Vector3 beforeFirst = _direction ==
            Direction.Vertical ?
            new Vector2(0, 1) :
            new Vector2(1, 0);

        Vector3 afterLast = _direction ==
            Direction.Vertical ?
            new Vector2(0, -_seqList[NextSeq].Count) :
            new Vector2(-_seqList[NextSeq].Count, 0);

        if (placedLetters
                .Select(l => l.transform.position)
                .Any(p => p == beforeFirst || p == afterLast))
            return false;

        return true;
    }

    private bool PlaceLetter(Letter letter, Vector3 pos)
    {
        if (letter.Active)
        {
            if (letter.transform.position == pos)
                return true;

            return false;
        }
        // delta
        Vector3 delta = pos - letter.transform.position;

        // get connected letters
        List<Letter> connectedLetters = new List<Letter>();
        letter.GetConnectedLetters(connectedLetters);

        connectedLetters.ForEach(l =>
        {
            l.Active=true;
            l.transform.position += delta;
        });

        return true;
    }

    public bool Validate(int eCount, Func<string, float, bool> showProgressBar , ref bool cancel)
    {
        _cancel =  false;
        var allWords = WordManager.GetComponentsInChildren<Word>();
        WordManager.ErrorWords.Clear();


        // check whole word in one group
        foreach (Word word in allWords)
        {
            List<Letter> conectedLetters=new List<Letter>();
            word.Letters[0].GetConnectedLetters(conectedLetters);

            if (conectedLetters.Intersect(word.Letters).ToList().Count == word.Letters.Count)
                return false;
        }

        // check more than one state for each word
        _letterOriginalPositions = LetterController.AllLetters.Select(l => l.transform.position).ToList();
        
        bool result= true;

        for (int i = 0; i < allWords.Length; i++)
        {
            
            GenerateSequences(allWords[i]);

            if (PlaceNextSequence(true, (i+1)/(float)allWords.Length,showProgressBar) > 1)
            {
                if (_cancel)
                    cancel = true;

                if (eCount<=0 || _cancel)
                {
                    WordManager.ErrorWords.Clear();
                    result = false;
                    break;
                    
                }
                WordManager.ErrorWords.Add(allWords[i]);
                eCount--;
            }
        }

        // restore to default state
        for (int i = 0; i < LetterController.AllLetters.Count; i++)
        {
            LetterController.AllLetters[i].transform.position = _letterOriginalPositions[i];
            LetterController.AllLetters[i].Active=true;
        }

        return result;
    }
}
