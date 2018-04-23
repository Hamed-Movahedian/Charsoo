//C# Example
using UnityEditor;
using UnityEngine;

public class Preferences : EditorWindow
{
    private string _key , _value;
    private string _currentValue="  ";
    public enum KeyType
    {
        Int, Float, String
    }

    private KeyType _prefType = KeyType.Int;


    // Add menu item named "My Window" to the Window menu
    [MenuItem("Window/Player Preferences")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(Preferences));
    }

    void OnGUI()
    {
        GUILayout.Label("Player Preferences", EditorStyles.boldLabel);

        _key = EditorGUILayout.TextField("Key", _key);
        _prefType = (KeyType)EditorGUILayout.EnumPopup("Type :", _prefType);
        string keyPrefix = _prefType == KeyType.Int ? "i_" : _prefType == KeyType.Float ? "f_" : "s_";
        _value = EditorGUILayout.TextField("Value", _value);
        if (ZPlayerPrefs.HasKey(keyPrefix+_key))
            switch (_prefType)
            {
                case KeyType.Int:
                    _currentValue = ZPlayerPrefs.GetInt(_key).ToString();
                    break;

                case KeyType.Float:
                    _currentValue = ZPlayerPrefs.GetFloat(_key).ToString("##.###");
                    break;

                case KeyType.String:
                    _currentValue = ZPlayerPrefs.GetString(_key);
                    break;
            }
        else
            _currentValue = "Don't Find This Key";

        EditorGUILayout.LabelField("Current Value : ", _currentValue);

        EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Set Key"))
            {
                switch (_prefType)
                {
                    case KeyType.Int:
                        ZPlayerPrefs.SetInt(_key, int.Parse(_value));
                        break;

                    case KeyType.Float:
                        ZPlayerPrefs.SetFloat(_key, float.Parse(_value));
                        break;

                    case KeyType.String:
                        ZPlayerPrefs.SetString(_key, _value);
                        break;
                }
            }
            if (GUILayout.Button("Delete Key"))
            {
                ZPlayerPrefs.DeleteKey(keyPrefix + _key);
            Debug.Log(keyPrefix);
            }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Delete All Data", "");

            if (GUILayout.Button("Delete All"))
            {
                if(EditorUtility.DisplayDialog("Delete All Player Preferences","Are you sure?\nThis is NOT Reversible.", "Delet All","Cancel"))
                    ZPlayerPrefs.DeleteAll();
            }
        EditorGUILayout.EndHorizontal();

    }
}
