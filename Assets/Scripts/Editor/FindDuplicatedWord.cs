using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FindDuplicatedWord : ScriptableWizard
{
    Dictionary<string, List<string>> _allWords = new Dictionary<string, List<string>>();


    public string FileAddress = "D://Duplicate Words.txt";
    public bool CheckSameCetegoryName=true;

    [MenuItem("Word Game/Duplicate Words To Text")]
    static void CreateWizard()
    {
        DisplayWizard<FindDuplicatedWord>("Export Duplicated Words", "Export");
    }


    void OnWizardCreate()
    {
        foreach (PuzzleComponent puzzle in Selection.activeTransform.GetComponentsInChildren<PuzzleComponent>())
        {
            string c = puzzle.Clue;
            WordSet ws = new WordSet();
            JsonUtility.FromJsonOverwrite(StringCompressor.DecompressString(puzzle.PuzzleData.Content), ws);
            foreach (SWord word in ws.Words)
            {
                List<string> list = new List<string>();

                if (_allWords.ContainsKey(word.Name))
                {
                    list.AddRange(_allWords[word.Name]);
                }

                list.Add(c);
                _allWords[word.Name] = list;
            }

            string export = "";
            foreach (string word in _allWords.Keys)
            {
                List<string> cats = _allWords[word];
                if (cats.Count > 1)
                {
                    if (CheckSameCetegoryName)
                    {
                        CheckSameCat:
                        for (var i = 0; i < cats.Count - 1; i++)
                        {
                            for (var j = i+1; j < cats.Count; j++)
                            {
                                if (cats[i].Replace(cats[j], "").Length != cats[i].Length||
                                    cats[j].Replace(cats[i], "").Length != cats[j].Length)
                                {
                                    cats.RemoveAt(i);
                                    goto CheckSameCat;
                                }
                            }
                        }
                        
                    }
                    if (cats.Count<2)
                    {
                        continue;
                    }

                    export += "\n:\t" + word +"\t";

                    foreach (string cat in cats)
                    {
                        export += "," + cat;
                    }
                }
            }

            System.IO.File.WriteAllText(FileAddress, export);
        }

    }
}
