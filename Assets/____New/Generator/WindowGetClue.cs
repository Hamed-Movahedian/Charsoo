using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class WindowGetClue :MgsUIWindow
{
    public InputField ClueInputField;

    [FollowMachine("Has Clue?", "Yes,No")]
    public void CheckClue()
    {
        FollowMachine.SetOutput(ClueInputField.text.Trim() == "" ? "No" : "Yes");
    }

    [FollowMachine("Clear clue")]
    public void ClearClue()
    {
        ClueInputField.text = "";
    }
}