using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJsonWordset : MonoBehaviour
{
    public WordSet WordSet;
	// Use this for initialization
	void Start ()
    {
        for (var i = 0; i < WordSet.Words.Count; i++)
        {
            SWord word = WordSet.Words[i];
            for (var j = 0; j < word.LocationList.Count; j++)
            {
                Vector2 v = word.LocationList[j];
                v.x = Mathf.RoundToInt(v.x);
                v.y = Mathf.RoundToInt(v.y);
                word.LocationList[j] = v;
            }
        }


        string compressString = StringCompressor.CompressString(JsonUtility.ToJson(WordSet));

        Debug.Log(compressString.Length);


        string decompressString = StringCompressor.DecompressString(compressString);
        Debug.Log(decompressString.Length);
	}
	
	// UpdateData is called once per frame
	void Update () {
		
	}
}
