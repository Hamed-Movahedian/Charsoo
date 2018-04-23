using System.Collections.Generic;
using UnityEngine;

public class CommonLettersDictionary
{
    private Dictionary<string, Dictionary<string, List<Vector2>>> _dic;

    public CommonLettersDictionary(List<string> ws)
    {
        _dic=new Dictionary<string,Dictionary<string,List<Vector2>>>();

        for (int i = 0; i < ws.Count; i++)
            for (int j = 0; j < ws.Count; j++)
                if (i != j)
                {
                    if (!_dic.ContainsKey(ws[i]))
                        _dic.Add(ws[i], new Dictionary<string, List<Vector2>>());

                    _dic[ws[i]].Add(ws[j], GetCommonLetters(ws[i], ws[j]));
                }
    }

    private List<Vector2> GetCommonLetters(string s1, string s2)
    {
        List<Vector2> list = new List<Vector2>();

        for (int i = 0; i < s1.Length; i++)
            for (int j = 0; j < s2.Length; j++)
                if (s1[i] == s2[j])
                    list.Add(new Vector2(i, j));

        return list;
    }

    public Dictionary<string, List<Vector2>> this[string s]
    {
        get { return _dic[s]; }
    }
}