using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Word : BaseObject
{
    public List<Letter> Letters;

    public WordDirection Direction;
    public bool IsComplete = false;
    public string Name;


    public void Complete()
    {
        IsComplete = true;
        for (int i = 0; i < Letters.Count - 1; i++)
            Letters[i].ConnectTo(Letters[i + 1]);

    }
}

public enum WordDirection
{
    Horizontal=0,Vertical=1
}
