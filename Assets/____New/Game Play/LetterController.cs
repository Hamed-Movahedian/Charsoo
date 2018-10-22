using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LetterController : BaseObject
{
    #region Public

    public float Acceleration = 5;

    public List<Letter> AllLetters { get; set; }=new List<Letter>();
    public List<Letter> SelectedLetters { get; set; } = new List<Letter>();
    public List<Letter> LastSelectedLetters { get; set; } = new List<Letter>();

    #endregion

    #region Events

    public UnityEvent OnLetterSelected;
    public UnityEvent OnLetterUnselected;
    public UnityEvent OnDropLetter;

    private List<Vector2> _deltaList;

    #endregion

    #region Start

    private void Start()
    {
        EditorInitialize();
        BulidDeltaList();
        ConnectAdjacentLetters();
    }

    public void EditorInitialize()
    {
        AllLetters = FindObjectsOfType<Letter>().ToList();
    }

    private void OnValidate()
    {
        EditorInitialize();
    }

    public void ConnectAdjacentLetters()
    {
        ConnectAdjacentLetters(AllLetters);
    }

    public void ConnectAdjacentLetters(List<Letter> letters)
    {
        foreach (Letter letter in letters)
        {
            letter.Snap();
            letter.ConnectedLetters.Clear();
        }

        for (int i = 0; i < letters.Count; i++)
            for (int j = i + 1; j < letters.Count; j++)
                if (Math.Abs((letters[i].transform.position - letters[j].transform.position).magnitude -
                             1) < 0.1f)
                {
                    letters[i].ConnectedLetters.Add(letters[j]);
                    letters[j].ConnectedLetters.Add(letters[i]);
                }

        foreach (Letter letter in letters)
            letter.SetupBridges();
    }

    #endregion

    #region BulidDeltaList

    private void BulidDeltaList()
    {
        _deltaList = new List<Vector2>();

        for (int x = -Table.Size; x < Table.Size; x++)
            for (int y = -Table.Size; y < Table.Size; y++)
                _deltaList.Add(new Vector2(x, y));

        _deltaList = _deltaList.OrderBy(v => v.magnitude).ToList();
    }

    #endregion

    #region Selected/Unselected Letter

    public void LetterSelected(Letter letter)
    {
        Debug.Log($"Letter {letter.Char} is selected");
        OnLetterSelected.Invoke();

        SelectedLetters.Clear();

        letter.GetConnectedLetters(SelectedLetters);

        foreach (Letter selectedLetter in SelectedLetters)
            selectedLetter.Select();
    }

    public IEnumerator LetterUnselected()
    {
        yield return MoveToSafePlace();

        OnLetterUnselected.Invoke();

        foreach (Letter letter in SelectedLetters)
            letter.Unselect();

        LastSelectedLetters.Clear();
        LastSelectedLetters.AddRange(SelectedLetters);

        SelectedLetters.Clear();

        yield return new WaitForSeconds(0.1f);

        OnDropLetter.Invoke();
    }

    #endregion

    #region Move


    public void Move(Vector3 delta)
    {
        foreach (Letter selectedLetter in SelectedLetters)
            selectedLetter.Move(delta);
    }
    #endregion

    #region MoveToSafePlace

    private IEnumerator MoveToSafePlace()
    {
        Vector2 delta = Vector2.zero;
        if (!IsSafeMove(delta))
            delta = FindSafeMove();

        yield return MoveTo(delta);
    }

    private IEnumerator MoveTo(Vector2 delta)
    {
        delta = delta + SnapPos(SelectedLetters[0]) - (Vector2)SelectedLetters[0].transform.position;

        float duration = Mathf.Sqrt(delta.magnitude / Acceleration);

        AnimationCurve ValueCurve = AnimationCurve.EaseInOut(0, 0, duration, 1);

        float time = 0;

        Vector2 lastDelta = Vector2.zero;

        while (time < duration)
        {
            Vector2 currentDelta = Vector2.Lerp(Vector2.zero, delta, ValueCurve.Evaluate(time));

            foreach (Letter selectedLetter in SelectedLetters)
                selectedLetter.Move(currentDelta - lastDelta);

            lastDelta = currentDelta;

            yield return null;

            time += Time.deltaTime;
        }

        foreach (Letter selectedLetter in SelectedLetters)
        {
            selectedLetter.Move(delta - lastDelta);
            selectedLetter.Snap();
        }


    }

    private Vector2 FindSafeMove()
    {
        Vector2 deltaVector = Vector2.zero;

        foreach (Vector2 delta in _deltaList)
            if (IsSafeMove(delta))
                return delta;

        Debug.LogError("Can't find safe move!!!");

        return deltaVector;
    }

    private bool IsSafeMove(Vector2 delta)
    {
        foreach (Letter selectedLetter in SelectedLetters)
            foreach (Letter unselectedLetter in AllLetters)
                if (!unselectedLetter.IsSelected)
                    if (SnapPos(selectedLetter) + delta == SnapPos(unselectedLetter))
                        return false;

        return true;
    }

    private Vector2 SnapPos(Letter letter)
    {
        Vector3 position = letter.transform.position;

        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);

        return position;
    }

    #endregion

    public void DeleteAllLetters()
    {
        if (AllLetters == null)
            return;

        while (AllLetters.Count > 0)
        {
            Letter letter = AllLetters[0];

            if (letter != null)
            {
                if (!Application.isPlaying)
                    DestroyImmediate(letter.gameObject);
                else
                    PoolManager.Instance.Return(letter);

            }
            AllLetters.Remove(letter);
        }
    }


    public Bounds GetLettersBound()
    {
        Bounds bound = new Bounds(AllLetters[0].transform.position, Vector3.zero);

        foreach (Letter letter in AllLetters)
            bound.Encapsulate(letter.transform.position);

        return bound;
    }

}
