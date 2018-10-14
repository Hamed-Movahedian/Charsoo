using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

internal static class BounderUtilitys
{
    #region privates

    public static Dictionary<Type,object> TypeDefaults=new Dictionary<Type, object>
    {
        {typeof(Int32),0 },
        {typeof(string),"" },
        {typeof(Single),0f },
        {typeof(Boolean),false },
    };

    #endregion

    #region BoldSeparator
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

    #endregion

    #region tex
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
    #endregion
    
    #region GetTypeDistance
    public static int GetTypeDistance(this Type type, Type baseType)
    {
        int i = 0;

        while (baseType != type)
        {
            type = type.BaseType;
            i++;
        }

        return i;
    }

    #endregion

    #region IsValidMember (static)
    public static bool IsValidMember(MemberInfo memberInfo)
    {
        if (memberInfo.MemberType != MemberTypes.Field &&
            memberInfo.MemberType != MemberTypes.Property &&
            memberInfo.MemberType != MemberTypes.Method)
            return false;


        if (memberInfo.MemberType == MemberTypes.Method)
        {
            var methodInfo = memberInfo as MethodInfo;

            if (
                methodInfo.IsGenericMethod |
                methodInfo.IsConstructor |
                (methodInfo.Name.Contains('_') && methodInfo.Name != "get_Item") |
                (methodInfo.ReturnType == typeof(void) || methodInfo.ReturnType == typeof(IEnumerator))
            )
                return false;

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                if (!TypeDefaults.ContainsKey(parameterInfo.ParameterType))
                    return false;
            }
        }

        return true;
    }
    #endregion

    public static List<Type> GetGameObjectComponentTypes(GameObject gameObject)
    {
        var types = gameObject.GetComponents<Component>()
            .Select(c => c.GetType())
            .ToList();

        types.Insert(0, typeof(GameObject));
        return types;
    }
}