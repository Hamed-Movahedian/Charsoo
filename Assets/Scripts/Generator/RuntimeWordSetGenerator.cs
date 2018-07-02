using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeWordSetGenerator : MonoBehaviour
{
    [Header("Windows")]
    public GetWordsWindow GetWordsWindow;
    public GetWordCountWindow WordCountWindow;
    public PartitionererWindow PartitionererWindow;
    public RegenerateWindow RegenerateWindow;

    public IEnumerator GenerateWordSet()
    {
        // set test words
        GetWordsWindow.WordsInputField.text =
            "فسنجان سمبوسه سوپ کشک خورشقيمه قرمهسبزي قیمه بادمجان سبزی‌پلو شیربرنج کلهپاچه باقالی‌پلو شیشلیک رشته‌پلو";

        // Delete all letters and words
        Singleton.Instance.LetterController.DeleteAllLetters();
        Singleton.Instance.WordManager.DeleteAllWords();

        // Disable letter selection
        Singleton.Instance.RayCaster.TriggerRaycast(false);
        Singleton.Instance.RayCaster.EnablePan(false);

        // show get words window and wait to close
        yield return GetWordsWindow.ShowWaitForCloseHide();
    }
}
