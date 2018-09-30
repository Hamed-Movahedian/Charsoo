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

                var foundLetters = new List<Letter>();

                while (letter != null)
                {
                    foundLetters.Add(letter);
                    letter = Next(letter, direction);
                }

                if(!foundLetters.Any(dropLetters.Contains))
                    continue;

                if (!CheckCorrectWords(foundLetters, direction, words))
                    CheckErrorWords(foundLetters,direction,words);
            }
        }

        if(FoundErrorWords.Count>0 || FoundWords.Count>0)
            FollowMachine.SetOutput("Found");
        else
            FollowMachine.SetOutput("Not found");
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

    private Letter Next(Letter letter, WordDirection direction) =>
        _location.ContainsKey(letter.transform.position + Next(direction)) ?
            _location[letter.transform.position + Next(direction)] :
            null;

    private bool IsStart(Letter letter, WordDirection direction) =>
            _location.ContainsKey(letter.transform.position + Next(direction)) &&
            !_location.ContainsKey(letter.transform.position - Next(direction));

    private static Vector3 Next(WordDirection direction) =>
        direction == WordDirection.Horizontal ? Vector3.left : Vector3.down;
}
