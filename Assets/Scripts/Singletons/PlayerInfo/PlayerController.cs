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
            LocalDatabase
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
            yield return Server.Post<PlayerInfo>(
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

    #endregion

 
    public void SaveToLocalDB()
    {
        LocalDatabase.InsertOrReplace(PlayerInfo);
    }
}
