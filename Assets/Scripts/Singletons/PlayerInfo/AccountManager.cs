using System;
using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using Assets.Scripts.Singletons;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib;
using Soomla.Store;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class AccountManager : MgsSingleton<AccountManager>
{
    // cache random code and phone number
    private string _generatedCode;
    private string _phoneNumber = " ";


    #region HasAccount

    [FollowMachine("Has Account", "Yes,No")]
    public void HasAccount()
    {
        PlayerInfo playerInfo = Singleton.Instance.PlayerController.PlayerInfo;

        if (playerInfo != null && playerInfo.HasAccount())
            FollowMachine.SetOutput("Yes");
        else
            FollowMachine.SetOutput("No");
    }
    #endregion

    #region Send RandomCode To PhoneNumber
    [FollowMachine("Send RandomCode To PhoneNumber", "Success,Not Register,No Sms Service,Invalid Phone Number,Network Error,Repetitive Number")]
    public IEnumerator SendRandomCodeToPhoneNumber(string phoneNumber, bool forRegister)
    {
        // Check phone number

        if (phoneNumber.Length != 11 || phoneNumber[0] != '0')
        {
            FollowMachine.SetOutput("Invalid Phone Number");
            yield break;
        }

        // Save phone number for later use
        if (phoneNumber == _phoneNumber)
        {
            FollowMachine.SetOutput("Success");
            yield return new WaitForSeconds(3);
            yield break;
        }


        // Generate random number
        GenerateRandomCode();

        // Ask server to send sms
        yield return ServerController.Post<string>(
            $@"Account/SendSms?phoneNumber={phoneNumber}&code={_generatedCode}&forRegister={forRegister}",
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
        {
            _phoneNumber = phoneNumber;
            yield return new WaitForSeconds(3);
        }

    }

    #endregion

    #region GenerateRandomCode
    public void GenerateRandomCode()
    {
        _generatedCode = Random.Range(1000, 9999).ToString();
    }
    #endregion

    #region IsCodeValid
    [FollowMachine("Is Code Valid?", "Yes,No")]
    public void IsCodeValid(string inputCode)
    {
        bool v = inputCode == _generatedCode;
        FollowMachine.SetOutput(v ? "Yes" : "No");
    }
    #endregion

    #region ConnectToAccount
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



            int balance = StoreInventory.GetItemBalance("charsoo_coin");
            if (balance > accountInfo.PlayerInfo.CoinCount)
            {
                PlayerPrefs.DeleteAll();
                StoreInventory.TakeItem("charsoo_coin", balance - accountInfo.PlayerInfo.CoinCount);
            }
            else if (accountInfo.PlayerInfo.CoinCount < balance)
            {
                PlayerPrefs.DeleteAll();
                StoreInventory.GiveItem("charsoo_coin", accountInfo.PlayerInfo.CoinCount - balance);
            }

            FollowMachine.SetOutput("Success");

            _phoneNumber = " ";
        }
    }

    #endregion

    #region SetPhoneNumber
    [FollowMachine("Register Phone Number", "Success,Fail,Network Error")]
    public IEnumerator RegisterPhoneNumber(string phoneNumber)
    {
        PlayerInfo playerInfo = Singleton.Instance.PlayerController.PlayerInfo;

        if (playerInfo == null)
        {
            FollowMachine.SetOutput("Fail");
            yield break;
        }

        var lastTelephone = playerInfo.Telephone;
        playerInfo.Telephone = phoneNumber;

        yield return ServerController.Post<string>(
            $@"PlayerInfo/Update?id={playerInfo.PlayerID}",
            playerInfo,
            // On Successfully connect to the account
            respnse =>
            {
                FollowMachine.SetOutput(respnse);
            },
            // On Error
            request =>
            {
                // Network Error !!!!!
                if (request.isNetworkError)
                    FollowMachine.SetOutput("Network Error");

                // Account recovery Error !!!!
                else if (request.isHttpError)
                    FollowMachine.SetOutput("Fail");
            });

        if (!FollowMachine.CheckOutputLable("Success"))
            playerInfo.Telephone = lastTelephone;

    }
    #endregion

    #region AccountInfo (for server data exchange)
    public class AccountInfo
    {
        public PlayerInfo PlayerInfo;
        public List<UserPuzzle> UserPuzzles;
        public List<PlayPuzzles> PlayPuzzleses;
        public List<Purchases> Purchaseses;
    }

    #endregion
}
