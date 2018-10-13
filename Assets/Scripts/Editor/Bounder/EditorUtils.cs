using UnityEngine;

internal class EditorUtils
{
    public static void BoldSeparator()
    {
        var lastRect = GUILayoutUtility.GetLastRect();
        GUILayout.Space(14);
        GUI.color = new Color(0, 0, 0, 0.25f);
        GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 4), tex);
        GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 1), tex);
        GUI.DrawTexture(new Rect(0, lastRect.yMax + 9, Screen.width, 1), tex);
        GUI.color = Color.white;
    }
    private static Texture2D _tex;
    private static Texture2D tex
    {
        get
        {
            if (_tex == null)
            {
                _tex = new Texture2D(1, 1);
                _tex.hideFlags = HideFlags.HideAndDontSave;
            }
            return _tex;
        }
    }
}