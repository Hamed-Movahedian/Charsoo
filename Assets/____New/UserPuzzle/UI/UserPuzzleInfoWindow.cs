﻿using ArabicSupport;
using FollowMachineDll.Attributes;
using MgsCommonLib.Theme;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class UserPuzzleInfoWindow : MgsUIWindow
{
    public UserPuzzleSelectionWindow PuzzleSelectionWindow;

    public Text Clue;
    public Text Description;
    public Image RateImage;
    public Text PlayCount;
    public Button RegisterButton;
    public Button ShareButton;

    public override void Refresh()
    {
        var puzzle = PuzzleSelectionWindow.SelectedPuzzle;

        Clue.text = ArabicFixer.Fix(puzzle.Clue);

        RegisterButton.interactable = puzzle.ServerID == null;
        ShareButton.interactable = puzzle.ServerID != null;

        RateImage.gameObject.SetActive(!string.IsNullOrEmpty(puzzle.CategoryName));
        PlayCount.gameObject.SetActive(!string.IsNullOrEmpty(puzzle.CategoryName));

        if (puzzle.ServerID == null)
            Description.text = ThemeManager.Instance.LanguagePack.GetLable("NotRegisterFull");
        else if (puzzle.CategoryName == null)
            Description.text = ThemeManager.Instance.LanguagePack.GetLable("InReviewFull");
        else if (puzzle.CategoryName == "")
            Description.text = ThemeManager.Instance.LanguagePack.GetLable("NoCategoryFull");
        else
        {
            PlayCount.text = ArabicFixer.Fix("نفر") + puzzle.PlayCount;
            if (puzzle.Rate != null) RateImage.fillAmount = puzzle.Rate.Value / 5f;
            Description.text = ThemeManager.Instance.LanguagePack.
                GetLable("UserPuzzleAcceptedFull").Replace("***",ArabicFixer.Fix(puzzle.CategoryName));
        }
    }

    [FollowMachine("Prepare selected puzzle for spawn")]
    public void SetForSpawn()
    {
        var selectedPuzzle = PuzzleSelectionWindow.SelectedPuzzle;

        var json = StringCompressor.DecompressString(selectedPuzzle.Content);

        WordSet wordSet = new WordSet();

        JsonUtility.FromJsonOverwrite(json, wordSet);

        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.Clue = selectedPuzzle.Clue;
        Singleton.Instance.WordSpawner.PuzzleRow = "";

        Singleton.Instance.WordSpawner.EditorInstatiate = null;
    }
}