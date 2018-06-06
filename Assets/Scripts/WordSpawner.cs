using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class WordSpawner : BaseObject
{
    //************* public
    public Letter LetterPrefab;
    public WordSet WordSet;
    public Func<Letter, Letter> EditorInstatiate;

    //************* private
    private Dictionary<Vector2, Letter> _locationDictionary;
    private Bounds _bounds;

    void Start()
    {

        //SpawnWords();
    }



    [ContextMenu("Spawn")]
    public void SpawnWords()
    {
        if (WordSet == null)
        {
            Debug.LogError("WordSet is null !!!");
            return;
        }

        // Delete all thing
        LetterController.DeleteAllLetters();

        if (_locationDictionary == null)
            _locationDictionary = new Dictionary<Vector2, Letter>();
        _locationDictionary.Clear();

        WordManager.DeleteAllWords();

        // Find center
        _bounds = WordSet.GetBound();
/*
        new Bounds(WordSet.Words[0].Min, Vector3.zero);
        WordSet.Words.ToList().ForEach(w =>
        {
            _bounds.Encapsulate(w.Min);
            _bounds.Encapsulate(w.Max);
        });
*/

        Vector3 boundsCenter = _bounds.center;
        boundsCenter.x = Mathf.Round(boundsCenter.x);
        boundsCenter.y = Mathf.Round(boundsCenter.y);
        boundsCenter.z = 0;
        _bounds.center = boundsCenter;

        // Spawn new words
        foreach (SWord sWord in WordSet.Words)
            SpawnWord(sWord);

        // PostProcess
        WordManager.GetWordsFormChilds();
        LetterController.ConnectAdjacentLetters();
    }

    private void SpawnWord(SWord sWord)
    {
        GameObject wordGameObject = new GameObject(sWord.Name);

        // Word component
        Word wordComponent = wordGameObject.AddComponent<Word>();
        wordComponent.Letters = new List<Letter>();
        wordComponent.Direction = sWord.Direction;
        wordGameObject.transform.parent = WordManager.transform;

        for (int i = 0; i < sWord.Name.Length; i++)
        {
            Letter letter=null;

            if (_locationDictionary.ContainsKey(sWord.Locations(i)))
                letter = _locationDictionary[sWord.Locations(i)];
            else
            {
                if (!Application.isPlaying)
                {
                    if (EditorInstatiate != null)
                        letter = EditorInstatiate(LetterPrefab);
                    else
                    {
                        Debug.LogError("EditorInstatiate not set !!!");
                    }
                }
                else
                    letter = Instantiate(LetterPrefab);

                // Position
                letter.transform.position = (Vector3) sWord.Locations(i) - _bounds.center;

                // Name
                letter.name = "Letter " + sWord.Name[i];

                // TextMesh
                letter.SetCharacter(sWord.Name[i]);

                // LetterController
                LetterController.AllLetters.Add(letter);
                letter.transform.parent = LetterController.transform;

                // Add to Dictionary
                _locationDictionary.Add(sWord.Locations(i), letter);
            }

            wordComponent.Letters.Add(letter);
            //letter.gameObject.SetActive(false);
        }
        if (Application.isPlaying)
        {
            StartCoroutine(EnlableParts());
        }
        else
        {
            foreach (List<Letter> part in CreatParts())
                foreach (Letter l in part)
                    l.gameObject.SetActive(true);
        }
    }

    #region enable letters PART BY PART

    private List<List<Letter>> CreatParts()
    {
        List<List<Letter>> parts=new List<List<Letter>>();
        parts.Clear();
        List<Letter> letters = new List<Letter>();

        WordManager.Words.ForEach(w =>
        {
            foreach (Letter l in w.Letters)
                if (!letters.Contains(l))
                    letters.Add(l);
        });

        while (letters.Count > 0)
        {
            Letter letter = letters[0];
            bool added = false;

            foreach (Letter connectedLetter in letter.ConnectedLetters)
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    List<Letter> part = parts[i];
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
                parts.Add(newPart);
            }
        }
        return parts;
    }

    public IEnumerator EnlableParts()
    {
        yield return new WaitForSeconds(0.1f);
        List<List<Letter>> parts = CreatParts();
        foreach (List<Letter> part in parts)
        {
            part.ForEach(l=>l.gameObject.SetActive(true));
            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

}
