using System.Collections;
using System.Collections.Generic;
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

    public void CheckFinishGame()
    {
        foreach (Word word in Words)
            if (!word.IsComplete)
                return;

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
