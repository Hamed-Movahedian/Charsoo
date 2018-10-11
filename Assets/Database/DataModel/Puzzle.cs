using System;
using System.Linq;
using SQLite4Unity3d;
using UnityEngine;

public class Puzzle : BaseTable
{
    public string Clue { get; set; }
    public int? CategoryID { get; set; }
    public string Content { get; set; }
    public int Row { get; set; }

    public bool Solved
    {
        get
        {
            return LocalDBController
                .Table<PlayPuzzles>()
                .SqlWhere(p => p.PuzzleID == ID)
                .Any(p => p.Success);
        }
    }

    public bool Paid
    {
        get
        {
            if (Row == 0)
                return true;

            Category category = LocalDBController
                .Table<Category>()
                .SqlWhere(c => c.ID == CategoryID.Value)
                .FirstOrDefault();

            if (category.IsUnlocked)
            {
                return true;
            }

            var prePuzzle = LocalDBController
                .Table<Puzzle>()
                .SqlWhere(p => p.CategoryID == CategoryID)
                .FirstOrDefault(p => p.Row == Row - 1);



            if (prePuzzle == null)
                return true;

            return prePuzzle.Solved;
        }
    }

    public WordSet GetWordSet()
    {
        WordSet wordSet = new WordSet();
        string decompressString = StringCompressor.DecompressString(Content);
        string replace = decompressString.Replace("\"Direction\"", "\"WordDirection\"");
        JsonUtility.FromJsonOverwrite(replace, wordSet);
        return wordSet;
    }
}
