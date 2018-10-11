using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class WordSetGenerator : BaseObject
{
    #region Fields

    public int UsedWordCount = 4;
    public bool BruteForce = false;
    public int MaxResults = 15000;

    [TextArea]
    public string AllWords;

    private List<string> _wordStrings;

    private int _foundResultCount = 0;

    private List<SWord> _words = new List<SWord>();
    private List<SWord> _usedWords = new List<SWord>();
    private WordSet _wordSet;
    private CommonLettersDictionary _clDictionary;
    public Func<Letter, Letter> EditorInstantiate;
    public string Clue;
    private List<SWord> _bestResult;
    public Color PartitionGizmoColor;

    #endregion

    #region MakeWordSet

    [ContextMenu("MakeWordSet")]
    public IEnumerator MakeWordSet()
    {
        Initialize();

        SWord word = null;
        int index = 0;

        var startTime = Time.time;

        while (_foundResultCount < MaxResults && Time.time < startTime + _words.Count * 2 )
        {
            MgsCoroutine.Info = _foundResultCount + " word set found.";
            if (BruteForce)
            {
                if (index < _words.Count)
                    word = _words[index++];
                else
                    break;
            }
            else
                word = _words[Random.Range(0, _words.Count)];

            word.X = 0;
            word.Y = 0;
            word.WordDirection = WordDirection.Horizontal;

            yield return TryOtherWords(word);
        }

        if (_foundResultCount == 0)
        {
            Successful = false;
            yield break;
        }

        
        print(_foundResultCount + " WordSet Found");
        Successful = true;

    }

    public void Initialize()
    {
        // Clear last partitions

        _wordStrings = AllWords
            .Replace("‌", "")
            .Split(' ', '\n', '\t', '-', '–')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0 && s[0] != '/')
            .Distinct()
            .ToList();

        // Clear results
        _foundResultCount = 0;
        _bestResult = null;

        // Initialize 
        _clDictionary = new CommonLettersDictionary(_wordStrings);


        // ** Setup words
        if (_words == null)
            _words = new List<SWord>();
        if (_usedWords == null)
            _usedWords = new List<SWord>();

        _words.Clear();
        _usedWords.Clear();

        foreach (string wordString in _wordStrings)
            _words.Add(new SWord { Name = wordString });

        MgsCoroutine.Title = "Generate Word set";
    }

    public bool Successful { get; set; }

    private IEnumerator TryOtherWords(SWord lastWord)
    {
        // Show Progress Bar
        MgsCoroutine.Info = _foundResultCount.ToString("N0") + " WordSet Found";
        MgsCoroutine.Percentage = _foundResultCount / (float)MaxResults;

        yield return null;

        // Safe Guard
        if (_foundResultCount > MaxResults)
            yield break;


        // Use last word
        _usedWords.Add(lastWord);

        if (_usedWords.Count >= UsedWordCount)
            // FIND A CORRECT PUZZLE !!!
            ResultFound();
        else
        {
            List<SWord> _unusedWords = _words.Where(w => _usedWords.Find(uw => uw.Name == w.Name) == null).ToList();

            int index = 0;
            SWord word = null;

            while (_unusedWords.Count > 0)
            {
                if (!BruteForce)
                {
                    index = Random.Range(0, _unusedWords.Count);
                    word = _unusedWords[index];
                    _unusedWords.RemoveAt(index);

                }
                else
                {
                    if (index < _unusedWords.Count)
                        word = _unusedWords[index++];
                    else
                        break;

                }

                if (_usedWords.Find(w => w.Name == word.Name) == null)
                {
                    List<SWord> placements = GetWordPlacements(word);

                    foreach (var w in placements)
                        yield return TryOtherWords(w);
                }
            }
        }

        // Unused last word
        _usedWords.Remove(lastWord);
    }

    private List<SWord> GetWordPlacements(SWord newWord)
    {
        List<SWord> placements = new List<SWord>();

        foreach (SWord usedWord in _usedWords)
            foreach (Vector2 location in _clDictionary[usedWord.Name][newWord.Name])
            {
                if (usedWord.WordDirection == WordDirection.Horizontal)
                {
                    newWord.WordDirection = WordDirection.Vertical;
                    newWord.X = (int)(usedWord.X - location.x);
                    newWord.Y = (int)(usedWord.Y + location.y);
                }
                else
                {
                    newWord.WordDirection = WordDirection.Horizontal;
                    newWord.X = (int)(usedWord.X + location.y);
                    newWord.Y = (int)(usedWord.Y - location.x);
                }

                int matchCount = PlaceWord(newWord);

                if (matchCount > 0)
                    placements.Add(new SWord(newWord, matchCount));
            }

        return placements;
    }

    #endregion

    #region Fitness Function

    private int Fitness(List<SWord> result)
    {
        int count = 0;

        for (int i = 0; i < result.Count; i++)
            for (int j = i + 1; j < result.Count; j++)
                if (IsWordsOverlap(result[i], result[j]))
                    if (IsCorrectOverlap(result[i], result[j]))
                        count++;

        return count;
    }
    private int Fitness4(List<SWord> result)
    {
        // Get all letters
        List<Vector2> lLocations = new List<Vector2>();

        result.ForEach(w =>
        {
            for (int i = 0; i < w.Name.Length; i++)
                lLocations.Add(w.Locations(i));
        });

        int count = 0;

        for (int i = 0; i < lLocations.Count; i++)
            for (int j = i + 1; j < lLocations.Count; j++)
                if (IsAdjacent(lLocations[i], lLocations[j]))
                    count++;

        return count;
    }

    private bool IsAdjacent(Vector2 p1, Vector2 p2)
    {
        if (p1.x == p2.x && (p1.y == p2.y + 1 || p1.y + 1 == p2.y))
            return true;

        if (p1.y == p2.y && (p1.x == p2.x + 1 || p1.x + 1 == p2.x))
            return true;

        return false;
    }

    private int Fitness2(List<SWord> result)
    {
        Bounds b = new Bounds(result[0].Max, Vector3.zero);

        result.ForEach(w =>
        {
            b.Encapsulate(w.Max);
            b.Encapsulate(w.Min);
        });

        return (int)(1000 - Mathf.Abs(b.size.x - b.size.y) + (500 - b.size.x - b.size.y));
    }
    private float Fitness3(List<SWord> result)
    {
        Bounds b = new Bounds(result[0].Max, Vector3.zero);

        result.ForEach(w =>
        {
            b.Encapsulate(w.Max);
            b.Encapsulate(w.Min);
        });
        int hCount = result.Count(w => w.WordDirection == WordDirection.Horizontal);
        int vCount = result.Count - hCount;

        return
            1 / (b.size.x * b.size.y) +
            1 / Mathf.Max(1, Mathf.Abs(b.size.x - b.size.y)) +
            1f / Mathf.Max(1, Mathf.Abs(hCount - vCount));
    }

    #endregion

    #region Result Found

    private void ResultFound()
    {
        _foundResultCount++;

        if (_bestResult == null)
            _bestResult = new List<SWord>(_usedWords);
        else if (Fitness4(_usedWords) > Fitness4(_bestResult))
            _bestResult = new List<SWord>(_usedWords);
    }

    #endregion

    #region Safe LocationWraper

    private int PlaceWord(SWord newWord)
    {
        int count = 0;

        foreach (SWord usedWord in _usedWords)
            if (IsWordsOverlap(usedWord, newWord))
                if (IsCorrectOverlap(usedWord, newWord))
                    count++;
                else
                    return -1;

        return count;
    }

    private bool IsCorrectOverlap(SWord w1, SWord w2)
    {
        if (w1.WordDirection == w2.WordDirection)
            return false;

        int i1, i2;

        if (w1.WordDirection == WordDirection.Horizontal)
        {
            i1 = w1.X - w2.X;
            i2 = w2.Y - w1.Y;
        }
        else
        {
            i1 = w1.Y - w2.Y;
            i2 = w2.X - w1.X;
        }

        if (i1 < 0 || i1 >= w1.Length)
            return false;

        if (i2 < 0 || i2 >= w2.Length)
            return false;

        return w1.Name[i1] == w2.Name[i2];
    }

    private bool IsWordsOverlap(SWord w1, SWord w2)
    {
        if (w1.X < w2.XMin || w2.X < w1.XMin)
            return false;

        if (w1.Y < w2.YMin || w2.Y < w1.YMin)
            return false;


        return true;
    }

    #endregion

    #region SpawnWordSet

    [ContextMenu("SpawnWordSet")]
    public void SpawnWordSet()
    {
        if (_foundResultCount == 0)
            return;

        if (_wordSet == null)
            _wordSet = new WordSet();

        _wordSet.Words = _bestResult;
        _wordSet.Clue = Clue;

        WordSpawner.WordSet = _wordSet;
        WordSpawner.EditorInstatiate = EditorInstantiate;
        WordSpawner.SpawnWords();
    }


    #endregion

    public WordSet GetBestWordSet()
    {
        if (_bestResult == null)
            throw new Exception("No word set found!!");

        if (_wordSet == null)
            _wordSet = new WordSet();

        _wordSet.Words = _bestResult;
        _wordSet.Clue = Clue;
        return _wordSet;
    }
}