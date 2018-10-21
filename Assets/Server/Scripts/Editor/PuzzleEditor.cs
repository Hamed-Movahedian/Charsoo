using System.Collections;
using System.Collections.Generic;
using FollowMachineEditor.Server;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PuzzleComponent))]
public class PuzzleEditor : Editor
{
    private PuzzleComponent _puzzleComponent;

    public override void OnInspectorGUI()
    {
        _puzzleComponent = target as PuzzleComponent;

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        #region Update

        if (GUILayout.Button("Update"))
            UpdateServer(_puzzleComponent);

        #endregion

        #region Delete

        if (GUILayout.Button("Delete!!"))
        {
            if (EditorUtility.DisplayDialog("Delete Puzzle", "Are you sure?", "Delete", "Cancel"))
            {
                if (!ServerEditor.Post(@"Puzzles/Delete/" + _puzzleComponent.ID,null, "Delete Puzzle", "Delete"))
                    EditorUtility.DisplayDialog("Error", "Can't delete Puzzle in database", "Ok");

                _puzzleComponent.Delete();
                return;
            }

        }
        

        #endregion

        EditorGUILayout.EndHorizontal();

        #region Spawn

        if (GUILayout.Button("Spawn"))
        {
            var Wordspawner = FindObjectOfType<WordSpawner>();

            if (Wordspawner == null)
            {
                EditorUtility.DisplayDialog("Error", "Can't find word spawner", "Ok");
                return;
            }

            Wordspawner.EditorInstatiate = EditorInstantiate;

            WordSet wordSet = new WordSet();
            JsonUtility.FromJsonOverwrite(StringCompressor.DecompressString(_puzzleComponent.Content),wordSet);
            Wordspawner.WordSet = wordSet;

            Wordspawner.SpawnWords();

        }
        

        #endregion

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("ID", _puzzleComponent.ID.ToString());
        DrawDefaultInspector();
    }

    private Letter EditorInstantiate(Letter letterPrefab)
    {
        return (Letter)PrefabUtility.InstantiatePrefab(letterPrefab);
    }

    #region UpdateServer

    public static void UpdateServer(PuzzleComponent component)
    {
        component.UpdateData();

        if (!ServerEditor.Post(@"Puzzles/Update/" + component.PuzzleData.ID, component.PuzzleData, "Update puzzle", "Update"))
            component.PuzzleData = null;

    }
    

    #endregion
}

