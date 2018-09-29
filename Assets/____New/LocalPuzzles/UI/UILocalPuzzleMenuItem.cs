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
            IsAvalable(_puzzle) ? PersianFixer.Fix(_puzzle.Clue) :
            ThemeManager.Instance.LanguagePack.GetLable("LockPuzzle");

        GetComponent<Image>().color = IsAvalable(_puzzle) ? OpenColor : LockColor;
        Row.gameObject.SetActive(IsAvalable(_puzzle));
        LockIcon.gameObject.SetActive(!IsAvalable(_puzzle));

        SolvedIcon.gameObject.SetActive(
            LocalDBController.Table<PlayPuzzles>()
            .FirstOrDefault(pp => pp.PuzzleID == _puzzle.ID && pp.Success)!=null);

        Row.text = PersianFixer.Fix((_puzzle.Row + 1).ToString(), true, true);

        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    private bool IsAvalable(Puzzle puzzle)
    {
        if (puzzle.Paid|| puzzle.Row == 0)
            return true;

        var prePuzzle = LocalDBController
            .Table<Puzzle>()
            .SqlWhere(p => p.CategoryID == puzzle.CategoryID)
            .FirstOrDefault(p => p.Row == puzzle.Row - 1);

        if (prePuzzle == null)
            return true;

        int preID = prePuzzle.ID;

        PlayPuzzles playPuzzles = LocalDBController.Table<PlayPuzzles>()
            .FirstOrDefault(pp => pp.PuzzleID == preID && pp.Success);

        return playPuzzles != null;
    }

    public override void Select()
    {
        if (!IsAvalable(_puzzle))
        {
            ((LocalPuzzlesSelectionWindow)_list).LockSelect();
            return;
        }
        base.Select();
    }
}
