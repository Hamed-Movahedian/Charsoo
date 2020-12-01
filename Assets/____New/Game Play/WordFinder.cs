using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.Utilities;
using UnityEngine;

public class WordFinder : MonoBehaviour
{
    public List<Bounds> FoundWords { get; set; } = new List<Bounds>();
    public List<Bounds> FoundErrorWords { get; set; } = new List<Bounds>();

    private Dictionary<Vector3, Letter> _location;

    [FollowMachine("Search for complete words", "Found,Not found")]
    public void Search(List<Letter> letters,List<Letter> dropLetters, List<Word> words)
    {
        // Initialize
        FoundWords.Clear();
        FoundErrorWords.Clear();

        // create location dictionary
        _location = new Dictionary<Vector3, Letter>();
        letters.ForEach(l => _location.Add(l.transform.position, l));

        // discover complete words
        foreach (WordDirection direction in Enum.GetValues(typeof(WordDirection)))
        {
            for (var i = 0; i < letters.Count; i++)
            {
                var letter = letters[i];

                if (!IsStart(letter, direction))
                    continue;

                List<List<Letter>> foundLettersList = FindLettersList(letter,direction);

                foreach (var foundLetters in foundLettersList)
                {
                    if (dropLetters.Count > 0 && !foundLetters.Any(dropLetters.Contains))
                        continue;

                    if (!CheckCorrectWords(foundLetters, direction, words))
                        CheckErrorWords(foundLetters, direction, words);

                }            }
        }

        if(FoundErrorWords.Count>0 || FoundWords.Count>0)
            FollowMachine.SetOutput("Found");
        else
            FollowMachine.SetOutput("Not found");
    }

    private List<List<Letter>> FindLettersList(Letter letter, WordDirection direction)
    {
        var lettersList= new List<List<Letter>>();
        var letters=new List<Letter>();

        while (letter!=null)
        {
            letters.Add(letter);

            var next = Next(letter, direction);

            if(!letter.IsConnectedTo(next))
                lettersList.Add(new List<Letter>(letters));

            letter = next;
        }

        return lettersList;
    }

    private void CheckErrorWords(List<Letter> letters, WordDirection direction, List<Word> words)
    {
        var charList = letters.Select(l => l.Char).ToList();

        foreach (var word in words)
        {
            if (word.IsComplete)
                continue;

            if (word.Letters.Count != letters.Count)
                continue;

            if (!charList.IsEqualTo(word.Letters.Select(l => l.Char).ToList()))
                continue;

            FoundErrorWords.Add(GetBounds(letters));
        }

    }

    private static Bounds GetBounds(List<Letter> letters)
    {
        Bounds bound = new Bounds(letters[0].transform.position, Vector3.zero);

        foreach (Letter letter in letters)
            letter.AddToBounds(ref bound);

        return bound;
    }

    private bool CheckCorrectWords(List<Letter> letters, WordDirection direction, List<Word> words)
    {

        foreach (var word in words)
        {
            if (word.IsComplete)
                continue;

            if (word.Direction != direction)
                continue;

            if (!letters.IsEqualTo(word.Letters))
                continue;

            word.Complete();

            FoundWords.Add(GetBounds(letters));

            return true;
        }

        return false;
    }

    private Letter Next(Letter letter, WordDirection direction)
    {
        if (_location.ContainsKey(letter.transform.position + Next(direction)))
            return _location[letter.transform.position + Next(direction)];
        else
            return null;
    }

    private bool IsStart(Letter letter, WordDirection direction)
    {
        // if next is empty return false
        if (!HasNextLetter(letter, direction))
            return false;

        if (!HasPreLetter(letter, direction))
            return true;

        return !PreLetter(letter, direction).IsConnectedTo(letter);
    }

    private Letter PreLetter(Letter letter, WordDirection direction)
    {
        return _location[letter.transform.position - Next(direction)];
    }

    private bool HasPreLetter(Letter letter, WordDirection direction)
    {
        return _location.ContainsKey(letter.transform.position - Next(direction));
    }

    private bool HasNextLetter(Letter letter, WordDirection direction)
    {
        return _location.ContainsKey(letter.transform.position + Next(direction));
    }

    private static Vector3 Next(WordDirection direction) =>
        direction == WordDirection.Horizontal ? Vector3.left : Vector3.down;
}
