using System;
using System.Collections;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class InputCodeWindow : UIWindowBase
{
    public InputField CodeInputField;


    public IEnumerator ValidateCode()
    {
        // Hide get code window
        yield return Hide();

        // Check is code valid
        if (AccountManager.Instance.IsCodeValid(CodeInputField.text))
        {
            // ************** Code is valid => Restore account

            // Display in progress window
            yield return UIController.ShowInprogressWindow(LanguagePack.Inprogress_AccountConnection);

            // Try to connect to account
            yield return AccountManager
                .Instance.ConnectToAccount();

            // Hide in-progress window
            yield return UIController
                .HideInprogressWindow();

            // Switch connection result
            switch (AccountManager.Instance.AccountConnectionResult)
            {
                case AccountManager.AccountConnectionResultEnum.NetworkError:
                    yield return UIController
                        .DisplayError(LanguagePack.Error_InternetAccess,IconPack.NetworkError);
                    break;
                case AccountManager.AccountConnectionResultEnum.AccountError:
                    yield return UIController
                        .DisplayError(LanguagePack.Error_AccountRecovery,IconPack.GeneralError);
                    break;
                case AccountManager.AccountConnectionResultEnum.Success:
                    // Display proper message
                    yield return UIController
                        .DisplayMessage(LanguagePack.SuccesfullAccountRecovery);

                    // Close window
                    Close();

                    // Exit method
                    yield break;
            }

        }
        else
        {
            // *************** Code isn't valid

            // Invalid code Error
            yield return UIController.DisplayError(
                LanguagePack.Error_InvalidCode,
                IconPack.InvalidCode);
        }

        // Show Input code window again
        yield return Show();
    }

}