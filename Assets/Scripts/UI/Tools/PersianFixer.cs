using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;

public static class PersianFixer
{
    public static string Fix(string word)
    {
        return ArabicFixer.Fix(word.Replace("ی", "ي"));
    }

    public static string Fix(string word, bool rtl)
    {
        return ArabicFixer.Fix(word.Replace("ی", "ي"), rtl);
    }

    public static string Fix(string word, bool tashkil, bool hindi)
    {
        return ArabicFixer.Fix(word.Replace("ی", "ي"), tashkil, hindi);
    }


}

