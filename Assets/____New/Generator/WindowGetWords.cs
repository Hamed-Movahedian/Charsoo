using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class WindowGetWords : MgsUIWindow
{
    public InputField WordsText;
    //public WindowGetWordCount WordCount;
    public override void OnClose()
    {
/*        WordCount.WordCountSlider.maxValue = GetWords().Count;
        if (WordCount.WordCountSlider.minValue == 0)
            WordCount.WordCountSlider.value = (WordCount.WordCountSlider.maxValue + 2) / 2;
        WordCount.WordCountSlider.minValue = 2;*/
    }

    public List<string> GetWords()
    {
        return WordsText.text
            .Replace("‌", "")
            .Split(' ', '\n', '\t', '-', '–')
            .Select(s => s.Trim())
            .Where(s => s.Length > 0 && s[0] != '/')
            .Distinct()
            .ToList();
    }

}
