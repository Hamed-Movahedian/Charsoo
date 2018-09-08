using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class HUD : BaseObject
{
    public Text StartClueText;
    public Text HeaderClueText;
    public Text HintClueText;
    public GameObject StartEffect;
    public Text IndexText;
    public Text CoinText;
    
    private string _clue;

    public void SetupHUD()
    {
        _clue = WordSpawner.Clue;
        StartClueText.text = _clue;
        HeaderClueText.text = _clue;
        HintClueText.text = _clue;
        IndexText.text = WordSpawner.PuzzleRow;
        gameObject.SetActive(true);

    }

}
