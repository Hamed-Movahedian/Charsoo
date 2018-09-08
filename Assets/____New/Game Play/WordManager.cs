using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WordManager : BaseObject
{
    public WordHighlightEffect WordCompleteEffect;
    public UnityEvent OnEnd;
    public List<Word> ErrorWords = new List<Word>();

    public List<Word> Words;
    private List<Word> _lastCompleteWords;

    void Start()
    {
        if (_lastCompleteWords == null)
            _lastCompleteWords = new List<Word>();

        GetWordsFormChilds();
    }

    public void GetWordsFormChilds()
    {
        if (Words == null)
            Words = new List<Word>();
        Words.Clear();
        Words.AddRange(GetComponentsInChildren<Word>());
    }

    public IEnumerator Check()
    {
        _lastCompleteWords.Clear();

        foreach (Word word in Words)
            if (!word.IsComplete)
                if (word.Check())
                    _lastCompleteWords.Add(word);

        if (_lastCompleteWords.Count > 0)
        {
            HintManager.CheckHintWord();
            yield return ShowWordEffect();

            if (IsFinished())
                OnEnd.Invoke();
        }
    }

    private bool IsFinished()
    {
        foreach (Word word in Words)
            if (!word.IsComplete)
                return false;

        return true;
    }

 
    private IEnumerator ShowWordEffect()
    {
        foreach (Word word in _lastCompleteWords)
            word.ShowCompeleteEffect();

        yield return new WaitForSeconds(0.4f);
    }

    public void DeleteAllWords()
    {
        // delete all childes
        while (transform.childCount>0)
        {
            var child = transform.GetChild(0);
            child.parent = null;
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }

        if (Words != null)
            Words.Clear();

    }

}
