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
            IsAvalable() ? PersianFixer.Fix(_puzzle.Clue) :
            ThemeManager.Instance.LanguagePack.GetLable("LockPuzzle");

        GetComponent<Image>().color = IsAvalable() ? OpenColor : LockColor;
        Row.gameObject.SetActive(IsAvalable());
        LockIcon.gameObject.SetActive(!IsAvalable());

        SolvedIcon.gameObject.SetActive(
            LocalDBController.Table<PlayPuzzles>()
            .FirstOrDefault(pp => pp.PuzzleID == _puzzle.ID && pp.Success)!=null);

        Row.text = PersianFixer.Fix((_puzzle.Row + 1).ToString(), true, true);

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

        int preID = LocalDBController.Table<Puzzle>()
            .Where(p => p.CategoryID == _puzzle.CategoryID)
            .FirstOrDefault(p => p.Row == _puzzle.Row - 1)
            .ID;

        PlayPuzzles playPuzzles = LocalDBController.Table<PlayPuzzles>()
            .FirstOrDefault(pp => pp.PuzzleID == preID && pp.Success);

        return playPuzzles != null;
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
