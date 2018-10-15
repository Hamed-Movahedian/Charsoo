using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public static class SupportedTypes
{
    private static  Dictionary<string, TypeUtils> Types = new Dictionary<string, TypeUtils>
        {
            {
                "Int32",new TypeUtils
                {
                    Type=typeof(Int32),
                    Default = 0,
                    Convertor = s=>Convert.ToInt32(s),
                    GUI=(s, o) => EditorGUILayout.IntField(s,(int)o)
                }
            } ,
            {
                "Boolean",new TypeUtils
                {
                    Type=typeof(Boolean),
                    Default = false,
                    Convertor = s=>Convert.ToBoolean(s),
                    GUI=(s, o) => EditorGUILayout.Toggle(s,(bool)o)
                }
            } ,
            {
                "Single",new TypeUtils
                {
                    Type=typeof(Single),
                    Default = 0f,
                    Convertor = s=>Convert.ToSingle(s),
                    GUI=(s, o) => EditorGUILayout.FloatField(s,(float)o)
                }
            } ,
            {
                "string",new TypeUtils
                {
                    Type=typeof(string),
                    Default = "",
                    Convertor = s=>s,
                    GUI=(s, o) => EditorGUILayout.TextField(s,(string)o)
                }
            } ,
        };

    #region Functions
    public class TypeUtils
    {
        public Type Type;
        public object Default;
        public Func<string, object> Convertor;
        public Func<string, object, object> GUI;
    }

    public static bool IsSupported(Type type)
    {
        return Types.ContainsKey(type.Name);
    }

    public static TypeUtils Get(string typeName) => Types[typeName];
    public static TypeUtils Get(Type type) => Types[type.Name];

    internal static bool IsSupported(string typeName)
    {
        return Types.ContainsKey(typeName);

    } 
    #endregion
}
