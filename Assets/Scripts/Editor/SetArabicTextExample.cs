using UnityEngine;
using ArabicSupport;
using UnityEditor;
using UnityEngine.UI;

public class SetArabicTextExample : ScriptableWizard
{


    [MenuItem("Word Game/ArabicFix #f")]
    static void ArabicFix()
    {
        EditorGUIUtility.systemCopyBuffer=ArabicFixer.Fix(EditorGUIUtility.systemCopyBuffer);
        

    }

    [MenuItem("Word Game/Fix Persian Text")]
    static void CreateWizard()
    {
        DisplayWizard<SetArabicTextExample>("Fix Persian Text", "Fix");
    }

    void OnWizardCreate()
    {
        foreach (GameObject o in Selection.gameObjects)
        {
            foreach (Text textO in o.GetComponentsInChildren<Text>())
            {
                string txt = textO.text;
                textO.text = ArabicFixer.Fix(txt, false, true);
            }
        }
        
        Debug.Log(EditorGUIUtility.keyboardControl);


        foreach (TextMesh textO in Selection.activeGameObject.GetComponentsInChildren<TextMesh>())
        {
            string txt = textO.text;
            textO.text = ArabicFixer.Fix(txt, false, true);
        }
    }

}