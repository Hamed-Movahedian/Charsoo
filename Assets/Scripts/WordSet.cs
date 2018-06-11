using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "WordSet", menuName = "Words/WordSet", order = 1)]
public class WordSet : ScriptableObject
{
    public List<SWord> Words;
    public string Clue;
    public List<SWord> NonuniqWords = new List<SWord>();
    [ContextMenu("Bound")]
    public Bounds GetBound()
    {
        Bounds bound = new Bounds();

        Words.ForEach(w =>
        {
            if (w.LocationList != null)
                w.LocationList.ForEach(l => bound.Encapsulate(l));
            else
            {
                bound.Encapsulate(w.Max);
                bound.Encapsulate(w.Min);
            }
        });
        return bound;

    }

    public string GetString()
    {
        return StringCompressor.CompressString(JsonUtility.ToJson(this));
    }
}

[Serializable]
public class SWord
{
    public string Name;
    public WordDirection WordDirection;
    public List<Vector2> LocationList;

    [NonSerialized]
    public int X;

    [NonSerialized]
    public int Y;

    [NonSerialized]
    public int MatchCount;


    public SWord()
    {
        Name = "";
        WordDirection = WordDirection.Horizontal;
        X = 0;
        Y = 0;
        MatchCount = 0;
        LocationList = null;
    }

    public SWord(SWord word, int matchCount)
    {
        Name = word.Name;
        WordDirection = word.WordDirection;
        X = word.X;
        Y = word.Y;
        MatchCount = matchCount;
        LocationList = word.LocationList;
    }

    public SWord(Word word)
    {
        Name = word.name;
        WordDirection = word.Direction;
        X = (int)word.Letters[0].transform.position.x;
        Y = (int)word.Letters[0].transform.position.y;
        MatchCount = 0;

        LocationList = new List<Vector2>();

        foreach (Letter letter in word.Letters)
            LocationList.Add(letter.transform.position);

    }

    public int Length { get { return Name.Length; } }

    public int XMin
    {
        get
        {
            if (WordDirection == WordDirection.Horizontal)
                return X - Length;
            else
                return X - 1;
        }
    }

    public int YMin
    {
        get
        {
            if (WordDirection == WordDirection.Horizontal)
                return Y - 1;
            else
                return Y - Length;
        }
    }

    public Vector3 Max { get { return new Vector3(X, Y, 0); } }
    public Vector3 Min { get { return new Vector3(XMin, YMin, 0); } }

    public static SWord Create(SWord inWord)
    {
        SWord outWord = inWord;

        return outWord;
    }

    public Vector2 Locations(int i)
    {
        if (LocationList != null)
            if (LocationList.Count > 0)
                return LocationList[i];

        if (WordDirection == WordDirection.Horizontal)
            return new Vector2(X - i, Y);
        else
            return new Vector2(X, Y - i);
    }


}
