using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PuzzleSort : ScriptableWizard
{


    [MenuItem("Word Game/Sort Puzzle")]
    static void CreateWizard()
    {
        DisplayWizard<PuzzleSort>("Sort Puzzle", "Export");
    }

    void OnWizardCreate()
    {
        List<PuzzleComponent> list = Selection.activeGameObject.GetComponentsInChildren<PuzzleComponent>().ToList();
        list.Sort((p1,p2) =>GetWordCount(p1).CompareTo(GetWordCount(p2)));
        list.ForEach(p=>p.transform.SetAsLastSibling());
    }

    private static int GetWordCount(PuzzleComponent p)
    {
        WordSet ws = new WordSet();
        JsonUtility.FromJsonOverwrite(StringCompressor.DecompressString(p.Content), ws);
        return ws.Words.Count;
    }
}
