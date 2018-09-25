using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : BaseObject
{
    #region Public

    private PlayerInfo PlayerInfo;

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
            .InsertOrReplace(PlayerInfo);

        if (PlayerInfo.PlayerID != null)
            NewPlayerID?.Invoke(PlayerInfo.PlayerID.Value);
    }

    #endregion

    #region LogIn

    private IEnumerator LoginToDB()
    {

        yield return LocalDBController.AddLogin(PlayerInfo.PlayerID);

        if (PlayerInfo.PlayerID != null)
            yield return FlashOutLocalLogins();

        NameText nameText = FindObjectOfType<NameText>();

        if (nameText != null)
            nameText.SetName(PlayerInfo.Name);
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


    public void SaveToLocalDB()
    {
        LocalDBController
            .DataService
            .Connection.DeleteAll<PlayerInfo>();

        LocalDBController.InsertOrReplace(PlayerInfo);
    }

    public void SetPlayerInfo(PlayerInfo playerInfo)
    {
        PlayerInfo = playerInfo;
    }

    public void SetPlayerInfoAndSaveTolocalDB(PlayerInfo playerInfo)
    {
        SetPlayerInfo(playerInfo);
        SaveToLocalDB();
    }

    public int? GetPlayerID
    {
        get
        {
            // Get player info from localDB
            PlayerInfo =
                LocalDBController
                    .Table<PlayerInfo>()
                    .FirstOrDefault();
            
            return PlayerInfo?.PlayerID;
        }
    }

    [FollowMachine("Register To Server", "Success,Fail")]
    public IEnumerator RegisterPlayerToServer()
    {
        PlayerInfo.PlayerID = -1;
        // Register player to server and get PlayerID
        yield return ServerController.Post<PlayerInfo>(
            @"PlayerInfo/Create",
            PlayerInfo,
            r => { PlayerInfo = r; });
        if (PlayerInfo.PlayerID == -1)
            PlayerInfo.PlayerID = null;

        FollowMachine.SetOutput(PlayerInfo?.PlayerID == null ? "Fail" : "Success");
    }

    public void CreatePlayerInfo(string playerName)
    {
        LocalDBController
            .InsertOrReplace(new PlayerInfo
            {
                Name = playerName,
            });
    }

    [FollowMachine("Valid Name?", "Yes,No")]
    public void IsValidName(string playerName)
    {
        FollowMachine.SetOutput(playerName == null || playerName.Trim() == "" ? "No" : "Yes");
    }
    [FollowMachine("Check Player Info ?", "No Player Info,No Player ID,Has Player ID")]
    public void CheckPlayerInfo()
    {
        PlayerInfo =
            LocalDBController
                .Table<PlayerInfo>()
                .FirstOrDefault();

        if (PlayerInfo == null)
            FollowMachine.SetOutput("No Player Info");
        else if (PlayerInfo.PlayerID == null)
            FollowMachine.SetOutput("No Player ID");
        else
            FollowMachine.SetOutput("Has Player ID");

    }
}
