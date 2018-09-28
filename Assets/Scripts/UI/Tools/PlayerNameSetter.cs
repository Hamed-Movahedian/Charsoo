using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameSetter : MonoBehaviour
{
    public Text IDText;
    public void OnEnable()
    {
        PlayerInfo info = LocalDBController.Table<PlayerInfo>().FirstOrDefault();
        GetComponent<Text>().text = PersianFixer.Fix(info.Name);
        IDText.text="ID: # " + info.PlayerID;
    }

}
