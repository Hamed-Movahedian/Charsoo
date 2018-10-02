using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExportCurreptedPuzzles : ScriptableWizard
{

    public string FileAddress = "D://CurreptedWordSets.txt";
    [MenuItem("Word Game/Export Currepted To Text")]
    static void CreateWizard()
    {
        DisplayWizard<ExportCurreptedPuzzles>("Export Currepted Wordsets", "Export");
    }


    void OnWizardCreate()
    {
        string Allfile = "";
        foreach (PuzzleComponent puzzle in Selection.activeGameObject.GetComponentsInChildren<PuzzleComponent>())
        {
            Dictionary<Vector2, string> letterPos = new Dictionary<Vector2, string>();
            WordSet wordset = new WordSet();
            JsonUtility.FromJsonOverwrite(StringCompressor.DecompressString(puzzle.Content), wordset);
            foreach (SWord word in wordset.Words)
            {
                for (int i = 0; i < word.LocationList.Count; i++)
                {
                    Vector2 l = word.LocationList[i];
                    if (letterPos.ContainsKey(l))
                    {
                        if (letterPos[l] != word.Name[i].ToString())
                        {
                            Allfile += $"کلمه : {word.Name}    در جدول :  {puzzle.Clue}+{puzzle.PuzzleData.Row} در مجموعه : {puzzle.transform.parent.GetComponent<CategoryComponent>().CategoryData.Name}  \n";
                            break;
                        }
                    }
                    else
                        letterPos.Add(l, word.Name[i].ToString());

                }
            }
        }
        System.IO.File.WriteAllText(FileAddress, Allfile);
    }

}
