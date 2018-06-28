using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GetWordsWindow : UIWindowBase
{
    public InputField WordsInputField;
    public WordSetGenerator WordSetGenerator;

    public IEnumerator GenerateWordSets()
    {
        // Set words in generator
        WordSetGenerator.AllWords=WordsInputField.text
            .Replace(' ', '\n');

        // Hide this window
        yield return Hide();

        // Show inProgress window
        yield return UIController.ShowInprogressWindow(LanguagePack.Inprogress_GenerateWordSet);

        // Generate words
        yield return WordSetGenerator.MakeWordSet();

        // Hide in-progress window
        yield return UIController.HideInprogressWindow();

        // Spawn words
    }
}
