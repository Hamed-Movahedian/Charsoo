using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InvitedPuzzleWindow : MgsUIWindow
{
    public Text Clue;
    public Text Creator;



    public override void Refresh()
    {
        base.Refresh();
        Clue.text = (string) OnlinePuzzleController.Instance.ServerRespond["Clue"];
        Creator.text = (string)OnlinePuzzleController.Instance.ServerRespond["Creator"];
    }
}
