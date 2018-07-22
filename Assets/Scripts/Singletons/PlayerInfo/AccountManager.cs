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
    public enum SendCodeResultEnum
    {
        NotRegister,
        NetworkError,
        SmsServiceError,
        Success,
        InvalidPhoneNumber
    }

    public enum AccountConnectionResultEnum
    {
        NetworkError,
        AccountError,
        Success
    }

    #endregion

    #region Fields

    // Is successfully connect to an account
    public bool IsConnected = false;

    // Send code via sms result
    public SendCodeResultEnum SendCodeResult;

    // connect to account result
    public AccountConnectionResultEnum AccountConnectionResult;

    // cache random code and phone number
    private string _generatedCode;
    private string _phoneNumber;


    #endregion

    #region Send code via SMS

    public IEnumerator SendRandomCodeToPhoneNumber(string phoneNumber)
    {
        // Save phone number for later use
        _phoneNumber = phoneNumber;

        // Generate random number
        _generatedCode = Random.Range(1000, 9999).ToString();

        // Ask server to send sms
        yield return ServerController.Post<string>(
            string.Format(@"Account/SendSms?phoneNumber={0}&code={1}",
                phoneNumber, _generatedCode),
            null,
            // On success
            respond =>
            {
                // Set error code base on respond
                switch (respond.ToLower())
                {
                    case "notregister":
                        SendCodeResult = SendCodeResultEnum.NotRegister;
                        break;
                    case "nosmsservice":
                        SendCodeResult = SendCodeResultEnum.SmsServiceError;
                        break;
                    case "invalidphonenumber":
                        SendCodeResult = SendCodeResultEnum.InvalidPhoneNumber;
                        break;
                    case "ok":
                        SendCodeResult = SendCodeResultEnum.Success;
                        break;
                }
            },
            // On ERROR !!!
            request =>
            {
                // Set default error code to network error
                SendCodeResult = SendCodeResultEnum.NetworkError;
            });

    }

    #endregion

    #region Check code validation

    public bool IsCodeValid(string inputCode)
    {
        return inputCode == _generatedCode;
    }

    #endregion

    #region Connect to account via phone number

    public IEnumerator ConnectToAccount()
    {
        // Ask command center to connect to account
        yield return ServerController.Post<PlayerInfo>(
            string.Format(@"Account/ConnectToAccount?phoneNumber={0}", _phoneNumber),
            null,
            // On Successfully connect to the account
            playerInfo =>
            {
                // Set player info and save to local DB
                Singleton.Instance.PlayerController
                    .SetPlayerInfoAndSaveTolocalDB(playerInfo);

                // Account is connected
                IsConnected = true;

                // Set connection result to success
                AccountConnectionResult = AccountConnectionResultEnum.Success;
            },
            // On Error
            request =>
            {
                // Network Error !!!!!
                if (request.isNetworkError)
                    AccountConnectionResult = AccountConnectionResultEnum.NetworkError;
                // Account recovery Error !!!!
                else if (request.isHttpError)
                    AccountConnectionResult = AccountConnectionResultEnum.AccountError;
            });
    }

    #endregion
}
