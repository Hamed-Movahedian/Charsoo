using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleMenuItem : BaseObject
{
    #region Public

    public Text Row;
    public GameObject LockIcon;
    public GameObject SolvedIcon;
    public Text Clue;

    #endregion

    #region Private

    private Puzzle _data;
    private bool _lock;

    #endregion

    #region Properties

    public bool IsSolved
    {
        get { return _data.Solved; }
    }

    #endregion

    #region Setup

    public void Setup(Puzzle puzzle)
    {
        _data = puzzle;

        SetVisuals();

    }

    #endregion

    #region SetVisuals

    private void SetVisuals()
    {
        CheckLockCondition();

        Row.text = ArabicFixer.Fix((_data.Row + 1).ToString(), true, true);
        LockIcon.SetActive(_lock);
        SolvedIcon.SetActive(_data.Solved);
        Row.gameObject.SetActive(!_lock);

        Clue.text = _lock ? ArabicFixer.Fix("بسته") : ArabicFixer.Fix(_data.Clue);
    }

    #endregion

    #region CheckLockCondition

    private void CheckLockCondition()
    {
        _lock = _data.Row != 0;

        if (_data.Solved || _data.Paid)
            _lock = false;

        if (_lock)
        {
            var row = _data.Row - 1;

            Puzzle prePuzzle = LocalDatabase
                .Table<Puzzle>()
                .SqlWhere(p => p.CategoryID == _data.CategoryID && p.Row == row)
                .FirstOrDefault();

            if (prePuzzle != null)
                _lock = !prePuzzle.Solved;
        }
    }


    #endregion

    #region Select

    public void Select()
    {
        if (!_lock)
        {
            ContentManager.SelectedPuzzleItem = this;
            WordSpawner.WordSet = _data.GetWordSet();
            ContentManager.OnWordSetSelected.Invoke();
        }
    }

    #endregion

    #region StartNext

    public void StartNext()
    {
        var activeItems = ContentManager.DynamicList.GetActiveItems<PuzzleMenuItem>();

        foreach (var activeItem in activeItems)
            if (activeItem._data.Row == _data.Row + 1)
            {
                if (!activeItem._data.Solved)
                {
                    activeItem.SetVisuals();
                    activeItem.Select();
                }
                else
                    ContentManager.OnShowPackPanel.Invoke();

                return;
            }

        ContentManager.OnShowPackPanel.Invoke();
        ContentManager.Back();
    }

    #endregion

    #region Solved
    public void Solved()
    {
        _data.Solved = true;

        LocalDatabase.Update(_data);

        SetVisuals();
    }
    #endregion

}
