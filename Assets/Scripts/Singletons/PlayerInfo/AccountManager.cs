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

    public SendCodeResaultEnum SendCodeResault;

    #endregion

    public bool IsConnected = false;

    private string _generatedCode;

    private IEnumerator RestoreAccountFromServer(string phoneNumber)
    {
        yield return new WaitForSeconds(4);
        IsConnected = true;
    }

    public IEnumerator SendRandomCodeToPhonenumber(string phoneNumber)
    {

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
            case '3':
                SendCodeResault = SendCodeResaultEnum.Success;
                break;
            default:
                break;
        }
    }
}
