using ArabicSupport;
using UnityEngine.UI;

class UIUserPuzzleMenuItem : UIMenuItem
{
    public Text ClueText;
    public Image RateImage;
    public Text PlayCount;
    public Text State; 

    protected override void Refresh(object data)
    {
        var puzzle = (UserPuzzle)data;

        ClueText.text = ArabicFixer.Fix(puzzle.Clue);

        if (puzzle.ServerID == null)
        {
            RateImage.gameObject.SetActive(false);
            PlayCount.gameObject.SetActive(false);
            State.gameObject.SetActive(true);
            State.text = ThemeManager.Instance.LanguagePack.NotRegister;
        }
        else if(puzzle.CategoryName=="")
        {
            RateImage.gameObject.SetActive(false);
            PlayCount.gameObject.SetActive(false);
            State.gameObject.SetActive(true);
            State.text = ThemeManager.Instance.LanguagePack.InReview;
        }
        else
        {
            State.gameObject.SetActive(false);

            if(puzzle.PlayCount.HasValue && puzzle.PlayCount != 0)
            {
                RateImage.gameObject.SetActive(true);
                PlayCount.gameObject.SetActive(true);
                PlayCount.text = puzzle.PlayCount.ToString();
                if (puzzle.Rate != null) RateImage.fillAmount = puzzle.Rate.Value / 5f;
            }
            else
            {
                RateImage.gameObject.SetActive(false);
                PlayCount.gameObject.SetActive(false);
            }
        }
    }
}