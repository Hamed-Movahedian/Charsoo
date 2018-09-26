using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using JetBrains.Annotations;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class UserAccountWindow : MgsUIWindow
{
    public Button PlayerName;
    public Button PlayerID;
    public Button UserPuzzleCount;
    public Button Telephone;
    public Button Email;

    [CanBeNull] private PlayerInfo _playerInfo;

    public void OnEnable()
    {


    }


    public override void Refresh()
    {
        _playerInfo = LocalDBController.Table<PlayerInfo>().FirstOrDefault();

        if (_playerInfo == null)
        {
            Close("NoPlayerInfo");
            return;
        }

        PlayerName.transform.GetChild(0).GetComponent<Text>().text = ArabicFixer.Fix(_playerInfo.Name);

        PlayerID.interactable = _playerInfo.PlayerID == null;
        PlayerID.onClick.AddListener(() => Close("Register Player Info"));

        PlayerID.transform.GetChild(0).GetComponent<Text>().text =
            ArabicFixer.Fix(
                _playerInfo.PlayerID != null ?
                    _playerInfo.PlayerID.ToString()
                    : "بدون شناسه", true, true);

        UserPuzzleCount.onClick.AddListener(() => Close("Goto User Puzzles"));
        UserPuzzleCount.transform.GetChild(0).GetComponent<Text>().text =
            ArabicFixer.Fix(LocalDBController.Table<UserPuzzle>().Count().ToString(), true, true);

        Telephone.interactable = _playerInfo.Telephone.Length < 5;
        Telephone.onClick.AddListener(() => Close("Register Phone Number"));
        Telephone.transform.GetChild(0).GetComponent<Text>().text =
            ArabicFixer.Fix(_playerInfo.Telephone.Length > 5 ? _playerInfo.Telephone : "اتصال به شماره موبایل");

        Email.interactable = _playerInfo.Email.Length < 5;
        Email.onClick.AddListener(() => Close("Register Email"));
        Email.transform.GetChild(0).GetComponent<Text>().text =
            _playerInfo.Email.Length > 5 ? _playerInfo.Email : ArabicFixer.Fix("اتصال به ایمیل");

    }
}
