using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MakeWord : ScriptableWizard
{
    public string Word = "آزمایشی";
    public GameObject LetterPrefab;

    private List<GameObject> _letterGo;

    [MenuItem("Word Game/Create Word Wizard")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<MakeWord>("Create Word", "Create");
    }

    void OnWizardCreate()
    {
        //LetterPrefab = GameObject.FindGameObjectWithTag("LetterPref");

        _letterGo =new List<GameObject>();

        for (int i = 0; i < Word.Length; i++)
        {
            char c = Word[i];
            GameObject co = (GameObject) PrefabUtility.InstantiatePrefab(LetterPrefab);
            co.transform.position = Vector3.left*i;
            co.name = "Letter " + c;
            co.SetActive(true);
            co.GetComponentInChildren<TextMesh>().text = c.ToString();
            _letterGo.Add(co);
            if (i>0)
            {
                co.GetComponent<Letter>().ConnectedLetters.Add(_letterGo[i-1].GetComponent<Letter>());
            }
        }
        for (int i = 0; i < _letterGo.Count-1; i++)
        {
            GameObject o = _letterGo[i];
            o.GetComponent<Letter>().ConnectedLetters.Add(_letterGo[i + 1].GetComponent<Letter>());
        }
    }

 
}
