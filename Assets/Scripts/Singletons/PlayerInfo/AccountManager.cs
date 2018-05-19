using UnityEngine;
using System.Collections;
using ArabicSupport;
using MgsCommonLib;
using MgsCommonLib.UI;
using UnityEngine.UI;

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

 
    public IEnumerator SendRandomCodeToPhonenumber(string phoneNumber)
    {
        // Save phone number for later use
        _phoneNumber = phoneNumber;

        // Generate random number
        _generatedCode = Random.Range(1000, 9999).ToString();

        // Wait for 3 sec (for test!!)
        yield return new WaitForSeconds(3);

        // Set error code base on phone number (for test!!)
        switch (phoneNumber[0])
        {
            case '0':
                SendCodeResault = SendCodeResaultEnum.NetworkError;
                break;
            case '1':
                SendCodeResault = SendCodeResaultEnum.NotRegister;
                break;
            case '2':
                SendCodeResault = SendCodeResaultEnum.SmsServiceError;
                break;
            default:
                SendCodeResault = SendCodeResaultEnum.Success;
                break;
        }
    }

    public bool IsCodeValid(string inputCode)
    {
        return inputCode == _generatedCode;
    }

    public IEnumerator ConnectToAccount()
    {
        // Wait for 4 sec... (for test!!!)
        yield return new WaitForSeconds(4);

        // Set error code base on phone number (for test!!)
        switch (_phoneNumber[1])
        {
            case '0':
                AccountConnectionResult=AccountConnectionResultEnum.AccountError;
                IsConnected = false;
                break;
            case '1':
                AccountConnectionResult=AccountConnectionResultEnum.NetworkError;
                IsConnected = false;
                break;
            default:
                AccountConnectionResult=AccountConnectionResultEnum.Success;
                Singleton.Instance.PlayerController.Name = "نام بازیابی شده";
                IsConnected = true;
                break;
        }
    }
}
