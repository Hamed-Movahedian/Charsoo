using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PuzzleSolver : MonoBehaviour
{
    private List<Letter> _letters;
    private List<Word> _words;
    public Dictionary<Letter, Vector3> LetterPositions { get; set; }

    public void Solve(List<Letter> letters, List<Word> words)
    {
        // Initialize
        _letters = new List<Letter>(letters);
        _words = new List<Word>(words);

        LetterPositions = new Dictionary<Letter, Vector3>();
        _letters.ForEach(l => LetterPositions.Add(l, Vector3.zero));

        // Place first letter
        Place(_letters[0], _letters[0].transform.position);

        // Place all letters
        while (_letters.Count > 0)
        {
            foreach (var word in _words)
            {
                Letter cLetter = FindCommonLetter(word);

                if (cLetter != null)
                {
                    Place(word, cLetter);
                    break;
                }
            }
        }

    }

    private void Place(Word word, Letter cLetter)
    {
        var dir = word.Direction == WordDirection.Horizontal ? Vector3.right : Vector3.up;

        var cIndex = word.Letters.IndexOf(cLetter);

        var startPos = LetterPositions[cLetter] + dir * cIndex;

        for (int i = 0; i < word.Letters.Count; i++)
        {
            Place(word.Letters[i], startPos - dir * i);
        }

        _words.Remove(word);
    }

    private Letter FindCommonLetter(Word word)
    {
        return word.Letters.FirstOrDefault(l => !_letters.Contains(l));
    }

    private void Place(Letter letter, Vector3 position)
    {
        LetterPositions[letter] = position;

        if (_letters.Contains(letter))
        {
            _letters.Remove(letter);
        }
    }
}
