using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using UnityEditor;
using UnityEngine;
using MgsCommonLib.Utilities;

[CustomEditor(typeof(WordSetGenerator))]
public class WordGeneratorEditor : Editor
{
    private WordSetGenerator _wg;
    private NewPartitioner _partitioner;
    private Shuffler _shuffler;
    #region Gizmo

    public void OnSceneGUI()
    {
        if (_wg == null)
            return;

        List<List<Letter>> paritions = new List<List<Letter>>();

        var allLetters =
            new List<Letter>(Singleton.Instance.LetterController.AllLetters);

        while (allLetters.Count > 0)
        {
            var letters = new List<Letter>();
            allLetters[0].GetConnectedLetters(letters);
            paritions.Add(letters);
            letters.ForEach(l => allLetters.Remove(l));
        }

        if (paritions.Count < 2)
            return;
        //Gizmos.color = Color.gray;
        Handles.color = _wg.PartitionGizmoColor;

        for (int i = 0; i < paritions.Count; i++)
        {
            List<Letter> parition = paritions[i];

            Bounds bounds = new Bounds(parition[0].transform.position, Vector3.zero);
            parition.ForEach(l => bounds.Encapsulate(l.transform.position));
            bounds.size += new Vector3(.7f, .7f, 0);
            Handles.DrawSolidRectangleWithOutline(
                new Rect(bounds.min, bounds.size ),
                Color.white, 
                Color.black );

        }
    }

    #endregion
    public override void OnInspectorGUI()
    {
        _wg = target as WordSetGenerator;


        if (GUILayout.Button("Generate"))
        {
            MgsCoroutine.GetTime = GetTime;
            MgsCoroutine.Start(
                _wg.MakeWordSet(),
                () => EditorUtility.DisplayCancelableProgressBar(MgsCoroutine.Title, MgsCoroutine.Info, MgsCoroutine.Percentage),
                0.1);

            EditorUtility.ClearProgressBar();

            _wg.EditorInstantiate = EditorInstantiate;

            _wg.SpawnWordSet();
        }

        if (GUILayout.Button("Partition"))
        {
            if (_partitioner == null)
                _partitioner = new NewPartitioner();
            
            MgsCoroutine.GetTime = GetTime;
            MgsCoroutine.Start(
                _partitioner.Portion(),
                () => EditorUtility.DisplayCancelableProgressBar(MgsCoroutine.Title, "Partitioning..", MgsCoroutine.Percentage),
                0.1);

            EditorUtility.ClearProgressBar();
        }

        if (GUILayout.Button("Shuffle"))
        {
            if (_shuffler == null)
                _shuffler = new Shuffler();

            Undo.RecordObjects(FindObjectsOfType<Letter>().Select(l=>(Object) (l.transform)).ToArray(), "Shuffle");

            _shuffler.ShuffleEditor();
        }

        DrawDefaultInspector();
    }


    private static double GetTime()
    {
        return EditorApplication.timeSinceStartup;
    }

    private Letter EditorInstantiate(Letter LetterPrefab)
    {
        return (Letter)PrefabUtility.InstantiatePrefab(LetterPrefab);
    }

}
