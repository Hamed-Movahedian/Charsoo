using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using MgsCommonLib.Theme;
using SQLite4Unity3d;
using UnityEngine;
using UnityEngine.UI;

public class UILocalPuzzleMenuItem : UIMenuItem
{
    public Text ClueText;
    public Text Row;
    public Image LockIcon;
    public Image SolvedIcon;
    public Color OpenColor;
    public Color LockColor;
    private Puzzle _puzzle;


    protected override void Refresh(object data)
    {
        _puzzle = (Puzzle)data;
        ClueText.text =
            IsAvalable() ? ArabicFixer.Fix(_puzzle.Clue) :
            ThemeManager.Instance.LanguagePack.GetLable("LockPuzzle");

        GetComponent<Image>().color = _puzzle.Paid ? OpenColor : LockColor;
        Row.gameObject.SetActive(_puzzle.Paid);
        LockIcon.gameObject.SetActive(!_puzzle.Paid);
        SolvedIcon.gameObject.SetActive(_puzzle.Solved);
        Row.text = ArabicFixer.Fix((_puzzle.Row + 1).ToString(), true, true);

        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    private bool IsAvalable()
    {
        if (_puzzle.Paid)
            return true;
        if (_puzzle.Row == 0)
        {
            _puzzle.Paid = true;
            LocalDBController.InsertOrReplace(_puzzle);
            return true;
        }

        return false;
    }

    public override void Select()
    {
        if (!_puzzle.Paid)
        {
            Debug.Log("Selected LockItem ");
            ((LocalPuzzlesSelectionWindow)_list).LockSelect();
            return;
        }
        base.Select();
    }
}
