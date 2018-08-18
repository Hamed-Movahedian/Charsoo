using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LanguagePack", menuName = "CharsooAssets/LanguagePack", order = 1)]
public class LanguagePack : ScriptableObject
{
    [Header("Messages")]
    [Multiline] public string SuccesfullOperation;

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

    [Header("Texts")]
    [Multiline] public string NotRegister;
    [Multiline] public string InReview;
    [Multiline] public string NoCategory;

    [Multiline] public string NotRegisterFull;
    [Multiline] public string InReviewFull;
    [Multiline] public string NoCategoryFull;
    [Multiline] public string UserPuzzleAcceptedFull;
    [Multiline] public string LockPuzzle;
}

public enum LanguagePackLables
{
    SuccesfullOperation,

    Inprogress_AccountRecovery,
    Inprogress_AccountConnection,
    Inprogress_GenerateWordSet,
    Inprogress_PartitionWordSet,

    Error_InternetAccess,
    Error_SmsService,
    Error_UnknownPhoneNumber,
    Error_AccountRecovery,
    Error_InvalidCode,
    Error_InvalidPhoneNumber,
    Error_GenerateWordSet,
    Error_NoCategoryIsSelected,

    NotRegister,
    InReview,
    NoCategory,

    NotRegisterFull,
    InReviewFull,
    NoCategoryFull,
    UserPuzzleAcceptedFull,
}



