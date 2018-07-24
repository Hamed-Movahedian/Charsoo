using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : BaseObject
{
    #region Public

    public PlayerInfo PlayerInfo;

    #endregion

    #region Properties

    public string Name
    {
        get { return PlayerInfo.Name; }
        set { PlayerInfo.Name = value; }
    }


    #endregion

    #region New PlayerID Event

    public delegate void NewPlayerIDEventHandler(int newPlayerID);
    public event NewPlayerIDEventHandler NewPlayerID;

    protected virtual void OnNewPlayerID(int newplayerid)
    {
        var handler = NewPlayerID;
        if (handler != null) handler(newplayerid);
    }

    #endregion
    
    #region LogIn

    public IEnumerator LogIn()
    {
        // Get player info from localDB
        PlayerInfo = 
            LocalDBController
            .Table<PlayerInfo>()
            .FirstOrDefault();

        // This is first time login
        if (PlayerInfo == null)
        {
            PlayerInfo = new PlayerInfo()
            {
                Name = ArabicFixer.Fix("بدون نام")
            };
            yield return UIController.Instance
                .PlayerInfoEditor
                .EditPlayerInfo();
        }

        if (PlayerInfo.PlayerID == null)
        {
            // Register player to server and get PlayerID
            yield return ServerController.Post<PlayerInfo>(
                @"PlayerInfo/Create",
                PlayerInfo,
                r => { PlayerInfo = r; }); ;

            // If player successfully registered to server
            if (PlayerInfo.PlayerID != null)
            {
                // run OnSetplayerID event
                OnNewPlayerID(PlayerInfo.PlayerID.Value);
            }
        }

        StartCoroutine(LoginToDB());

    }

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

    public int? GetPlayerID()
    {
        // Get player info from localDB
        PlayerInfo =
            LocalDBController
                .Table<PlayerInfo>()
                .FirstOrDefault();

        return PlayerInfo == null ? null : PlayerInfo.PlayerID;
    }
}
