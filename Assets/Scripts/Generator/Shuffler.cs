using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MgsCommonLib.Animation;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shuffler
{
    public void ShuffleEditor()
    {
        List<LetterBound> letterBounds = ComputeLetterBounds();
        foreach (var letterBound in letterBounds)
            letterBound.MoveTowardTarget(1f);
    }
    public IEnumerator ShuffleRuntime(MonoBehaviour owner)
    {
        List<LetterBound> letterBounds = ComputeLetterBounds();
        // Get target bound
        Bounds targetBounds = letterBounds[0].GetBounds();

        for (int i = 1; i < letterBounds.Count; i++)
            letterBounds[i].AddBounds(ref targetBounds);

        owner.StartCoroutine(Singleton.Instance.CameraController.FocusToBound(targetBounds));

        yield return MsgAnimation.RunAnimation(
            1f,
            (v) =>
            {
                foreach (var letterBound in letterBounds)
                    letterBound.MoveTowardTarget(v);
            });
    }

    private static List<LetterBound> ComputeLetterBounds()
    {
        List<List<Letter>> paritions = NewPartitioner.GetPartitions();

        #region Shuffle partions

        for (int i = 0; i < paritions.Count * 2; i++)
        {
            int p1 = UnityEngine.Random.Range(0, paritions.Count);
            int p2 = Random.Range(0, paritions.Count);

            var tp = paritions[p1];
            paritions[p1] = paritions[p2];
            paritions[p2] = tp;
        }

        #endregion

        #region Get bounds

        List<LetterBound> letterBounds = new List<LetterBound>();

        foreach (List<Letter> letters in paritions)
            letterBounds.Add(new LetterBound(letters));

        #endregion

        #region Place as a grid

        int columns = Mathf.RoundToInt(Mathf.Sqrt(letterBounds.Count));

        int x = 0, y = 0;

        for (int i = 0; i <= columns; i++)
        {
            int width = 0;
            int height = 0;

            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;

                if (index >= paritions.Count)
                    break;

                width += letterBounds[index].Width;

                if (height < letterBounds[index].Height)
                    height = letterBounds[index].Height;
            }

            x = -width / 2;

            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;

                if (index >= paritions.Count)
                    break;

                var bounds = letterBounds[index];

                bounds.SetTarget(x, y + (height - bounds.Height) / 2);

                x += bounds.Width;

            }

            y += height;
        }

        #endregion

        #region Move to center

        foreach (var letterBound in letterBounds)
            letterBound.TargetY += -y / 2 - 2;

        #endregion

        return letterBounds;
    }

}
public class LetterBound
{
    public int X, Y, Width, Height;

    public int TargetY;
    public int TargetX;
    public Vector3 Min;
    private Vector3 _lastDelta = Vector3.zero;

    private List<Letter> _letters;

    public LetterBound(List<Letter> letters)
    {
        _letters = letters;
        X = Mathf.RoundToInt(letters.Min(l => l.transform.position.x)) - 1;
        Y = Mathf.RoundToInt(letters.Min(l => l.transform.position.y)) - 1;

        Width = Mathf.RoundToInt(letters.Max(l => l.transform.position.x)) - X + 1;
        Height = Mathf.RoundToInt(letters.Max(l => l.transform.position.y)) - Y + 1;

        Min = new Vector3(X, Y, 0);
    }


    public void SetTarget(int x, int y)
    {
        TargetX = x;
        TargetY = y;
    }

    public void MoveTowardTarget(float value)
    {
        Vector3 delta = Vector3.Lerp(Min, new Vector3(TargetX, TargetY), value) - Min;

        _letters.ForEach(l => l.transform.position += delta - _lastDelta);

        _lastDelta = delta;
    }

    public Bounds GetBounds()
    {
        var delta = new Vector3(TargetX, TargetY) - Min;

        var bounds = new Bounds(_letters[0].transform.position+delta, Vector3.zero);

        for (int i = 1; i < _letters.Count; i++)
            bounds.Encapsulate(_letters[i].transform.position + delta);

        return bounds;
    }

    public void AddBounds(ref Bounds bounds)
    {
        var delta = new Vector3(TargetX, TargetY) - Min;

        foreach (Letter letter in _letters)
            bounds.Encapsulate(letter.transform.position + delta);
    }
}

