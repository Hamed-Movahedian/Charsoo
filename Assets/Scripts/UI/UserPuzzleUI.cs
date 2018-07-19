using System.Collections;
using System.Collections.Generic;

public class UserPuzzleUI
{
    public IEnumerator ShowPuzzleSelectionWindow()
    {
        throw new System.NotImplementedException();
    }

    public bool Back { get; set; }
    public ResultEnum Result { get; set; }

    public enum ResultEnum
    {
        Play,Share,Register,Add
    }

    public UserPuzzle GetSelectedPuzzle()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ShowPuzzleInfo(UserPuzzle puzzle)
    {
        throw new System.NotImplementedException();
    }

    public void InitializeUserPuzzleSelectinWindow(List<UserPuzzle> userPuzzles)
    {
        throw new System.NotImplementedException();
    }

    public void ShowSyncInProgress()
    {
        throw new System.NotImplementedException();
    }

    public void HideSyncInProgress()
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ShowSyncResult(bool result)
    {
        throw new System.NotImplementedException();
    }
}