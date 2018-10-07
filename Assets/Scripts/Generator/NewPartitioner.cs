using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class NewPartitioner
{
    public void Portion()
    {
        Singleton.Instance.LetterController.ConnectAdjacentLetters();

        var words = Singleton.Instance.WordManager.Words;

        foreach (var word in words)
            Partition(word.Letters, true);

        Singleton.Instance.LetterController.AllLetters.ForEach(l=>l.SetupBridges());
    }

    private void Partition(List<Letter> letters,bool firstTime)
    {
        if (!firstTime && letters.Count <= UnityEngine.Random.Range(3, 4))
            return;

        var half = letters.Count / 2;

        letters[half-1].DisConnect(letters[half]);

        Partition(
            letters.Take(half).ToList(),
            false);

        Partition(
            letters.GetRange(half,letters.Count-half).ToList(),
            false);
    }
}
