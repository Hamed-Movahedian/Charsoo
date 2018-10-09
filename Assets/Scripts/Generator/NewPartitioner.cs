using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class NewPartitioner
{
    public void Portion()
    {
        Singleton.Instance.LetterController.ConnectAdjacentLetters();

        var words = Singleton.Instance.WordManager.Words;

        foreach (var word in words)
            Partition(word.Letters, true);

        Singleton.Instance.LetterController.AllLetters.ForEach(l => l.SetupBridges());
    }

    private void Partition(List<Letter> letters, bool firstTime)
    {
        int half;

        if (firstTime)
        {
            half = letters.Count -
                   Mathf.Min(letters.Count - 2, UnityEngine.Random.Range(2,  5 ));
        }
        else
        {
            if (letters.Count <= 3)
                return;

            half = letters.Count -
                   Mathf.Min(letters.Count - 3, UnityEngine.Random.Range(2,  5));

        }
        letters[half - 1].DisConnect(letters[half]);

        Partition(
            letters.Take(half).ToList(),
            false);

    }


}
