using System.Collections;
using MgsCommonLib.UI;

public class PhoneNumberWindow : UIWindow
{
    public IEnumerator SendCodeButton()
    {
        // Hide phone number window
        return Hide();

        yield return UIController
            .Instance
            .DisplayInprogress("در حال ارسال کد...");
    }
}