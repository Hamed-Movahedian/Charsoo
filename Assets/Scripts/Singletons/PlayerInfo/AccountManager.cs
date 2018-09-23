using System;
using System.Collections;
using ArabicSupport;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using Random = UnityEngine.Random;

public class AccountManager : MgsSingleton<AccountManager>
{

    #region Fields

    // Is successfully connect to an account
    public bool IsConnected = false;

    // cache random code and phone number
    private string _generatedCode;
    private string _phoneNumber;


    #endregion

    [FollowMachine("Send RandomCode To PhoneNumber", "Success,Not Register,No Sms Service,Invalid Phone Number,Network Error")]
    public IEnumerator SendRandomCodeToPhoneNumber(string phoneNumber)
    {
        // Check phone number
        if (phoneNumber[0] != '0' || phoneNumber.Length != 11)
        {
            FollowMachine.SetOutput("Invalid Phone Number");
            yield break;
        }

        // Save phone number for later use
        _phoneNumber = phoneNumber;

        // Generate random number
        _generatedCode = Random.Range(1000, 9999).ToString();

        // Ask server to send sms
        yield return ServerController.Post<string>(
            $@"Account/SendSms?phoneNumber={phoneNumber}&code={_generatedCode}",
            null,
            // On success
            respond =>
            {
                // Set error code base on respond
                FollowMachine.SetOutput(respond);
            },
            // On ERROR !!!
            request =>
            {
                // Set default error code to network error
                FollowMachine.SetOutput("Network Error");
                SendCodeResult = SendCodeResultEnum.NetworkError;
            });

    }

    [FollowMachine("Is Code Valid?", "Yes,No")]
    public void IsCodeValid(string inputCode)
    {
        FollowMachine.SetOutput(inputCode == _generatedCode ? "Yes" : "No");
    }

    [FollowMachine("Connect To Account", "Success,Network Error,Account Error")]
    public IEnumerator ConnectToAccount()
    {
        // Ask command center to connect to account
        yield return ServerController.Post<PlayerInfo>(
            $@"Account/ConnectToAccount?phoneNumber={_phoneNumber}",
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
                FollowMachine.SetOutput("Success");
            },
            // On Error
            request =>
            {
                // Network Error !!!!!
                if (request.isNetworkError)
                    FollowMachine.SetOutput("Network Error");

                // Account recovery Error !!!!
                else if (request.isHttpError)
                    FollowMachine.SetOutput("Account Error");
            });
    }
}
