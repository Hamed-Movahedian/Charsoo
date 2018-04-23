﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Partioner : BaseObject
{
    public int MinZise = 2;
    public int MaxSize = 3;
    public int ErrorCount = 1;
    public List<List<Letter>> Paritions;
    public Color ParitionColor;
    private List<Letter> _allLetters;
    public Action<UnityEngine.Object, string> Undo;
    private int _compressCount;
    private bool _cancel;

    public Func<string, float, bool> ShowProgressBar;
    public Func<double> GetTime;
    private double _lastTime;
    private int _invalidResults;
    public bool Validate = true;

    public void PartionLetters()
    {
        // initialize
        _cancel = false;
        _compressCount = 1;
        _invalidResults = 0;
        // record time
        _lastTime = GetTime();
        Solver solver = GetComponent<Solver>();
        solver.GetTime = GetTime;


        for (int i = 0; i < 30000; i++)
        {
            if (TryPartion())
            {
                solver.ProgressTitle = "Result number " + (_invalidResults + 1);
                if (Validate)
                    if (!solver.Validate(ErrorCount, ShowProgressBar, ref _cancel))
                    {
                        if (_cancel)
                            break;
                        _invalidResults++;
                        continue;
                    }

                // SetupBridges for all letters
                Paritions.SelectMany(p => p)
                    .ToList()
                    .ForEach(l => l.SetupBridges());

                Debug.Log("Connect in " + i + " try.");

                return;
            }


            if (_cancel)
                break;
            // Show Progress Bar
            if (ShowProgressBar != null)
                if (GetTime() - _lastTime > .1)
                {
                    ShowProgressBar("Invalid results : " + _invalidResults + " Searching...", i / 30000f);

                    _lastTime = GetTime();
                }


        }

        Paritions.Clear();

        if (!_cancel)
        {
            Debug.Log(ErrorCount+1);
            ErrorCount++;
            PartionLetters();
        }
        Debug.LogError(string.Format("Portion failed with {0} Invalid results !!!", _invalidResults));
    }


    private bool TryPartion()
    {
        LetterController.ConnectAdjacentLetters();

        // Clear partitions
        if (Paritions == null)
            Paritions = new List<List<Letter>>();
        Paritions.Clear();

        // Add first partion with all letters
        Paritions.Add(
            WordManager
            .GetComponentsInChildren<Word>()
            .SelectMany(w => w.Letters)
            .Distinct()
            .ToList());


        while (Paritions.Max(p => p.Count) > MaxSize)
        {
            // Get random letter from biggest partion
            List<Letter> biggestPartion = Paritions.OrderByDescending(p => p.Count).First();
            Letter letter = GetRandomMember(biggestPartion);

            // Get connected letters
            List<Letter> newPartion = new List<Letter>();
            GetConnectedLetters(letter, newPartion, Random.Range(MinZise+1, MaxSize + 1));

            // Disconnect newPartion and add to list
            Disconnect(biggestPartion, newPartion);
            Paritions.Add(newPartion);

            // Separate Biggest partion in two -- if possible
            Separate(biggestPartion);

            if (Paritions.Min(p => p.Count) < MinZise)
                return false;
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (Paritions == null)
            return;


        Gizmos.color = ParitionColor;

        foreach (List<Letter> parition in Paritions)
        {
            Bounds bounds = new Bounds(parition[0].transform.position, Vector3.zero);
            parition.ForEach(l => bounds.Encapsulate(l.transform.position));
            Gizmos.DrawCube(bounds.center, bounds.size + new Vector3(.7f, .7f, 0));
        }
    }

    private void Separate(List<Letter> partion)
    {
        List<Letter> tp = new List<Letter>();

        GetConnectedLetters(partion[0], tp, partion.Count);

        if (tp.Count < partion.Count)
        {
            tp.ForEach(l => partion.Remove(l));
            Paritions.Add(tp);
            Separate(partion);
        }
    }

    private void Disconnect(List<Letter> biggestPartion, List<Letter> newPartion)
    {
        foreach (Letter letter in newPartion)
            biggestPartion.Remove(letter);

        foreach (Letter l1 in biggestPartion)
            foreach (Letter l2 in newPartion)
                l1.DisConnect(l2);
    }

    private void GetConnectedLetters(Letter letter, List<Letter> list, int count)
    {
        if (list.Contains(letter))
            return;

        if (list.Count < count)
        {
            list.Add(letter);

            foreach (Letter connectedLetter in letter.ConnectedLetters)
                GetConnectedLetters(connectedLetter, list, count);
        }
    }

    private Letter GetRandomMember(List<Letter> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public void Shafle()
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

        List<Bounds> boundses = new List<Bounds>();


        foreach (List<Letter> parition in Paritions)
        {
            Bounds bounds = new Bounds(parition[0].transform.position, Vector3.zero);

            parition.ForEach(l => bounds.Encapsulate(l.transform.position));

            bounds.size += new Vector3(2, 2, 0);

            boundses.Add(bounds);
        }

        #endregion

        #region Place as a grid

        float colums = Mathf.Sqrt(boundses.Count);

        float x = 0, y = 0;
        float maxX = 0;

        for (int i = 0; i < colums; i++)
        {
            x = 0;
            float maxY = 0;
            for (int j = 0; j < colums; j++)
            {
                int index = (int)(i * colums + j);

                if (index >= Paritions.Count)
                    break;

                var bounds = boundses[index];

                x += bounds.extents.x;

                MovePartion(index, new Vector3(x, y + bounds.extents.y) - bounds.center);

                x += bounds.extents.x;

                if (bounds.size.y > maxY)
                    maxY = bounds.size.y;

            }

            if (x > maxX)
                maxX = x;

            y += maxY;
        }

        #endregion

        #region Move to center

        // Compute center
        _allLetters = Paritions
            .SelectMany(p => p)
            .ToList();

        Vector3 center = _allLetters
            .Select(l => l.transform.position)
            .Aggregate(((a, l) => a + l)) * (1f / _allLetters.Count);

        // Move center to (0,0)
        _allLetters
            .ForEach(l => l.transform.position -= center);


        #endregion
    }

    public void Compress()
    {

        foreach (List<Letter> parition in Paritions)
            TryMoveToCenter(parition);

        _compressCount++;

        LetterController.AllLetters.ForEach(l => l.Snap());
    }

    private bool TryMoveToCenter(List<Letter> parition)
    {
        bool result = false;
        // partition center
        Vector3 center = parition
                          .Select(l => l.transform.position)
                          .Aggregate((a, p) => a + p) * (1f / parition.Count);
        // delta move
        Vector2 delta = new Vector2(center.x == 0 ? 0 : -Mathf.Sign(center.x), 0);

        if (delta != Vector2.zero)
            if (IsValidMove(parition, delta))
            {
                parition.ForEach(l => l.transform.position += (Vector3)delta);
                result = true;
            }

        delta = new Vector2(0, center.y == 0 ? 0 : -Mathf.Sign(center.y));

        if (delta != Vector2.zero)
            if (IsValidMove(parition, delta))
            {
                parition.ForEach(l =>
                {
                    //if (!result)
                    Undo(l.transform, "Compress " + _compressCount);
                    l.transform.position += (Vector3)delta;
                });
                result = true;
            }

        return result;
    }

    private bool IsValidMove(List<Letter> parition, Vector2 delta)
    {
        foreach (Letter targetLetter in parition)
            foreach (Letter otherLetter in _allLetters.Where(l => !parition.Contains(l)))
                if (Vector2.Distance((Vector2)targetLetter.transform.position + delta, otherLetter.transform.position) < 1.5f)
                    return false;

        return true;
    }

    private void MovePartion(int index, Vector3 delta)
    {
        Paritions[index].ForEach(l =>
        {
            Undo(l.transform, "Shuffle");
            l.transform.position += delta;
        });
    }

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
                    (w.Direction == Direction.Horizontal) ?
                    Direction.Vertical : Direction.Horizontal;
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

    public void Cancel()
    {
        _cancel = true;
    }
}
