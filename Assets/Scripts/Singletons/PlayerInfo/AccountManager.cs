using System;
using System.Collections;
using ArabicSupport;
using MgsCommonLib;
using MgsCommonLib.UI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AccountManager : MgsSingleton<AccountManager>
{
    #region Enums
    public enum SendCodeResaultEnum
    {
        NotRegister,
        NetworkError,
        SmsServiceError,
        Success
    }

    public enum AccountConnectionResultEnum
    {
        NetworkError,
        AccountError,
        Success
    }

    #endregion

    public bool IsConnected = false;

    public SendCodeResaultEnum SendCodeResault;

    public AccountConnectionResultEnum AccountConnectionResult;

    private string _generatedCode;
    private string _phoneNumber;

    #region Send code

    public IEnumerator SendRandomCodeToPhoneNumber(string phoneNumber)
    {
        // Save phone number for later use
        _phoneNumber = phoneNumber;

        // Generate random number
        _generatedCode = Random.Range(1000, 9999).ToString();

        // Set default error code to network error
        SendCodeResault = SendCodeResaultEnum.NetworkError;

        // Ask server to send sms
        yield return Server.Post<string>(
            string.Format(@"Account/SendSms?phoneNumber={0}&code={1}",
                phoneNumber, _generatedCode),
            null,
            respond =>
            {
                // Set error code base on respond
                switch (respond.ToLower())
                {
                    case "notregister":
                        SendCodeResault = SendCodeResaultEnum.NotRegister;
                        break;
                    case "nosmsservice":
                        SendCodeResault = SendCodeResaultEnum.SmsServiceError;
                        break;
                    case "ok":
                        SendCodeResault = SendCodeResaultEnum.Success;
                        break;
                }
            });

    }

    #endregion

    public bool IsCodeValid(string inputCode)
    {
        return inputCode == _generatedCode;
    }

    public IEnumerator ConnectToAccount()
    {
        // Set default result to network error
        AccountConnectionResult = AccountConnectionResultEnum.NetworkError;

        // Ask command center to connect to account
        yield return Server.Post<string>(
             string.Format(@"Commands/ConnectToAccount?phoneNumber={0}&lastCommandTime={1:s}",
                 _phoneNumber,
                 DateTime.Now),
             null,
             respond =>
             {
                 if (respond.ToLower() == "fail")
                     AccountConnectionResult = AccountConnectionResultEnum.AccountError;
                 else
                 {
                     Singleton.Instance.CommandController
                        .
                 }
             });
        // Set error code base on phone number (for test!!)
        switch (_phoneNumber[1])
        {
            case '0':
                AccountConnectionResult = AccountConnectionResultEnum.AccountError;
                IsConnected = false;
                break;
            case '1':
                AccountConnectionResult = AccountConnectionResultEnum.NetworkError;
                IsConnected = false;
                break;
            default:
                AccountConnectionResult = AccountConnectionResultEnum.Success;
                Singleton.Instance.PlayerController.Name = "نام بازیابی شده";
                IsConnected = true;
                break;
        }
    }
}
