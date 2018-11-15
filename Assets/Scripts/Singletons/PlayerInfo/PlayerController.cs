using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

        yield return ServerController.Post<int>(
            @"/api/Login/AddRange",
            logIns,
            r =>
            {
                if (logIns.Count == r)
                    LocalDBController.DeleteAll<LogIn>();
            });
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
        // Register player to server and get PlayerID
        yield return ServerController.Post<PlayerInfo>(
            @"PlayerInfo/Create",
            _playerInfo,
            r => { _playerInfo = r; });

        if (_playerInfo.PlayerID == -1)
            _playerInfo.PlayerID = null;
        else
            OnNewPlayerID();
    }

    public void RegisterPlayerAsync()
    {
        StartCoroutine(RegisterPlayer());
    }
    #endregion

    #region CreatePlayerInfo 
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

        LocalDBController
            .InsertOrReplace(new PlayerInfo
            {
                Name = playerName,
            });
    }

    #endregion

    #region IsValidName
    [FollowMachine("Valid Name?", "Yes,No")]
    public void IsValidName(string playerName)
    {
        FollowMachine.SetOutput(playerName == null || playerName.Trim() == "" ? "No" : "Yes");
    }

    #endregion    [FollowMachine("Check Player Info ?", "No Player Info,No Player ID,Has Player ID")]

    #region CheckPlayerInfo
    [FollowMachine("Check Player Info", "No Player Info,No Player ID,Has Player ID")]
    public void CheckPlayerInfo()
    {
        _playerInfo =
            LocalDBController
                .Table<PlayerInfo>()
                .FirstOrDefault();

        if (_playerInfo == null)
            FollowMachine.SetOutput("No Player Info");
        else if (_playerInfo.PlayerID == null)
            FollowMachine.SetOutput("No Player ID");
        else
            FollowMachine.SetOutput("Has Player ID");
    }

    #endregion

    public IEnumerator SyncPlayerInfo()
    {
        LocalDBController.DeleteAll<PlayerInfo>();
        LocalDBController.InsertOrReplace(_playerInfo);

        if (_playerInfo.PlayerID == null)
            yield return RegisterPlayerToServer();
        if (_playerInfo.PlayerID != null)
            yield return ServerController.Post<string>(
                $@"PlayerInfo/Update?id={GetPlayerID}",
                _playerInfo,
                // On Successfully connect to the account
                respnse =>
                {
                    if (respnse == "Success") _playerInfo.Dirty = false;
                });

        LocalDBController.DeleteAll<PlayerInfo>();
        LocalDBController.InsertOrReplace(_playerInfo);
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
}
