using UnityEngine;
using ArabicSupport;
using UnityEditor;
using UnityEngine.UI;

public class PersianTextWindow : EditorWindow
{
    public string Text;
    public string FixedText;

    
    [MenuItem("Window/Persian Text")]
    static void CreateWizard()
    {
        PersianTextWindow window = (PersianTextWindow)EditorWindow.GetWindow(typeof(PersianTextWindow));
        window.titleContent=new GUIContent("Persian Text");
        window.Show();
    }

    void OnGUI()
    {
        Text = EditorGUILayout.TextField("Text ", Text);
        if(!string.IsNullOrEmpty(Text))
        EditorGUILayout.LabelField("Fix text ",ArabicFixer.Fix(Text));
        if (GUILayout.Button("Copy"))
            EditorGUIUtility.systemCopyBuffer = ArabicFixer.Fix(Text);

    }

}