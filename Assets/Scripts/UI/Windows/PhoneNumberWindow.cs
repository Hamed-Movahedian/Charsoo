using System.Collections;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class PhoneNumberWindow : UIWindow
{
    public InputField PhoneNumberInputField;

    private LanguagePack LanguagePack
    {
        get { return LanguageManager.Instance.LanguagePack; }
    }
    private UIController UIController { get { return UIController.Instance; } }

    public IEnumerator SendCodeButton()
    {
        // Hide phone number window
        yield return Hide();

        // Show in-progress window
        yield return UIController.ShowInprogressWindow(LanguagePack.Inprogress_AccountRecovery);

        // Send Random code to phone number
        yield return AccountManager
            .Instance
            .SendRandomCodeToPhonenumber(PhoneNumberInputField.text);

        // Hide in-progress window
        yield return UIController.HideInprogressWindow();

        // Switch Send code result
        switch (AccountManager.Instance.SendCodeResault)
        {
            case AccountManager.SendCodeResaultEnum.NotRegister:
                yield return UIController.DisplayError(LanguagePack.Error_UnknownPhoneNumber);
                break;

            case AccountManager.SendCodeResaultEnum.NetworkError:
                yield return UIController.DisplayError(LanguagePack.Error_InternetAccess);
                break;

            case AccountManager.SendCodeResaultEnum.SmsServiceError:
                yield return UIController.DisplayError(LanguagePack.Error_SmsService);
                break;

            case AccountManager.SendCodeResaultEnum.Success:
                yield return UIController.InputCodeWindow.ShowWaitForCloseHide();

                if (AccountManager.Instance.IsConnected)
                {
                    Close();
                    yield break;
                }
                break;
        }

        // Show phone number window in case of error
        yield return Show();
    }

}