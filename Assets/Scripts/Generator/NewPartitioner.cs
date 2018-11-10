using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MgsCommonLib.Utilities;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class NewPartitioner
{
    private List<int> _partSizeList = new List<int> { 2, 3, 3, 3 };
    public IEnumerator Portion(int tryCount=500)
    {
        Singleton.Instance.LetterController.EditorInitialize();

        var words = Singleton.Instance.WordManager.Words;

        float bestScore = 100000;
        List<List<Letter>> bestPartitons = null;

        for (int i = 0; i < tryCount; i++)
        {
            Singleton.Instance.LetterController.ConnectAdjacentLetters();

            foreach (var word in words)
                Partition(word.Letters, true);

            var partitions = GetPartitions();

            var biggestPartSize = partitions.Max(p => p.Count);
            var biggestPartSizecount = partitions.Count(p => p.Count == biggestPartSize);
            var smallPartSizecount = partitions.Count(p => p.Count == 2);

            var score =
                biggestPartSize * 30 +
                biggestPartSizecount +
                smallPartSizecount * 10;

            if (score < bestScore)
            {
                bestPartitons = partitions;
                bestScore = score;
            }

            MgsCoroutine.Percentage = 500 / (float)i;
            if (i % 5 == 0)
            {
                Singleton.Instance.LetterController.AllLetters.ForEach(l => l.SetupBridges());
                yield return null;
            }
        }

        Singleton.Instance.LetterController.AllLetters.ForEach(l => l.ConnectedLetters.Clear());

        foreach (var partiton in bestPartitons)
        {
            Singleton.Instance.LetterController.ConnectAdjacentLetters(partiton);
        }

        Singleton.Instance.LetterController.AllLetters.ForEach(l => l.SetupBridges());
    }

    private void Partition(List<Letter> letters, bool firstTime)
    {
        if (!firstTime && letters.Count < 4)
            return;

        var fpc = _partSizeList[Random.Range(0, _partSizeList.Count)];

        fpc = Mathf.Min(fpc, letters.Count - 2);
        fpc = Mathf.Max(fpc, 2);

        letters[fpc - 1].DisConnect(letters[fpc]);

        Partition(
            letters.GetRange(fpc, letters.Count - fpc).ToList(),
            false);

    }
    private void Partition_Old(List<Letter> letters, bool firstTime)
    {
        int half;

        if (firstTime)
        {
            half = letters.Count -
                   Mathf.Min(letters.Count - 2, UnityEngine.Random.Range(1, 4));
        }
        else
        {
            if (letters.Count <= 3)
                return;

            half = letters.Count -
                   Mathf.Min(letters.Count - 3, UnityEngine.Random.Range(1, 4));

        }

        letters[half - 1].DisConnect(letters[half]);

        Partition(
            letters.Take(half).ToList(),
            false);

    }


    public static List<List<Letter>> GetPartitions()
    {
        List<List<Letter>> paritions = new List<List<Letter>>();

        var allLetters =
            new List<Letter>(Singleton.Instance.LetterController.AllLetters);

        while (allLetters.Count > 0)
        {
            var letters = new List<Letter>();
            allLetters[0].GetConnectedLetters(letters);
            paritions.Add(letters);
            letters.ForEach(l => allLetters.Remove(l));
        }

        return paritions;
    }
}
