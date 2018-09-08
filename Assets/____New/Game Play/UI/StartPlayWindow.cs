using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class StartPlayWindow : MgsUIWindow
{
    public Text Clue;
    public Text Index;

    public override void Refresh()
    {
        Clue.text = Singleton.Instance.WordSpawner.Clue;
        Index.text = Singleton.Instance.WordSpawner.PuzzleRow;
    }
}
