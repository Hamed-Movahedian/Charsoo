using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class WordManager : BaseObject
{
    public WordHighlightEffect WordCompleteEffect;
    public UnityEvent OnEnd;

    public List<Word> Words { get; set; }

    void Start()
    {
        GetWordsFormChilds();
    }

    public void GetWordsFormChilds()
    {
        if (Words == null)
            Words = new List<Word>();
        Words.Clear();
        Words.AddRange(GetComponentsInChildren<Word>());
    }

    [FollowMachine("Is Puzzle Solved?", "No,Yes")]
    public void CheckFinishGame()
    {
        foreach (Word word in Words)
            if (!word.IsComplete)
            {
                FollowMachine.SetOutput("No");
                return;
            }

        FollowMachine.SetOutput("Yes");

        OnEnd.Invoke();
    }

    public void DeleteAllWords()
    {
        // delete all childes
        while (transform.childCount>0)
        {
            var child = transform.GetChild(0);
            child.parent = null;
            if (Application.isPlaying)
                PoolManager.Instance.Return(child.GetComponent<Word>());
            else
                DestroyImmediate(child.gameObject);
        }

        Words?.Clear();

    }

}
