using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class GetWords : MgsUIWindow
{
    public InputField WordsText;
    public override void Refresh()
    {
        WordsText.text = "اصغر اکبر ابراهیم مرتضی مصطفی";
    }
}
