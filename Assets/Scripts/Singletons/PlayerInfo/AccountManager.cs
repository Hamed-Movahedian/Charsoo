using UnityEngine;
using System.Collections;
using ArabicSupport;
using MgsCommonLib;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class AccountManager : MgsSingleton<AccountManager>
{
    private string _generatedCode;
    public bool IsConnected = false;

    public IEnumerator ConnectToAccount()
    {
        // Get window
        UIWindow getPhoneWindow = UIWindow.GetWindow("GetPhoneNumber");

        while (true)
        {
            // Show - Wait for action - Hide
            yield return getPhoneWindow.ShowWaitForActionHide();

            // Back to previous window
            if(getPhoneWindow.CheckLastAction("Back"))
                yield break;

            // Send code
            if (getPhoneWindow.CheckLastAction("SendCode"))
            {
                // Get phone number
                var phoneNumber = getPhoneWindow.GetComponentByName<InputField>("PhoneNumber").text;

                // Try to send code
                yield return SendCode(phoneNumber);

                // if connect to server return to previous window
                if (IsConnected)
                    yield break;

            }
        }
    }

    private IEnumerator SendCode(string phoneNumber)
    {
        #region Send code

        // Generate random number
        _generatedCode = Random.Range(1000, 9999).ToString();

        // Show inprogress window
        yield return UIWindow.ShowDialogue("InProgress", "در حال ارسال کد...");

        // Wait for 3 sec
        yield return new WaitForSeconds(3);

        // Hide inprogress window
        yield return UIWindow.Dialogue.Hide();

        #endregion

        // Get GetCode Window
        var getCodeWindow = UIWindow.GetWindow("GetCode");

        // initialize window
        getCodeWindow.GetComponentByName<Text>("Label").text =
            _generatedCode + " " + ArabicFixer.Fix("لطفا کد دریافتی را وارد کنید: ");

        while (true)
        {
            // Show and Wait for action
            yield return getCodeWindow.ShowWaitForActionHide();

            if (getCodeWindow.CheckLastAction("Back"))
                yield break;

            if (getCodeWindow.CheckLastAction("ValidateCode"))
            {
                // get input code
                var inputCode = getCodeWindow
                    .GetComponentByName<InputField>("InputCode").text;

                // Check input code is correct?
                if (inputCode == _generatedCode)
                {
                    // ----------------------------- correct code
                    #region Try to restore account

                    // show inprogress window
                    yield return UIWindow.ShowDialogue("InProgress", "در حال بازیابی حساب کاربری...");

                    // restore account from server
                    yield return RestoreAccountFromServer(phoneNumber);

                    // hide inprogress window
                    yield return UIWindow.Dialogue.Hide();

                    #endregion

                    // Restore successfully => Back to previous window
                    if (IsConnected)
                        yield break;

                    // Fail to Restore
                    if (!IsConnected)
                    {
                        // Inform user
                        yield return UIWindow.ShowDialogueWaitHide(
                             "GeneralDialogue",
                             "امکان بازیابی حساب وجود ندارد",
                             "بازگشت", null, null);

                        // Stay in this window
                    }

                }
                else
                {
                    // Incorrect code 
                    // Inform user
                    yield return UIWindow.ShowDialogueWaitHide(
                        "GeneralDialogue",
                        "کد وارد شده صحیح نمیباشد",
                        "بازگشت", null, null);

                    // Stay in this window
                }
            }
        }

    }


    private IEnumerator RestoreAccountFromServer(string phoneNumber)
    {
        yield return new WaitForSeconds(4);
        IsConnected = true;
    }
}
