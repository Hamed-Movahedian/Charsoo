using UnityEngine;
using System.Collections;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class PlayerInfoEditor : UIWindow
{
    #region pubic

    public PhoneNumberWindow PhoneNumberWindow;

    public InputField NameInputField;
    public Button AccountButton;

    #endregion

    public IEnumerator ConnectToAccountButton()
    {
        // Hide this window
        yield return Hide();

        // Show phone number window => wait for close => Hide
        yield return PhoneNumberWindow.ShowWaitForCloseHide();

        // if connected refresh window
        if (AccountManager.Instance.IsConnected)
            UpdateWindow();

        // Show window
        yield return Show();
    }

    private void UpdateWindow()
    {
        NameInputField.text = Singleton.Instance.PlayerController.Name;
        AccountButton.interactable = !AccountManager.Instance.IsConnected;
    }

    public void EnterGameButton()
    {
        // Save player info
        Singleton.Instance.PlayerController.Name = NameInputField.text;
        Singleton.Instance.PlayerController.SaveToLocalDB();

        // close window and return to caller
        Close();
    }

    public IEnumerator EditPlayerInfo()
    {
        UpdateWindow();
        yield return ShowWaitForCloseHide();
    }
}