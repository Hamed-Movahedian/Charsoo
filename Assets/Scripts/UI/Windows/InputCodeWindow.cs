using System;
using System.Collections;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class InputCodeWindow : UIWindow
{
    public InputField CodeInputField;

    private LanguagePack LanguagePack
    {
        get { return LanguageManager.Instance.LanguagePack; }
    }
    public UIController UIController { get { return UIController.Instance; } }


    public IEnumerator ValidateCode()
    {
        // Hide get code window
        yield return Hide();

        // Check is code valid
        if (AccountManager.Instance.IsCodeValid(CodeInputField.text))
        {
            // ************** Code is valid => Restore account

            // Display inprogress window
            yield return UIController.ShowInprogressWindow(LanguagePack.Inprogress_AccountConnection);

            // Try to connect to account
            yield return AccountManager
                .Instance.ConnectToAccount();

            // Switch connection result
            switch (AccountManager.Instance.AccountConnectionResult)
            {
                case AccountManager.AccountConnectionResultEnum.NetworkError:
                    yield return UIController.DisplayError(LanguagePack.Error_InternetAccess);
                    break;
                case AccountManager.AccountConnectionResultEnum.AccountError:
                    yield return UIController.DisplayError(LanguagePack.Error_AccountRecovery);
                    break;
                case AccountManager.AccountConnectionResultEnum.Success:
                    // Display proper message
                    yield return UIController.DisplayMessage(LanguagePack.SuccesfullAccountRecovery);

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
            yield return UIController.DisplayError(LanguagePack.Error_InvalidCode);
        }

        // Show Input code window again
        yield return Show();
    }

}