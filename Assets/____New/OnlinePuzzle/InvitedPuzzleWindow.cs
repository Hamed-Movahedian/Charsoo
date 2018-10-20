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
    public Text Data;



    public override void Refresh()
    {
        base.Refresh();
        Clue.text = PersianFixer.Fix((string) OnlinePuzzleController.Instance.ServerRespond["Clue"]);
        Creator.text = PersianFixer.Fix((string)OnlinePuzzleController.Instance.ServerRespond["Creator"]);
        Data.text= Data.text.Replace("****", PersianFixer.Fix((string)OnlinePuzzleController.Instance.ServerRespond["Sender"]));
    }
}
