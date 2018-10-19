using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackWindow : MgsUIWindow
{
    public Slider StarSlider;
    public Text ClueText;


    public override void Refresh()
    {
        StarSlider.value = 4f;
        ClueText.text = PersianFixer.Fix(Singleton.Instance.WordSpawner.Clue);
    }



}
