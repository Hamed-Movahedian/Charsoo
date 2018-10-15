using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class BoundMember
{
    private MemberInfo _memberInfo;
    private object[] _parameterObjects;
    private ParameterInfo[] _parameterInfos = new ParameterInfo[0];

    public BoundMember(MemberInfo memberInfo)
    {
        _memberInfo = memberInfo;

        if (_memberInfo.MemberType == MemberTypes.Method)
        {
            _parameterInfos = (_memberInfo as MethodInfo).GetParameters();

            var objects=new List<object>();
            foreach (var parameterInfo in _parameterInfos)
            {
                objects.Add(SupportedTypes.Get(parameterInfo.ParameterType).Default);
            }
            _parameterObjects = objects.ToArray();
        }
    }

    public BoundMember(Type type, string name)
    {
        if (name.Contains('('))
        {
            string[] parts = name
                .Replace("\"", "")
                .Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

            MethodInfo methodInfo = (MethodInfo)type
                .GetMember(parts[0], BindingFlags.Public | BindingFlags.Instance)
                .Where(mi => mi.MemberType == MemberTypes.Method)
                .Select(mi => (MethodInfo)mi)
                .FirstOrDefault(mi => mi.GetParameters().Length == parts.Length - 1);

            if (methodInfo == null)
                throw new Exception($"Method info by name {name} not found!!");

            _memberInfo = methodInfo;

            _parameterInfos = methodInfo.GetParameters();

            var objects = new List<object>();

            for (int i = 0; i < _parameterInfos.Length; i++)
            {
                var parameterTypeName = _parameterInfos[i].ParameterType.Name;

                if (!SupportedTypes.IsSupported(parameterTypeName))
                    throw new Exception($"Invalid parameter {parameterTypeName} in method {methodInfo.Name}");

                objects.Add(SupportedTypes.Get(parameterTypeName)
                        .Convertor(parts[i + 1]));
            }

            _parameterObjects = objects.ToArray();
        }
        else
        {
            _memberInfo = type
                .GetMember(name, BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(mi =>
                    mi.MemberType == MemberTypes.Field |
                    mi.MemberType == MemberTypes.Property);

            _parameterInfos = new ParameterInfo[0];

            _parameterObjects = new object[0];

            if (_memberInfo == null)
                throw new Exception($"Member info by name {name} not found!!");
        }
    }

    public Type ReturnType
    {
        get
        {
            switch (_memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return AsFieldInfo.FieldType;
                case MemberTypes.Method:
                    return AsMethodInfo.ReturnType;
                case MemberTypes.Property:
                    return AsPropertyInfo.PropertyType;
            }

            return null;
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
                for (var i = 0; i < _parameterObjects.Length; i++)
                {
                    if (i > 0)
                        text += " ,";
                    if (_parameterInfos[i].ParameterType == typeof(string))
                        text += "\"" + _parameterObjects[i] + "\"";
                    else
                        text += _parameterObjects[i].ToString();
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

        GUILayout.Label("Parameters :", EditorStyles.boldLabel);

        for (var i = 0; i < _parameterInfos.Length; i++)
        {
            var parameterInfo = _parameterInfos[i];

            var typeUtils = SupportedTypes.Get(parameterInfo.ParameterType);

            if (_parameterObjects[i] == null)
                _parameterObjects[i] = typeUtils.Default;

            _parameterObjects[i] = typeUtils
                .GUI(parameterInfo.Name, _parameterObjects[i]);
        }

        BounderUtilitys.BoldSeparator();

    }

    public object GetValue(object inObject)
    {
        if(_memberInfo==null)
            throw new Exception($"Invalid bound member");

        switch (_memberInfo.MemberType)
        {
            case MemberTypes.Field:
                return AsFieldInfo.GetValue(inObject);
            case MemberTypes.Property:
                return AsPropertyInfo.GetValue(inObject);
            default:
                return AsMethodInfo.Invoke(inObject, _parameterObjects);
        }
    }

    public FieldInfo AsFieldInfo => _memberInfo as FieldInfo;
    public PropertyInfo AsPropertyInfo => _memberInfo as PropertyInfo;
    public MethodInfo AsMethodInfo => _memberInfo as MethodInfo;
}