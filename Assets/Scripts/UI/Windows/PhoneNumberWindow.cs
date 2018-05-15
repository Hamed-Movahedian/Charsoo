using System.Collections;
using MgsCommonLib.UI;
using UnityEngine.UI;

public class PhoneNumberWindow : UIWindow
{
    public InputField PhoneNumberInputField;

    public IEnumerator SendCodeButton()
    {
        // Hide phone number window
        yield return Hide();

        UIController _uiController = UIController
                    .Instance;

        // Show in-progress window
        yield return _uiController.ShowInprogressWindow("در حال ارسال کد...");

        // Send Random code to phone number
        yield return AccountManager
            .Instance
            .SendRandomCodeToPhonenumber(PhoneNumberInputField.text);

        // Hide in-progress window
        yield return _uiController.HideInprogressWindow();

        // Switch Send code result
        switch (AccountManager.Instance.SendCodeResault)
        {
            case AccountManager.SendCodeResaultEnum.NotRegister:
                _uiController.DisplayError("شماره واره شده قبلا ثبت نشده است!");
                break;

            case AccountManager.SendCodeResaultEnum.NetworkError:
                _uiController.DisplayError("خطا در دسترسی به اینترنت!");
                break;

            case AccountManager.SendCodeResaultEnum.SmsServiceError:
                _uiController.DisplayError("سرویس پیام کوتاه در حال حاظر در دسترس نیست!");
                break;

            case AccountManager.SendCodeResaultEnum.Success:
                _uiController.inputCodeWindow.ShowWaitForCloseHide();

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