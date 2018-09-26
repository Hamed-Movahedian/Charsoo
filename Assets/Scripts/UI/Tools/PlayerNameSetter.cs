using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameSetter : MonoBehaviour
{
    private Text _text;
    public void OnEnable()
    {
        PlayerInfo info = LocalDBController.Table<PlayerInfo>().FirstOrDefault();
        GetComponent<Text>().text = ArabicFixer.Fix(info.PlayerID+"   "+ info.Name);
    }

}
