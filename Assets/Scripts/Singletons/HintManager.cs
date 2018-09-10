using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using FollowMachineDll.Attributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HintManager : BaseObject
{
    [Header("Hints Prcie")]
    public int FullWord = 35;
    public int FullWordset = 60;

    public UnityEvent OnLowMoney;
    public UnityEvent CancelHint;
    public UnityEvent OnHintShow;

    public RectTransform WordHintPanel;
    public RectTransform LetterPrefab;
    public RectTransform SpacePrefab;
    public UnityEvent OnWordCompelete;
    public Material LetterMaterial;
    public GameObject HintEffect;

    public List<List<Letter>> HintParts = new List<List<Letter>>();
    public List<List<RectTransform>> HintRects = new List<List<RectTransform>>();
    private Material _defaultMat;
    private int _partId = 0;
    private Word _selectedWord;
    private bool _wordHintActive = false;

    // Use this for initialization
    void Start()
    {
        HintParts.Clear();
        WordHintPanel.gameObject.SetActive(false);
    }

    private Word SelectWord()
    {

        if (_selectedWord)
            if (!_selectedWord.IsComplete)
                return _selectedWord;

        List<Word> unSolvedWords = new List<Word>();

        unSolvedWords.Clear();
        foreach (Word word in WordManager.Words)
            if (!word.IsComplete)
                unSolvedWords.Add(word);

        _selectedWord = unSolvedWords[Random.Range(0, unSolvedWords.Count)];

        if (!_defaultMat)
            _defaultMat = _selectedWord.GetComponent<Material>();

        return _selectedWord;
    }

    private void CreatHintParts()
    {
        HintParts.Clear();
        List<Letter> letters = new List<Letter>();

        letters.AddRange(SelectWord().Letters);

        while (letters.Count > 0)
        {
            Letter letter = letters[0];
            bool added = false;

            foreach (Letter connectedLetter in letter.ConnectedLetters)
            {
                for (int i = 0; i < HintParts.Count; i++)
                {
                    List<Letter> part = HintParts[i];
                    if (part.Contains(connectedLetter))
                    {
                        part.Add(letters[0]);
                        letters.Remove(letter);
                        added = true;
                    }
                }
            }

            if (!added)
            {
                List<Letter> newPart = new List<Letter> { letter };
                letters.Remove(letter);
                HintParts.Add(newPart);
            }
        }
        CreatePartRects();
    }

    private void CreatePartRects()
    {
        HintParts.Reverse();
        for (int i = 0; i < HintParts.Count; i++)
        {
            HintParts[i].Reverse();
            List<RectTransform> partRects = new List<RectTransform>();
            List<Letter> part = HintParts[i];
            WordHintPanel.gameObject.SetActive(true);
            for (int j = 0; j < part.Count; j++)
            {
                RectTransform lGo = Instantiate(LetterPrefab, WordHintPanel);
                lGo.GetComponentInChildren<Text>().text = "";
                partRects.Add(lGo);
            }
            if (i != HintParts.Count)
                Instantiate(SpacePrefab, WordHintPanel);
            HintRects.Add(partRects);
        }
    }

    public void ShowPart(int index)
    {
        index = HintParts.Count - 1 - index;
        for (int i = 0; i < HintParts[index].Count; i++)
        {
            //HintParts[index][i].Frame.SetActive(true);
            HintRects[index][i].GetComponentInChildren<Text>().text =
                HintParts[index][i].GetComponentInChildren<TextMesh>().text;
        }


        StartCoroutine(ShowEffect(HintParts[index], HintRects[index], 0.1f));
    }


    private IEnumerator ShowEffect(List<Letter> part, List<RectTransform> partRects, float f)
    {
        GameObject effect = Instantiate(HintEffect);
        yield return new WaitForSeconds(f);
        Vector3 partPos = new Vector3();
        Vector3 rectPos = new Vector3();
        foreach (Letter letter in part)
            partPos += letter.transform.position;
        partPos /= part.Count;
        partPos.z = -2;
        effect.transform.position = partPos;

        foreach (RectTransform rect in partRects)
            rectPos += Camera.main.ScreenToWorldPoint(rect.position);
        rectPos /= partRects.Count;
        rectPos.z = -2;
        effect.transform.GetChild(0).position = rectPos;
        effect.SetActive(true);
        effect.transform.GetChild(0).GetComponent<MovingEffect>().OnArrive.AddListener
            (() =>
            {
                foreach (Letter t in part)
                    t.Frame.SetActive(true);
            }
            );
    }




    [FollowMachine("Hint One Part")]
    public void ShowNextPart()
    {
        if (_wordHintActive)
        {
            CancelHint.Invoke();
            return;
        }

        if (_selectedWord)
        {
            if (_selectedWord == SelectWord())
                if (_partId >= HintParts.Count)
                {
                    CancelHint.Invoke();
                    return;
                }
        }
        else
        {
            ClearHintPanel();
            CreatHintParts();
        }
        ShowPart(_partId);
        _partId++;
        OnHintShow.Invoke();
    }

    [FollowMachine("Hint Full")]
    public void ShowHwoleWord()
    {
        if (_wordHintActive)
        {
            CancelHint.Invoke();
            return;
        }

        if (PurchaseManager.PayCoin(FullWord))
        {
            if (HintParts.Count <= 0)
                CreatHintParts();

            for (int i = _partId; i < HintParts.Count; i++)
                ShowNextPart();

            _wordHintActive = true;
            OnHintShow.Invoke();
        }
        else
        {
            //CancelHint.Invoke();
            OnLowMoney.Invoke();
        }
    }


    public void CheckHintWord()
    {
        if (!_selectedWord)
            return;

        if (_selectedWord.IsComplete)
        {
            _wordHintActive = false;
            HideHintLetters();
        }
    }

    public void HideHintLetters()
    {
        if (!_selectedWord)
            return;

        foreach (Letter l in _selectedWord.Letters)
            l.Frame.SetActive(false);

        _selectedWord = null;
        ClearHintPanel();
        OnWordCompelete.Invoke();
    }

    private void ClearHintPanel()
    {
        HintParts.Clear();
        HintRects.Clear();
        _partId = 0;
        while (WordHintPanel.transform.childCount > 0)
        {
            GameObject o = WordHintPanel.GetChild(0).gameObject;
            o.transform.parent = null;
            Destroy(o);
        }

        WordHintPanel.gameObject.SetActive(false);
    }

}
