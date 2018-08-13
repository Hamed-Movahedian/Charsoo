using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
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


    protected override void Refresh(object data)
    {
        var puzzle = (Puzzle)data;

        ClueText.text =
            !puzzle.Paid? ThemeManager.Instance.LanguagePack.LockPuzzle:
            ArabicFixer.Fix(puzzle.Clue);

        GetComponent<Image>().color = puzzle.Paid ? OpenColor : LockColor;
        Row.gameObject.SetActive(puzzle.Paid);
        LockIcon.gameObject.SetActive(!puzzle.Paid);
        SolvedIcon.gameObject.SetActive(puzzle.Solved);
        Row.text = puzzle.Row.ToString();

        GetComponent<RectTransform>().localScale = Vector3.one;
    }


}
