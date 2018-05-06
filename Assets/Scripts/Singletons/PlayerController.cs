using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib.Utilities;
using UnityEngine;

public class PlayerController : BaseObject
{
    #region Public

    public PlayerInfo PlayerInfo;

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

    #region Awake

    private void Awake()
    {
        NewPlayerID += SetPlayerIDInLocalDB;
    }

    private void SetPlayerIDInLocalDB(int newPlayerID)
    {
        throw new System.NotImplementedException();
    }

    #endregion


    #region LogIn

    public IEnumerator LogIn()
    {
        PlayerInfo = 
            LocalDatabase
            .Table<PlayerInfo>()
            .FirstOrDefault();

        // This is first time login
        if (PlayerInfo == null)
            yield return FirstTimeLogin();

        if (PlayerInfo.PlayerID == null)
        {
            // Register player to server and get PlayerID
            yield return Server.Post<PlayerInfo>(
                @"PlayerInfo/Create",
                PlayerInfo,
                r => { PlayerInfo = r; }); ;

            // If player successfully registered to server
            // run OnSetplayerID event
            if (PlayerInfo.PlayerID != null)
                OnNewPlayerID(PlayerInfo.PlayerID.Value);
        }

        StartCoroutine(LoginToDB());

    }

    private IEnumerator LoginToDB()
    {

        yield return LocalDatabase.AddLogin(PlayerInfo.PlayerID);

        if (PlayerInfo.PlayerID != null)
            yield return FlashOutLocalLogins();

        NameText nameText = FindObjectOfType<NameText>();

        if (nameText != null)
            nameText.SetName(PlayerInfo.Name);
    }

    private IEnumerator FlashOutLocalLogins()
    {
        List<LogIn> logIns = 
            LocalDatabase.Table<LogIn>().ToList();

        yield return Server.Post<int>(
            @"/api/Login/AddRange",
            logIns,
            r =>
            {
                if (logIns.Count == r)
                    LocalDatabase.DeleteAll<LogIn>();
            });
    }

    private IEnumerator GetPlayerID()
    {
        yield return Server.Post<PlayerInfo>(
            @"PlayerInfo/Create",
            PlayerInfo,
            r => { PlayerInfo = r; });
    }
    
    public IEnumerator FirstTimeLogin()
    {
        PlayerInfo = new PlayerInfo();

        var loginWindow = 
            DialogueWindow.GetWindow("LoginWindow");

        loginWindow["Name"] = "New Player";
        //loginWindow["Avatar"] = "";

        loginWindow.SetAction("HasAccount", () =>
        {
            
        });

        yield return loginWindow.ShowWindowAndWaitForClose();

        PlayerInfo.Name = loginWindow["Name"];
        //PlayerInfo.Avatar = loginWindow["Avatar"];

        yield break;

        if (PlayerInfo.PlayerID == null)
            // Create new one
            yield return Server.Post<PlayerInfo>(
                    @"PlayerInfo/Create",
                    PlayerInfo,
                    r => { PlayerInfo = r; });


        LocalDatabase.Insert(PlayerInfo);
    }

    private IEnumerator GetPlayerInfoByDeviceID()
    {
        yield return Server.Get<PlayerInfo>(
            @"Login/RestorePlayerInfo?deviceId=" 
                + SystemInfo.deviceUniqueIdentifier,
            o => { PlayerInfo = (PlayerInfo)o; });

    }


    #endregion


}
