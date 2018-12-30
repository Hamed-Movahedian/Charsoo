using ArabicSupport;
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

        Clue.text = PersianFixer.Fix(puzzle.Clue);

        RegisterButton.interactable = puzzle.ServerID == null;
        ShareButton.interactable = puzzle.ServerID != null;

        RateImage.gameObject.SetActive(puzzle.PlayCount != null);
        PlayCount.gameObject.SetActive(puzzle.PlayCount != null);

        if (puzzle.ServerID == null)
            Description.text = ThemeManager.Instance.LanguagePack.GetLable("NotRegisterFull");
        else if (puzzle.CategoryName == "")
        {
            Description.text = ThemeManager.Instance.LanguagePack.GetLable("InReviewFull");
            Description.text = Description.text.Replace("**-**", PersianFixer.Fix(puzzle.ServerID.ToString()));
        }
        else if (puzzle.CategoryName == "-")
        {
            Description.text = ThemeManager.Instance.LanguagePack.GetLable("NoCategoryFull");
            Description.text = Description.text.Replace("**-**", PersianFixer.Fix(puzzle.ServerID.ToString()));
        }
        if (puzzle.ServerID != null)
        {
            PlayCount.text = PersianFixer.Fix(puzzle.PlayCount + " نفر ", true, true);
            if (puzzle.Rate != null) RateImage.fillAmount = puzzle.Rate.Value / 5f;
            if (puzzle.CategoryName != null && puzzle.CategoryName.Length > 1)
            {
                Description.text = ThemeManager.Instance.LanguagePack.
                  GetLable("UserPuzzleAcceptedFull").Replace("***", PersianFixer.Fix(puzzle.CategoryName));
                Description.text = Description.text.Replace("**-**", PersianFixer.Fix(puzzle.ServerID.ToString()));

            }
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
        Singleton.Instance.WordSpawner.PuzzleID = -1;
        Singleton.Instance.WordSpawner.PuzzleRow = "";

        Singleton.Instance.WordSpawner.EditorInstatiate = null;
    }

    public int? GetPuzzleID => PuzzleSelectionWindow.SelectedPuzzle.ServerID;

    public string GetPuzzleClue()
    {
        return PuzzleSelectionWindow.SelectedPuzzle.Clue;
    }
}