using UnityEngine;
using System.Collections;
using ArabicSupport;
using MgsCommonLib;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class AccountManager : MgsSingleton<AccountManager>
{
    private string _generatedCode;

    public IEnumerator ConnectToAccount()
    {
        #region Initialize window

        UIWindow window = UIWindow.GetWindow("GetPhoneNumber");

        #endregion

        #region Set Actions

        window.SetAction("SendCode", SendCode);
        window.SetAction("Back", Back);
        #endregion

        yield return window.ShowAndWaitForClose();
    }

    private IEnumerator Back(UIWindow window)
    {
        window.Close();
        yield return null;
    }

    private IEnumerator SendCode(UIWindow getPhoneNumberWindow)
    {
        // Hide GetPhoneNumber window
        yield return getPhoneNumberWindow.Hide();

        // Generate random number
        _generatedCode = Random.Range(1000, 9999).ToString();

        // Get Inprogress window
        var inProgressWindow = UIWindow.GetWindow("InProgress");

        // Initialize window
        inProgressWindow.GetComponentByName<Text>("Message").text =
            ArabicFixer.Fix("در حال ارسال کد...");

        // Show inprogress window
        inProgressWindow.Show();

        // Wait for 3 sec
        yield return new WaitForSeconds(3);

        // Hide inprogress window
        inProgressWindow.Hide();

        // Get GetCode Window
        var getCodeWindow = UIWindow.GetWindow("GetCode");

        // initialize window
        getCodeWindow.GetComponentByName<Text>("Label").text =
            _generatedCode + " " + ArabicFixer.Fix("لطفا کد دریافتی را وارد کنید: ");

        // Set actions
        getCodeWindow.SetAction("ValidateCode", ValidateCode);
        getCodeWindow.SetAction("Back", Back);
    }

    private IEnumerator ValidateCode(UIWindow window)
    {
        // get input code
        var inputCode = window.GetComponentByName<InputField>("PhoneNumber").text;

        // Check input code is correct?
        if (inputCode == _generatedCode)
        {
            // ----------------------------- correct code

            // get inprogress window
            var inprogressWindow = UIWindow.GetWindow("InProgress");

            // initialize inprogress window
            inprogressWindow.GetComponentByName<Text>("Message").text =
                ArabicFixer.Fix("در حال بازیابی حساب کاربری...");

            // show inprogress window
            yield return inprogressWindow.Show();

            // restore account from server
            yield return RestoreAccountFromServer();

            // hide inprogress window
            yield return inprogressWindow.Hide();

        }
        else
        {
            // -------------------------- incorrect code

            // get InvalidCode window
            var invalidCodeWindow = UIWindow.GetWindow("UnvalidCode");

            // Set Actions
            invalidCodeWindow.SetAction("Back",Back);

            // show invalid code and wait for close
            yield return invalidCodeWindow.ShowAndWaitForClose();
        }
    }

    private IEnumerator RestoreAccountFromServer()
    {
        yield return new WaitForSeconds(4);
    }
}
