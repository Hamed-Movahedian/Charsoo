using ArabicSupport;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class UserPuzzleInfoWindow : MgsUIWindow
{
    public Text Clue;
    public Text Description;
    public Image RateImage;
    public Text PlayCount;
    public Button RegisterButton;
    public Button ShareButton;

    public void Refresh(UserPuzzle puzzle)
    {
        Clue.text = ArabicFixer.Fix(puzzle.Clue);

        RegisterButton.interactable = puzzle.ServerID == null;
        ShareButton.interactable = puzzle.ServerID != null;

        RateImage.gameObject.SetActive(!string.IsNullOrEmpty(puzzle.CategoryName));
        PlayCount.gameObject.SetActive(!string.IsNullOrEmpty(puzzle.CategoryName));

        if (puzzle.ServerID == null)
            Description.text = ThemeManager.Instance.LanguagePack.NotRegisterFull;
        else if (puzzle.CategoryName == null)
            Description.text = ThemeManager.Instance.LanguagePack.InReviewFull;
        else if (puzzle.CategoryName == "")
            Description.text = ThemeManager.Instance.LanguagePack.NoCategoryFull;
        else
        {
            PlayCount.text = ArabicFixer.Fix("نفر") + puzzle.PlayCount;
            if (puzzle.Rate != null) RateImage.fillAmount = puzzle.Rate.Value / 5f;
            Description.text = ThemeManager.Instance.LanguagePack.
                UserPuzzleAcceptedFull.Replace("***",ArabicFixer.Fix(puzzle.CategoryName));
        }
    }
}