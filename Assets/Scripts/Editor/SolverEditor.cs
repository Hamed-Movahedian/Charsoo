using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Solver))]
public class SolverEditor : Editor
{
    private Solver _solver;

    public override void OnInspectorGUI()
    {
        _solver = target as Solver;

        if (GUILayout.Button("Connect Adjacent Letters"))
        {
            _solver.LetterController.ConnectAdjacentLetters();
            
        }
       if (GUILayout.Button("Snap"))
        {
            _solver.LetterController.AllLetters.ForEach(l=>l.Snap());
            
        }

        if (GUILayout.Button("Generate Sequences"))
        {
            _solver.GenerateSequences(_solver.Word);
        }

        if (GUILayout.Button("Place Next Sequence"))
        {
            _solver.PlaceNextSequence(false);
        }


        DrawDefaultInspector();

    }


}
