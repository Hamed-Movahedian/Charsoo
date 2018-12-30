using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetText : MonoBehaviour
{
    public void SetClueFromUserPuzzleInfo(UserPuzzleInfoWindow upi)
    {
        GetComponent<Text>().text = PersianFixer.Fix(upi.GetPuzzleClue());
    }
}
