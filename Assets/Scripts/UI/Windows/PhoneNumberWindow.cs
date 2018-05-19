using System.Collections;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class PhoneNumberWindow : UIWindowBase
{
    public InputField PhoneNumberInputField;
    
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
                yield return UIController
                    .DisplayError(
                        LanguagePack.Error_UnknownPhoneNumber,
                        IconPack.UnkownPhone);
                break;

            case AccountManager.SendCodeResaultEnum.NetworkError:
                yield return UIController
                    .DisplayError(
                        LanguagePack.Error_InternetAccess,
                        IconPack.NetworkError);
                break;

            case AccountManager.SendCodeResaultEnum.SmsServiceError:
                yield return UIController
                    .DisplayError(
                        LanguagePack.Error_SmsService,
                        IconPack.ServiceError);
                break;

            case AccountManager.SendCodeResaultEnum.Success:
                yield return UIController
                    .InputCodeWindow.ShowWaitForCloseHide();

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