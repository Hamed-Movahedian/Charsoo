using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExportWordsetToText : ScriptableWizard
{
    public string FileAddress = "D://WordSets.txt";
    [MenuItem("Word Game/Export To Text")]
    static void CreateWizard()
    {
        DisplayWizard<ExportWordsetToText>("Export Words", "Export");
    }


    void OnWizardCreate()
    {
        string Allfile = "";
        foreach (PuzzleComponent puzzle in FindObjectsOfType<PuzzleComponent>())
        {
            string p ="اشاره: "+ puzzle.Clue+",";
            WordSet ws=new WordSet();
            JsonUtility.FromJsonOverwrite(StringCompressor.DecompressString(puzzle.PuzzleData.Content), ws);
            foreach (SWord word in ws.Words)
            {
                p += word.Name + ",";
            }
            p += "\n";
            Allfile += p;
        }
        System.IO.File.WriteAllText(FileAddress, Allfile);
    }


}
