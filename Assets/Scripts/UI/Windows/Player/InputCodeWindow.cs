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
        {
            // ************** Code is valid => Restore account

            // Display in progress window
            yield return UIController.ShowInprogressWindow(LanguagePack.GetLable("Inprogress_AccountConnection"));

            // Try to connect to account
            yield return AccountManager
                .Instance.ConnectToAccount();

            // Hide in-progress window
            yield return UIController
                .HideInprogressWindow();

            // Switch connection result
            switch (AccountManager.Instance.AccountConnectionResult)
            {
                // Network Error !!!!!
                case AccountManager.AccountConnectionResultEnum.NetworkError:
                    yield return UIController
                        .DisplayError(LanguagePack.GetLable("Error_InternetAccess"),IconPack.GetIcon("NetworkError"));
                    break;
                // Account can't recover error !!!!!
                case AccountManager.AccountConnectionResultEnum.AccountError:
                    yield return UIController
                        .DisplayError(LanguagePack.GetLable("Error_AccountRecovery"),IconPack.GetIcon("GeneralError"));
                    break;
                // Successfully connect to account !!!!!
                case AccountManager.AccountConnectionResultEnum.Success:
                    // Display proper message
                    yield return UIController
                        .DisplayMessage(LanguagePack.GetLable("SuccesfullOperation"));

                    // Close window
                    Close();

                    // Exit method
                    yield break;
            }

        }
        {
            // *************** Code isn't valid

            // Invalid code Error
            yield return UIController.DisplayError(
                LanguagePack.GetLable("Error_InvalidCode"),
                IconPack.GetIcon("InvalidCode"));
        }

        // Show Input code window again
        yield return Show();
    }

}