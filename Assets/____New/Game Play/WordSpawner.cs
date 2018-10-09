using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;
using UnityEngine.Networking;

public class WordSpawner : BaseObject
{
    //************* public
    public Word WordPrefab;
    public Letter LetterPrefab;
    public WordSet WordSet;
    public Func<Letter, Letter> EditorInstatiate;

    //************* private
    private Dictionary<Vector2, Letter> _locationDictionary;
    private Bounds _bounds;
    public string Clue;
    public string PuzzleRow;
    public bool PuzzleReward;
    public int PuzzleID;
    public bool SpawnPartByPart = true;


    [FollowMachine("Has Reward?", "Yes,No")]
    public void RewardPuzzle()
    {
        FollowMachine.SetOutput(PuzzleReward?"Yes":"No");
    }


    [FollowMachine("Spawn Words")]
    public void SpawnWords()
    {
        if (WordSet == null)
        {
            Debug.LogError("WordSet is null !!!");
            return;
        }

        ClearTable();

        // Find center
        _bounds = WordSet.GetBound();

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

    public void ClearTable()
    {
        // Delete all thing
        LetterController.DeleteAllLetters();

        if (_locationDictionary == null)
            _locationDictionary = new Dictionary<Vector2, Letter>();
        _locationDictionary.Clear();

        WordManager.DeleteAllWords();
    }

    private void SpawnWord(SWord sWord)
    {
        // Word component
        Word wordComponent = (Word) PoolManager.Instance.Get(WordPrefab, WordManager.transform);

        if (wordComponent.Letters == null)
            wordComponent.Letters = new List<Letter>();
        else
            wordComponent.Letters.Clear();

        wordComponent.name = ArabicSupport.ArabicFixer.Fix(sWord.Name);
        wordComponent.Direction = sWord.WordDirection;
        wordComponent.Name = sWord.Name;

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
                {
                    letter = (Letter) PoolManager.Instance.Get(
                        LetterPrefab,
                        LetterController.transform);
                }

                // Position
                letter.transform.position = (Vector3) sWord.Locations(i) - _bounds.center;

                // Name
                letter.name = "Letter " + sWord.Name[i];
                letter.Char = sWord.Name[i];
                // TextMesh
                letter.SetCharacter(sWord.Name[i]);

                // LetterController
                LetterController.AllLetters.Add(letter);

                // Add to Dictionary
                _locationDictionary.Add(sWord.Locations(i), letter);
            }

            wordComponent.Letters.Add(letter);
            letter.gameObject.SetActive(false);
        }

        if (Application.isPlaying)
        {
            StartCoroutine(CameraController.FocusAllLetters());
            if (SpawnPartByPart)
                StartCoroutine(EnableParts());
            else
                LetterController.AllLetters.ForEach(l => l.gameObject.SetActive(true));
        }
        else
            LetterController.AllLetters.ForEach(l => l.gameObject.SetActive(true));
    }

    #region enable letters PART BY PART

    private List<List<Letter>> CreateParts()
    {
        List<List<Letter>> parts=new List<List<Letter>>();
        parts.Clear();

        List<Letter> letters = new List<Letter>(LetterController.AllLetters);


        while (letters.Count>0)
        {
            List<Letter> connectedLetters = new List<Letter>();

            letters[0].GetConnectedLetters(connectedLetters);

            parts.Add(connectedLetters);

            connectedLetters.ForEach(l=>letters.Remove(l));

        }
 
        return parts;
    }

    public IEnumerator EnableParts()
    {
        yield return new WaitForSeconds(0.1f);

        List<List<Letter>> parts = CreateParts();

        foreach (List<Letter> part in parts)
        {
            part.ForEach(l=>l.gameObject.SetActive(true));
            yield return new WaitForSeconds(0.1f);
        }
    }

    #endregion

}
