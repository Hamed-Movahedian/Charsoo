using System;
using SQLite4Unity3d;
using UnityEngine;

public class Puzzle : BaseTable
{
    public string Clue { get; set; }

    public int? CategoryID { get;  set; }
    public string Content { get; set; }
    public int Row { get; set; }

    public bool Solved { get; set; }
    public bool Paid { get; set; }

    public WordSet GetWordSet()
    {
        WordSet wordSet = new WordSet();
        string decompressString = StringCompressor.DecompressString(Content);
/*        string replace = decompressString.Replace("\"Direction\":1", "\"Direction\":V");
        decompressString= replace.Replace("\"Direction\":0", "\"Direction\":H");*/
        JsonUtility.FromJsonOverwrite(decompressString, wordSet);
        return wordSet;
    }
}
