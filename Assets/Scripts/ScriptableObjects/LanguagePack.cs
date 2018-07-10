using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LanguagePack", menuName = "CharsooAssets/LanguagePack", order = 1)]
public class LanguagePack : ScriptableObject
{
    [Header("Messages")]
    [Multiline] public string SuccesfullAccountRecovery;

    [Header("In-Progress Messages")]
    [Multiline] public string Inprogress_AccountRecovery;
    [Multiline] public string Inprogress_AccountConnection;
    [Multiline] public string Inprogress_GenerateWordSet;
    [Multiline] public string Inprogress_PartitionWordSet;

    [Header("Errors")]
    [Multiline] public string Error_InternetAccess;
    [Multiline] public string Error_SmsService;
    [Multiline] public string Error_UnknownPhoneNumber;
    [Multiline] public string Error_AccountRecovery;
    [Multiline] public string Error_InvalidCode;
    [Multiline] public string Error_InvalidPhoneNumber;
    [Multiline] public string Error_GenerateWordSet;
    [Multiline] public string Error_NoCategoryIsSelected;
}

