using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib.Animation;
using MgsCommonLib.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class Partitioner : BaseObject
{
    public int MinSize = 2;
    public int MaxSize = 3;
    public int ErrorCount = 1;
    public List<List<Letter>> Paritions;
    public Color ParitionColor;
    public Action<UnityEngine.Object, string> Undo;
    public bool Validate = true;


    private List<Letter> _allLetters;
    private int _compressCount;
    public int InvalidResults;
    private WordSetValidator _validator;
    private double _startTime;
    public int TryCount;
    public bool PartitionSuccessfully;
    public int MaxTry;

    #region Partitionerer

    public IEnumerator PortionLetters()
    {

        // initialize
        _compressCount = 1;
        InvalidResults = 0;
        PartitionSuccessfully = false;
        MgsCoroutine.Percentage = 0;
        MgsCoroutine.Title = "Partitioning";

        // Validator
        if (_validator == null)
            _validator = new WordSetValidator();


        _validator.Initialize(this);


        for (TryCount = 0; TryCount < MaxTry; TryCount++)
        {
            MgsCoroutine.Info = " Try " + TryCount + "\n\r Invalid Results " + InvalidResults;
            MgsCoroutine.Percentage = ((float)TryCount) / MaxTry;
            yield return null;

            if (TryPartition())
            {
                // SetupBridges for all letters
                Paritions.SelectMany(p => p)
                    .ToList()
                    .ForEach(l => l.SetupBridges());

                if (Validate)
                {
                    yield return _validator.ValidateWordSet(Paritions);

                    if (!_validator.IsValid)
                    {
                        InvalidResults++;
                        continue;
                    }
                }

                PartitionSuccessfully = true;

                yield break;
            }
            
        }
        PartitionSuccessfully = false;

        Paritions.Clear();

    }


    private bool TryPartition()
    {

        LetterController.ConnectAdjacentLetters();

        // Clear partitions
        if (Paritions == null)
            Paritions = new List<List<Letter>>();
        Paritions.Clear();

        // Add first partition with all letters
        Paritions.Add(
            WordManager
                .GetComponentsInChildren<Word>()
                .SelectMany(w => w.Letters)
                .Distinct()
                .ToList());


        while (Paritions.Max(p => p.Count) > MaxSize)
        {
            // Get random letter from biggest partition
            List<Letter> biggestPartition = Paritions.OrderByDescending(p => p.Count).First();
            Letter letter = GetRandomMember(biggestPartition);

            // Get connected letters
            List<Letter> newPartition = new List<Letter>();
            GetConnectedLetters(letter, newPartition, Random.Range(MinSize + 1, MaxSize + 1));

            // Disconnect newPartition and add to list
            Disconnect(biggestPartition, newPartition);
            Paritions.Add(newPartition);

            // Separate Biggest partition in two -- if possible
            Separate(biggestPartition);

            if (Paritions.Min(p => p.Count) < MinSize)
                return false;
        }

        return true;
    }

    private void Separate(List<Letter> partition)
    {
        List<Letter> tp = new List<Letter>();

        GetConnectedLetters(partition[0], tp, partition.Count);

        if (tp.Count < partition.Count)
        {
            tp.ForEach(l => partition.Remove(l));
            Paritions.Add(tp);
            Separate(partition);
        }
    }

    private void Disconnect(List<Letter> biggestPartition, List<Letter> newPartition)
    {
        foreach (Letter letter in newPartition)
            biggestPartition.Remove(letter);

        foreach (Letter letter in newPartition)
        {
            for (int i = 0; i < letter.ConnectedLetters.Count; i++)
            {
                if (biggestPartition.Contains(letter.ConnectedLetters[i]))
                {
                    letter.DisConnect(letter.ConnectedLetters[i]);
                    i--;
                }
            }
        }
    }

    private void GetConnectedLetters(Letter letter, List<Letter> list, int count)
    {
        if (list.Contains(letter))
            return;

        if (list.Count < count)
        {
            list.Add(letter);

            var connectedLetters = letter.ConnectedLetters.OrderBy(l => l.ConnectedLetters.Count);

            foreach (Letter connectedLetter in connectedLetters)
                GetConnectedLetters(connectedLetter, list, count);
        }
    }

    private Letter GetRandomMember(List<Letter> list)
    {
        var letters = list
            .Where(l => l.ConnectedLetters.Count == 1)
            .ToList();
        return list[Random.Range(0, list.Count)];
    }


    #endregion

    #region Gizmo

    private void OnDrawGizmos()
    {
        if (Paritions == null)
            return;

        return;

        Gizmos.color = ParitionColor;

        for (int i = 0; i < Paritions.Count; i++)
        {
            List<Letter> parition = Paritions[i];

            Bounds bounds = new Bounds(parition[0].transform.position, Vector3.zero);
            parition.ForEach(l => bounds.Encapsulate(l.transform.position));
            Gizmos.DrawCube(bounds.center, bounds.size + new Vector3(.7f, .7f, 0));

            var style = new GUIStyle();
            style.fontSize = 30;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = Color.yellow;
#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.Label(bounds.center + Vector3.back * 3, i.ToString(), style);
#endif
        }
    }


    #endregion

    public IEnumerator Shuffle()
    {
        _compressCount = 1;

        #region Shuffle partions

        for (int i = 0; i < Paritions.Count * 2; i++)
        {
            int p1 = Random.Range(0, Paritions.Count);
            int p2 = Random.Range(0, Paritions.Count);

            var tp = Paritions[p1];
            Paritions[p1] = Paritions[p2];
            Paritions[p2] = tp;
        }

        #endregion

        #region Get bounds

        List<LetterBound> letterBounds = new List<LetterBound>();

        foreach (List<Letter> letters in Paritions)
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

                if (index >= Paritions.Count)
                    break;

                width += letterBounds[index].Width;

                if (height < letterBounds[index].Height)
                    height = letterBounds[index].Height;
            }

            x = -width / 2;

            for (int j = 0; j < columns; j++)
            {
                int index = i * columns + j;

                if (index >= Paritions.Count)
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

        if (Application.isPlaying)
        {
            // Get target bound
            Bounds targetBounds = letterBounds[0].GetBounds();

            for (int i = 1; i < letterBounds.Count; i++)
                letterBounds[i].AddBounds(ref targetBounds);

            StartCoroutine(Singleton.Instance.CameraController.FocusToBound(targetBounds));

            yield return MsgAnimation.RunAnimation(
                1f,
                (v) =>
                {
                    foreach (var letterBound in letterBounds)
                        letterBound.MoveTowardTarget(v);
                });
        }
        else
            foreach (var letterBound in letterBounds)
                letterBound.MoveTowardTarget(1f);
    }

    #region Compress

    public void Compress()
    {

        foreach (List<Letter> parition in Paritions)
            TryMoveToCenter(parition);

        _compressCount++;

        LetterController.AllLetters.ForEach(l => l.Snap());
    }

    private bool TryMoveToCenter(List<Letter> partition)
    {
        bool result = false;
        // partition center
        Vector3 center = partition
                             .Select(l => l.transform.position)
                             .Aggregate((a, p) => a + p) * (1f / partition.Count);
        // delta move
        Vector2 delta = new Vector2(center.x == 0 ? 0 : -Mathf.Sign(center.x), 0);

        if (delta != Vector2.zero)
            if (IsValidMove(partition, delta))
            {
                partition.ForEach(l => l.transform.position += (Vector3)delta);
                result = true;
            }

        delta = new Vector2(0, center.y == 0 ? 0 : -Mathf.Sign(center.y));

        if (delta != Vector2.zero)
            if (IsValidMove(partition, delta))
            {
                partition.ForEach(l =>
                {
                    //if (!result)
                    Undo(l.transform, "Compress " + _compressCount);
                    l.transform.position += (Vector3)delta;
                });
                result = true;
            }

        return result;
    }

    private bool IsValidMove(List<Letter> partition, Vector2 delta)
    {
        foreach (Letter targetLetter in partition)
            foreach (Letter otherLetter in _allLetters.Where(l => !partition.Contains(l)))
                if (Vector2.Distance((Vector2)targetLetter.transform.position + delta, otherLetter.transform.position) < 1.5f)
                    return false;

        return true;
    }


    #endregion

    public void Clear()
    {
        if (Paritions != null)
            Paritions.Clear();
    }

    public void Rotate()
    {
        WordManager
            .GetComponentsInChildren<Word>()
            .ToList()
            .ForEach(w =>
            {
                Undo(w, "Rotate");
                w.Direction =
                    (w.Direction == WordDirection.Horizontal) ?
                    WordDirection.Vertical : WordDirection.Horizontal;
            });

        LetterController
            .AllLetters
            .ForEach(l =>
            {
                Vector3 position = l.transform.position;
                position.x = l.transform.position.y;
                position.y = l.transform.position.x;

                Undo(l.transform, "Rotate");
                l.transform.position = position;
            });

        LetterController.ConnectAdjacentLetters();
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

