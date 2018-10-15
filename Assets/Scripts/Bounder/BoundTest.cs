using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundTest : MonoBehaviour
{
    public GameObject BoundObject;
    public string BoundText;

    public string Value;
    
    [ContextMenu("Edit")]
    public void EditBound()
    {
        BounderWindow.EditBound(
            BoundObject, 
            BoundText,
            typeof(string),
            (go, text) => 
            {
                BoundObject = go;
                BoundText = text;
            }
            );
    }
    [ContextMenu("GetValue")]
    public void GetValue()
    {
        var boundData = new BoundData(BoundObject, BoundText);

        object o= boundData.GetValue(typeof(string));

        if(o==null)
            return;

        Value = o.ToString();
    }
}
