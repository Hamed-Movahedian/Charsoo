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
        #region Initialize window

        UIWindow window = UIWindow.GetWindow("GetPhoneNumber");

        #endregion
     
        while (true)
        {
            // Show window
            yield return window.Show();

            // Wait for action
            yield return window.WaitForAction();

            switch (window.LastActionName)
            {
                case "Back":
                    // Hide window
                    yield return window.Hide();

                    // return
                    yield break;

                case "SendCode":
                    // Hide window
                    yield return window.Hide();

                    // Get phone number
                    var phoneNumber = window.GetComponentByName<InputField>("PhoneNumber").text;
                    // Try to send code
                    yield return SendCode(phoneNumber);


            }
        }
    }

    private IEnumerator SendCode(string phoneNumber)
    {

        #region Send code

        // Generate random number
        _generatedCode = Random.Range(1000, 9999).ToString();

        // Get Inprogress window
        var inProgressWindow = UIWindow.GetWindow("InProgress");

        // Initialize window
        inProgressWindow.GetComponentByName<Text>("Message").text =
            ArabicFixer.Fix("در حال ارسال کد...");

        // Show inprogress window
        yield return inProgressWindow.Show();

        // Wait for 3 sec
        yield return new WaitForSeconds(3);

        // Hide inprogress window
        yield return inProgressWindow.Hide();
        

        #endregion

        // Get GetCode Window
        var getCodeWindow = UIWindow.GetWindow("GetCode");

        // initialize window
        getCodeWindow.GetComponentByName<Text>("Label").text =
            _generatedCode + " " + ArabicFixer.Fix("لطفا کد دریافتی را وارد کنید: ");

        while (true)
        {
            // Show window
            yield return getCodeWindow.Show();

            // Wait for action
            yield return getCodeWindow.WaitForAction();

            // Switch action
            switch (getCodeWindow.LastActionName)
            {
                case "ValidateCode":

                    // get input code
                    var inputCode = getCodeWindow
                        .GetComponentByName<InputField>("InputCode").text;

                    // Check input code is correct?
                    if (inputCode == _generatedCode)
                    {
                        // ----------------------------- correct code

                        #region Try to restore account

                        // get inprogress window
                        var inprogressWindow = UIWindow.GetWindow("InProgress");

                        // initialize inprogress window
                        inprogressWindow.GetComponentByName<Text>("Message").text =
                            ArabicFixer.Fix("در حال بازیابی حساب کاربری...");

                        // show inprogress window
                        yield return inprogressWindow.Show();

                        // restore account from server
                        yield return RestoreAccountFromServer(phoneNumber);

                        // hide inprogress window
                        yield return inprogressWindow.Hide();

                        #endregion

                        #region Fail to restore account

                        UIWindow.GetWindow("")

                        #endregion

                        // Connect and restore successfully
                        if(IsConnected)
                            yield break;
                    }
                    else
                    {
                        // -------------------------- incorrect code

                        // get InvalidCode window
                        var invalidCodeWindow = UIWindow.GetWindow("UnvalidCode");

                        // Set Actions
                        invalidCodeWindow.SetAction("Back", Back);

                        // show invalid code and wait for close
                        yield return invalidCodeWindow.ShowAndWaitForClose();
                    }

                    break;
            }
        }
        // Wait to close window

    }


    private IEnumerator RestoreAccountFromServer(string phoneNumber)
    {
        yield return new WaitForSeconds(4);
        IsConnected = true;
    }
}
