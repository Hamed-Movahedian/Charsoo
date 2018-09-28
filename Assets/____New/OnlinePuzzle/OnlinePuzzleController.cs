using System.Collections;
using System.Collections.Generic;
using FMachine;
using FollowMachineDll.Attributes;
using UnityEngine;

public class OnlinePuzzleController : MonoBehaviour
{
    [FollowMachine("Prepare online puzzle for spawn")]
    public IEnumerator SetForSpawn(int ID)
    {
        UserPuzzle selectedPuzzle=null;

        // Ask command center to connect to account
         yield return ServerController
            .Get<UserPuzzle>($@"UserPuzzles/{ID}",
                  info => { selectedPuzzle = (UserPuzzle) info; } );


        var json = StringCompressor.DecompressString(selectedPuzzle.Content);

        WordSet wordSet = new WordSet();

        JsonUtility.FromJsonOverwrite(json, wordSet);

        Singleton.Instance.WordSpawner.WordSet = wordSet;
        Singleton.Instance.WordSpawner.Clue = selectedPuzzle.Clue;
        Singleton.Instance.WordSpawner.PuzzleID = -1;
        Singleton.Instance.WordSpawner.PuzzleRow = "";

        Singleton.Instance.WordSpawner.EditorInstatiate = null;
    }


    [FollowMachine("Prepare online puzzle for spawn")]
    public IEnumerator SetForSpawn(string ID)
    {
        yield return SetForSpawn(int.Parse(ID));
    }



}
