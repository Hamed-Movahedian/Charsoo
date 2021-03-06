﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FMachine;
using FollowMachineDll.Attributes;
using FollowMachineDll.Components;
using MgsCommonLib;
using MgsCommonLib.UI;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : BaseObject
{
    #region Public

    private PlayerInfo _playerInfo;

    #endregion

    #region Properties


    #endregion

    #region New PlayerID Event

    public delegate void NewPlayerIDEventHandler(int newPlayerID);

    public event NewPlayerIDEventHandler NewPlayerID;

    public virtual void OnNewPlayerID()
    {
        LocalDBController
            .DataService
            .Connection.DeleteAll<PlayerInfo>();

        LocalDBController
            .InsertOrReplace(_playerInfo);

        if (_playerInfo.PlayerID != null)
            NewPlayerID?.Invoke(_playerInfo.PlayerID.Value);
    }

    #endregion

    #region LogIn

    private IEnumerator LoginToDB()
    {

        yield return LocalDBController.AddLogin(_playerInfo.PlayerID);

        if (_playerInfo.PlayerID != null)
            yield return FlashOutLocalLogins();

        NameText nameText = FindObjectOfType<NameText>();

        if (nameText != null)
            nameText.SetName(_playerInfo.Name);
    }

    private IEnumerator FlashOutLocalLogins()
    {
        List<LogIn> logIns =
            LocalDBController.Table<LogIn>().ToList();

        yield return Post<int>(
            @"/api/Login/AddRange",
            logIns,
            r =>
            {
                if (logIns.Count == r)
                    LocalDBController.DeleteAll<LogIn>();
            });
    }
    
    public static IEnumerator Post<TReturnType>(
        string url,
        object bodyData,
        Action<TReturnType> onSuccess,
        Action<UnityWebRequest> onError = null)
    {

        UnityWebRequest request = PostRequest(url, bodyData);
        Debug.Log(request.url);

        request.Send();
        Debug.Log("request sent");

        while (!request.isDone)
            yield return (object) null;
        if (!request.isHttpError && !request.isNetworkError)
        {
            Debug.Log("request No Error");

            if (onSuccess != null)
                onSuccess(JsonConvert.DeserializeObject<TReturnType>(request.downloadHandler.text));
        }
        else if (onError != null)
        {
            Debug.Log(request.error);

            onError(request);
        }
    }
    
    public static UnityWebRequest PostRequest(string url, object bodyData)
    {
        UnityWebRequest unityWebRequest = new UnityWebRequest("http://37.191.79.205:5351" + "/api/" + url, "POST", (DownloadHandler) new DownloadHandlerBuffer(), bodyData == null ? (UploadHandler) null : (UploadHandler) new UploadHandlerRaw(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(bodyData))));
        Debug.Log(unityWebRequest.url);

        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        return unityWebRequest;
    }

    
    

    #endregion

    #region Old Methods
    public void SaveToLocalDB()
    {
        LocalDBController
            .DataService
            .Connection.DeleteAll<PlayerInfo>();

        LocalDBController.InsertOrReplace(_playerInfo);
    }

    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        _playerInfo = playerInfo;
    }

    public void SetPlayerInfoAndSaveTolocalDB(PlayerInfo playerInfo)
    {
        SetPlayerInfo(playerInfo);
        SaveToLocalDB();
    }
    #endregion

    #region Playerinfo records
    public PlayerInfo PlayerInfo
    {
        get
        {
            if (_playerInfo == null)
                // Get player info from localDB
                _playerInfo =
                    LocalDBController
                        .Table<PlayerInfo>()
                        .FirstOrDefault();

            return _playerInfo;
        }
    }
    public int? GetPlayerID => PlayerInfo?.PlayerID;
    public string PlayerID => GetPlayerID?.ToString();

    public string PlayerName => PlayerInfo?.Name;
    public string PlayerTelephone => PlayerInfo?.Telephone;
    #endregion

    #region RegisterPlayerToServer
    [FollowMachine("Register To Server", "Success,Fail")]
    public IEnumerator RegisterPlayerToServer()
    {
        yield return RegisterPlayer();

        FollowMachine.SetOutput(_playerInfo?.PlayerID == null ? "Fail" : "Success");
    }

    private IEnumerator RegisterPlayer()
    {
        _playerInfo.PlayerID = -1;
        Debug.Log(_playerInfo.PlayerID);
        Debug.Log(_playerInfo);
        // Register player to server and get PlayerID
        yield return Post<PlayerInfo>(
            @"PlayerInfo/Create",
            _playerInfo,
            r => { _playerInfo = r; }, request =>
            {
                Debug.Log(request.error);
            });

        if (_playerInfo.PlayerID == -1)
        {
            Debug.Log("player not registered");

            _playerInfo.PlayerID = null;
        }
        else
        {
            OnNewPlayerID();
            Debug.Log("player registered with id : "+_playerInfo.PlayerID);

            string pusheId = PlayerPrefs.GetString("PID", "");

            if (pusheId == "")
                yield break;

            yield return ServerControllerBase.Post<string>(
                    $@"PushIDs/Update?playerID={_playerInfo.PlayerID}",
                    pusheId,
                    Debug.Log
                );
        }
    }


    public void RegisterPlayerAsync()
    {
        StartCoroutine(RegisterPlayer());
    }
    #endregion

    #region CreatePlayerInfo 

    public void CreateEmptyPlayer()
    {
        CreatePlayerInfo("بدون نام");
    }
    public void CreatePlayerInfo(string playerName)
    {
        LocalDBController
            .DataService
            .Connection
            .DeleteAll<PlayerInfo>();
        LocalDBController
            .DataService
            .Connection
            .DeleteAll<PlayPuzzles>();
        LocalDBController
            .DataService
            .Connection
            .DeleteAll<UserPuzzle>();
        LocalDBController
            .DataService
            .Connection
            .DeleteAll<Purchases>();

        ZPlayerPrefs.DeleteAll();

        _playerInfo = new PlayerInfo
        {
            CoinCount = 0,
            Name = playerName,
        };
        LocalDBController
            .InsertOrReplace(_playerInfo);
    }

    public void CreatePlayerInfo(InputField nameInputField)
    {
        CreatePlayerInfo(nameInputField.text);
    }
    #endregion

    #region IsValidName
    [FollowMachine("Valid Name?", "Yes,No")]
    public void IsValidName(string playerName)
    {
        FollowMachine.SetOutput(playerName == null || playerName.Trim() == "" ? "No" : "Yes");
    }

    [FollowMachine("Valid Name?", "Yes,No")]
    public void IsValidName(InputField playerName)
    {
        FollowMachine.SetOutput(playerName.text.Trim() == "بدون نام" || playerName.text == null || playerName.text.Trim() == "" ? "No" : "Yes");
    }

    #endregion

    #region CheckPlayerInfo
    [FollowMachine("Check Player Info", "No Player Info,No Player ID,Has Player ID")]
    public void CheckPlayerInfo()
    {

        _playerInfo =
            LocalDBController
                .Table<PlayerInfo>()
                .FirstOrDefault();

        if (_playerInfo == null)
        {
            FollowMachine.SetOutput("No Player Info");
        }
        else if (_playerInfo.PlayerID == null || _playerInfo.PlayerID == -1)
        {
            if (_playerInfo.PlayerID==-1)
            {
                _playerInfo.PlayerID = null;
                LocalDBController.DeleteAll<PlayerInfo>();
                LocalDBController.InsertOrReplace(_playerInfo);
            }
            FollowMachine.SetOutput("No Player ID");
        }
        else
            FollowMachine.SetOutput("Has Player ID");
    }

    #endregion

    public IEnumerator SyncPlayerInfo()
    {
        LocalDBController.DeleteAll<PlayerInfo>();
        LocalDBController.InsertOrReplace(_playerInfo);

        if (_playerInfo.PlayerID == null)
            yield return RegisterPlayer();
        if (_playerInfo.PlayerID != null)
        {
            yield return ServerController.Post<string>(
                $@"PlayerInfo/Update?id={GetPlayerID}",
                _playerInfo,
                // On Successfully connect to the account
                respnse =>
                {
                    if (respnse == "Success")
                    {
                        _playerInfo.Dirty = false;
                        LocalDBController.DeleteAll<PlayerInfo>();
                        LocalDBController.InsertOrReplace(_playerInfo);
                    }
                });

            string pusheId = FindObjectsOfType<Pushe>()[0].Pid;

            if (pusheId == "")
                Debug.Log("no push id");
            else
                yield return ServerControllerBase.Post<string>(
                    $@"PushIDs/Update?playerID={_playerInfo.PlayerID}",
                    pusheId,
                    Debug.Log
                );

        }


    }

    public IEnumerator ChangeCoinCount(int currentCoin)
    {
        _playerInfo.CoinCount = currentCoin;
        yield return SyncPlayerInfo();
    }

    public void HsyncPlayerInfo()
    {
        StartCoroutine(SyncPlayerInfo());
    }

    public void ChangePlayerInfo(PlayerInfo playerInfo)
    {
        if (PlayerInfo.PlayerID == playerInfo.PlayerID)
        {
            _playerInfo = playerInfo;
            _playerInfo.Dirty = true;
            StartCoroutine(SyncPlayerInfo());
        }
    }

    [FollowMachine("Has Name?", "Yes,No")]
    public void HasName()
    {
        FollowMachine.SetOutput((PlayerName.Trim() == "بدون نام" || PlayerName.Trim() == "") ? "No" : "Yes");
    }

    [FollowMachine("Has User Puzzle?", "Yes,No")]
    public void HasUserPuzzle()
    {
        FollowMachine.SetOutput(
            !LocalDBController.Table<UserPuzzle>().Any()&&
            LocalDBController.Table<PlayPuzzles>().Count()>10 &&
            Random.Range(0, 100) > 70 ? "Yes" : "No");
    }


    public void ChangeCoin(int amount)
    {
        var coinCount = _playerInfo.CoinCount;
        coinCount += amount;
        _playerInfo.CoinCount = coinCount;

        PurchaseManager.HcurrencyChanged(amount);
        
        StartCoroutine(SyncPlayerInfo());
    }

    public void SetName(InputField name)
    {
        string nameText = name.text;
        _playerInfo.Name = nameText;
        StartCoroutine(SyncPlayerInfo());
    }
}
