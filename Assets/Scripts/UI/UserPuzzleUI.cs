using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MgsCommonLib.UI;
using UnityEngine;

public class UserPuzzleUI : MonoBehaviour
{
    public UIMenuItemList UserPuzzlesListWindow;
    public MgsUIWindow PuzzleInfoWindow;

    private MgsUIWindow _currentWindow;

    public IEnumerator ShowPuzzleSelectionWindow()
    {
        _currentWindow = UserPuzzlesListWindow;
        yield return UserPuzzlesListWindow.ShowWaitForCloseHide();
    }

    public bool Back
    {
        get { return _currentWindow != null && _currentWindow.Result == "Back"; }
    }

    public ResultEnum Result
    {
        get
        {
            switch (_currentWindow.Result)
            {
                case "Play":
                    return ResultEnum.Play;
                case "Share":
                    return ResultEnum.Share;
                case "Register":
                    return ResultEnum.Register;
                case "Add":
                    return ResultEnum.Add;
                default:
                    return ResultEnum.None;
            }
        }
    }

    public enum ResultEnum
    {
        Play,Share,Register,Add,None
    }

    public UserPuzzle GetSelectedPuzzle()
    {
        return (UserPuzzle) UserPuzzlesListWindow.GetSelectedItem();
    }

    public IEnumerator ShowPuzzleInfo(UserPuzzle puzzle)
    {
        yield return PuzzleInfoWindow.ShowWaitForCloseHide();
    }

    public void InitializeUserPuzzleSelectinWindow(List<UserPuzzle> userPuzzles)
    {
        UserPuzzlesListWindow.UpdateItems(userPuzzles.Cast<object>());
    }

    public void ShowSyncInProgress()
    {
        StartCoroutine(UIController.Instance.ShowInprogressWindow(ThemeManager.Instance.LanguagePack.Inprogress_AccountConnection));
    }

    public IEnumerator HideSyncInProgress()
    {
        yield return UIController.Instance.HideInprogressWindow();
    }

    public IEnumerator ShowSyncResult(bool result)
    {
        if (result)
            yield return UIController.Instance.DisplayMessage(
                ThemeManager.Instance.LanguagePack.SuccesfullOperation);
        else
            yield return UIController.Instance.DisplayError(
                ThemeManager.Instance.LanguagePack.Error_InternetAccess,
                ThemeManager.Instance.IconPack.NetworkError);
    }
}