using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Word : BaseObject
{
    public List<Letter> Letters;

    public WordDirection Direction;
    public bool IsComplete = false;
    private WordHighlightEffect _compeleteEffect;
    public string Name;


    void Start()
    {
        IsComplete = false;
        _compeleteEffect = Instantiate(WordManager.WordCompleteEffect);
        _compeleteEffect.gameObject.SetActive(false);
        _compeleteEffect.transform.parent = transform;
    }

    void Update()
    {

    }


    public bool Check()
    {
        if (InCorrectOrder())
        {
            ConnectLetters();
            IsComplete = true;
            return true;
        }
        return false;
    }

    private void ConnectLetters()
    {
        for (int i = 0; i < Letters.Count - 1; i++)
            Letters[i].ConnectTo(Letters[i + 1]);
    }

    private bool InCorrectOrder()
    {
        for (int i = 0; i < Letters.Count - 1; i++)
            if (!Letters[i].IsNextTo(Letters[i + 1], Direction))
                return false;

        return true;
    }

    public void ShowCompeleteEffect()
    {
        Bounds bound = new Bounds(Letters[0].transform.position, Vector3.zero);

        AddToBound(ref bound);

        _compeleteEffect.Show(bound);
    }

    public void AddToBound(ref Bounds bound)
    {
        foreach (Letter letter in Letters)
            letter.AddToBounds(ref bound);
        return ;
    }
}

public enum WordDirection
{
    Horizontal,Vertical
}
