using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyJsonUtility 
{
    public static string CompressString(string input)
    {
        return StringCompressor.CompressString(input);
    }

    public static string DeCompressString(string input)
    {
        return StringCompressor.DecompressString(input);
    }
    public static T[] getJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
