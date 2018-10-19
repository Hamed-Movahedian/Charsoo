using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class HUD : BaseObject
{
    public Text StartClueText;
    public Text HeaderClueText;
    public Text HintClueText;
    public Text IndexText;
    
    private string _clue;

    public void SetupHUD()
    {
        _clue = PersianFixer.Fix(WordSpawner.Clue);
        StartClueText.text = _clue;
        HeaderClueText.text = _clue;
        HintClueText.text = _clue;
        IndexText.text = PersianFixer.Fix(WordSpawner.PuzzleRow);
    }

}
