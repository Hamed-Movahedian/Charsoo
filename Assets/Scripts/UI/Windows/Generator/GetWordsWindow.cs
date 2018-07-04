using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MgsCommonLib.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class GetWordsWindow : UIWindowBase
{
    public InputField WordsInputField;
    public GetWordCountWindow WordCountWindow;
    public WordSetGenerator WordSetGenerator;

    public IEnumerator GenerateWordSets()
    {
        // Set words in generator
        WordSetGenerator.AllWords=WordsInputField.text
            .Replace(' ', '\n');
        WordSetGenerator.Initialize();

        // Hide this window
        StartCoroutine(Hide());


        // Show get word count window
        yield return WordCountWindow.ShowWaitForCloseHide();
 
    }
}
