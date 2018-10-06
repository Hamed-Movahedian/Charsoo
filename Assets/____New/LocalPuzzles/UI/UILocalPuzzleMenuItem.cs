using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            _puzzle.Paid ? PersianFixer.Fix(_puzzle.Clue) :
            ThemeManager.Instance.LanguagePack.GetLable("LockPuzzle");

        GetComponent<Image>().color = _puzzle.Paid ? OpenColor : LockColor;
        Row.gameObject.SetActive(_puzzle.Paid);
        LockIcon.gameObject.SetActive(!_puzzle.Paid);

        SolvedIcon.gameObject.SetActive(_puzzle.Solved);

        Row.text = PersianFixer.Fix((_puzzle.Row + 1).ToString(), true, true);

        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    public override void Select()
    {
        if (!_puzzle.Paid)
        {
            ((LocalPuzzlesSelectionWindow)_list).LockSelect();
            return;
        }
        base.Select();
    }
}
