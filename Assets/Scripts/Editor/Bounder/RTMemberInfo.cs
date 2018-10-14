using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

internal class RTMemberInfo
{
    private MemberInfo _memberInfo;
    private List<object> _parameterObject;
    private ParameterInfo[] _parameterInfos = new ParameterInfo[0];

    public RTMemberInfo(MemberInfo memberInfo)
    {
        _memberInfo = memberInfo;

        if (_memberInfo.MemberType == MemberTypes.Method)
        {
            _parameterInfos = (_memberInfo as MethodInfo).GetParameters();

            _parameterObject = new List<object>(_parameterInfos.Length);

            foreach (var parameterInfo in _parameterInfos)
            {
                _parameterObject.Add(BounderUtilitys.TypeDefaults[parameterInfo.ParameterType]);
            }
        }
    }

    public RTMemberInfo(Type type,string text)
    {
        if()
        type.
    }

    public Type MemberType
    {
        get
        {
            switch (_memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)_memberInfo).FieldType;
                case MemberTypes.Method:
                    return ((MethodInfo)_memberInfo).ReturnType;
                case MemberTypes.Property:
                    return ((PropertyInfo)_memberInfo).PropertyType;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public string Name
    {
        get
        {
            string text = _memberInfo.Name;

            if (_memberInfo.MemberType == MemberTypes.Method)
            {
                text += "(";
                for (var i = 0; i < _parameterObject.Count; i++)
                {
                    if (i > 0)
                        text += " ,";
                    if (_parameterInfos[i].ParameterType == typeof(string))
                        text += "\"" + _parameterObject[i] + "\"";
                    else
                        text += _parameterObject[i].ToString();
                }

                text += ")";
            }

            return text;
        }
    }

    public void OnGUI()
    {
        if (_parameterInfos.Length == 0)
            return;
        GUILayout.Label("Parameters :",EditorStyles.boldLabel);

        for (var i = 0; i < _parameterInfos.Length; i++)
        {
            var parameterInfo = _parameterInfos[i];

            if (parameterInfo.ParameterType == typeof(Int32))
            {
                if (_parameterObject[i] == null)
                    _parameterObject[i] = default(Int32);

                _parameterObject[i] =
                    EditorGUILayout.IntField(
                        parameterInfo.Name,
                        (int)_parameterObject[i]);

            }

            else if (parameterInfo.ParameterType == typeof(string))
            {
                if (_parameterObject[i] == null)
                    _parameterObject[i] = default(string);

                _parameterObject[i] =
                    EditorGUILayout.TextField(
                        parameterInfo.Name,
                        (string)_parameterObject[i]);
            }

            else if (parameterInfo.ParameterType == typeof(Boolean))
            {
                _parameterObject[i] =
                    EditorGUILayout.Toggle(
                        parameterInfo.Name,
                        (Boolean)_parameterObject[i]);
            }

            else if (parameterInfo.ParameterType == typeof(Single))
            {
                _parameterObject[i] =
                    EditorGUILayout.FloatField(
                        parameterInfo.Name,
                        (Single)_parameterObject[i]);
            }
        }

        BounderUtilitys.BoldSeparator();

    }
}