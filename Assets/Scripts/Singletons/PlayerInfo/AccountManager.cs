using System;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using Assets.Scripts.Singletons;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using UnityEngine;
using Random = UnityEngine.Random;

public class AccountManager : MgsSingleton<AccountManager>
{
    // cache random code and phone number
    private string _generatedCode;
    private string _phoneNumber;


    #region Send RandomCode To PhoneNumber
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
        GenerateRandomCode();

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
            });

        if (FollowMachine.CheckOutputLable("Success"))
            yield return new WaitForSeconds(3);

    }

    #endregion

    public void GenerateRandomCode()
    {
        _generatedCode = Random.Range(1000, 9999).ToString();
    }

    [FollowMachine("Is Code Valid?", "Yes,No")]
    public void IsCodeValid(string inputCode)
    {
        FollowMachine.SetOutput(inputCode == _generatedCode ? "Yes" : "No");
    }

    [FollowMachine("Connect To Account", "Success,Network Error,Account Error")]
    public IEnumerator ConnectToAccount()
    {
        AccountInfo accountInfo = null;
        // Ask command center to connect to account
        yield return ServerController.Post<AccountInfo>(
            $@"Account/ConnectToAccount?phoneNumber={_phoneNumber}",
            null,
            // On Successfully connect to the account
            info => { accountInfo = info; },
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

        if (accountInfo != null)
        {
            // Restore player info
            Singleton.Instance.PlayerController
                .SetPlayerInfoAndSaveTolocalDB(accountInfo.PlayerInfo);

            // Restore play history
            PlayPuzzleController.Instance.RestorePlayHistory(accountInfo.PlayPuzzleses);

            // Restore purchases
            PurchaseController.Instance.RestorePurchase(accountInfo.Purchaseses);

            // Restore user puzzles
            UserPuzzleSynchronizer.Instance.RestoreUserPuzzles(accountInfo.UserPuzzles);

            // Set connection result to success
            FollowMachine.SetOutput("Success");

        }
    }

    [FollowMachine("Set Phone Number","Success,Fail")]
    public IEnumerator SetPhoneNumber(string phoneNumber)
    {
        //PlayerInfo playerInfo = Singleton.Instance.PlayerController.PlayerInfo;

        /*yield return ServerController.Post<AccountInfo>(
            $@"Account/UpdatePlayerInfo",
            ,
            // On Successfully connect to the account
            info => { accountInfo = info; },
            // On Error
            request =>
            {
                // Network Error !!!!!
                if (request.isNetworkError)
                    FollowMachine.SetOutput("Network Error");

                // Account recovery Error !!!!
                else if (request.isHttpError)
                    FollowMachine.SetOutput("Account Error");
            });*/
        return null;
    }

    public class AccountInfo
    {
        public PlayerInfo PlayerInfo;
        public List<UserPuzzle> UserPuzzles;
        public List<PlayPuzzles> PlayPuzzleses;
        public List<Purchases> Purchaseses;
    }
}
