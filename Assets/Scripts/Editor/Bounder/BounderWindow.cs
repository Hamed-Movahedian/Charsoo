
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class BounderWindow : EditorWindow
{
    #region Styles

    private GUIStyle _itemStyle;

    public GUIStyle ItemStyle => _itemStyle ?? (_itemStyle = new GUIStyle("box")
    {
        fontSize = 14,
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleLeft
    });

    private GUIStyle _itemLableStyle;

    public GUIStyle ItemLableStyle => _itemLableStyle ?? (_itemLableStyle = new GUIStyle(EditorStyles.boldLabel)
    {
        fontSize = 14,
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleLeft
    });

    private GUIStyle _bindStringStyle;
    public GUIStyle BindStringStyle => _bindStringStyle ?? (_bindStringStyle = new GUIStyle("box")
    {
        fontSize = 20,
        fontStyle = FontStyle.Bold,
        alignment = TextAnchor.MiddleLeft,
        padding = new RectOffset(5, 5, 5, 5),
    });

    private GUIStyle _searchTextField;
    public GUIStyle SearchTextFieldStyle => _searchTextField ?? (_searchTextField = new GUIStyle("ToolbarSeachTextField")
    {
        fontSize = 14,
        fontStyle = FontStyle.Bold,
        fixedHeight = 20,
        stretchHeight = true
    });

    #endregion

    #region Privates

    private List<string> _boundText = new List<string>();
    private List<string> _menuItems = new List<string>();
    private List<MemberInfo> _memberInfos = new List<MemberInfo>();
    private List<Type> _types = new List<Type>();
    private GameObject _targetGO;
    private Dictionary<string, Type> _typeDic = new Dictionary<string, Type>();
    private Vector2 _scrollPos;
    private string _search;

    private readonly List<Type> _supportedTypes = new List<Type>
    {
        typeof(Int32),typeof(Boolean),typeof(string),typeof(Single)
    };
    #endregion
    
    #region Window Functions

    [MenuItem("Test/Bounder")]
    static void Init()
    {
        EditorWindow window = EditorWindow.GetWindow<BounderWindow>();
        window.Show();
    }

    private void OnEnable()
    {
        //Reset();
        UpdateList();
    }
    #endregion

    #region UpdateList
    private void UpdateList()
    {
        _search = "";
        if (_boundText.Count == 0)
        {
            if (_targetGO == null)
            {
                Reset();
                return;
            }

            _types = _targetGO
                .GetComponents<Component>()
                .Select(c => c.GetType())
                .ToList();

            _types.Insert(0, typeof(GameObject));

            _types.ForEach(t => _typeDic[t.Name] = t);

            _menuItems = _types
                .Select(t => GetTypeFrindlyName(t))
                .ToList();
        }
        else
        {
            var finalType = GetFinalType();

            _memberInfos = finalType
                .GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => IsValidMember(m))
                .Where(m => GetMemberType(m) != typeof(void))
                .OrderBy(m => m.Name)
                .OrderBy(m => GetTypeDistance(finalType, m.DeclaringType))
                .ToList();

            _menuItems = _memberInfos
                .Select(m => GetMemberFrindlyName(m))
                .ToList();
        }
    }
    #endregion

    #region Utils
    private bool IsDisplayingMembers()
    {
        return _boundText.Count != 0;
    }
    private int GetTypeDistance(Type finalType, Type type)
    {
        int i = 0;
        while (type != finalType)
        {
            finalType = finalType.BaseType;
            i++;
        }

        return i;
    }
    private Type GetMemberType(MemberInfo memberInfo)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)memberInfo).FieldType;
            case MemberTypes.Method:
                return ((MethodInfo)memberInfo).ReturnType;
            case MemberTypes.Property:
                return ((PropertyInfo)memberInfo).PropertyType;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private MemberInfo GetMember(Type type, string s)
    {
        if (s.Contains("("))
            s = s.Split('(')[0];

        var memberInfos = type.GetMember(s);

        if (memberInfos.Length == 0)
            throw new Exception($"Member name {s} not found !!!");

        if (memberInfos.Length > 1)
            throw new Exception($"Member name {s} more than one found !!!");

        return memberInfos[0];
    }

    #endregion

    #region IsValidMember
    private bool IsValidMember(MemberInfo m)
    {
        if (m.MemberType != MemberTypes.Field &&
            m.MemberType != MemberTypes.Property &&
            m.MemberType != MemberTypes.Method)
            return false;


        if (m.MemberType == MemberTypes.Method)
        {
            var methodInfo = m as MethodInfo;

            if (
                methodInfo.IsGenericMethod |
                methodInfo.IsConstructor |
                (methodInfo.Name.Contains('_') && methodInfo.Name != "get_Item") |
                (methodInfo.ReturnType == typeof(void) || methodInfo.ReturnType == typeof(IEnumerator))
                )
                return false;

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                if (!_supportedTypes.Contains(parameterInfo.ParameterType))
                    return false;
            }
        }

        return true;
    }

    #endregion

    #region GetFinalType
    private Type GetFinalType()
    {
        if (_boundText.Count == 0)
            return null;

        var type = _typeDic[_boundText[0]];

        for (int i = 1; i < _boundText.Count; i++)
        {
            var memberInfo = GetMember(type, _boundText[i]);
            type = GetMemberType(memberInfo);
        }

        return type;
    }

    #endregion
    
    #region Frindly Names
    private string GetMemberFrindlyName(MemberInfo memberInfo)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                var fieldInfo = (FieldInfo)memberInfo;
                return $"{fieldInfo.Name,-35} {fieldInfo.FieldType.Name}";

            case MemberTypes.Property:
                var propertyInfo = memberInfo as PropertyInfo;

                return $"{propertyInfo.Name,-35} {propertyInfo.PropertyType.Name}";

            case MemberTypes.Method:
                var methodInfo = memberInfo as MethodInfo;

                var frindlyName = $"{methodInfo.Name,-35} {methodInfo.ReturnType.Name}(";

                var parameters = methodInfo.GetParameters();

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameterInfo = parameters[i];
                    frindlyName += $"{parameterInfo.ParameterType.Name} {parameterInfo.Name}{(i == parameters.Length - 1 ? "" : ",  ")} ";
                }

                return frindlyName + ")";

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string GetTypeFrindlyName(Type type)
    {
        return $"{type.Name,-35} ({type.FullName})";
    }
    #endregion

    #region Reset
    private void Reset()
    {
        _boundText.Clear();
        _menuItems.Clear();
        _types.Clear();
        _memberInfos.Clear();
    }
    #endregion

    #region AddLevel
    private void AddLevel(int i)
    {
        if (_boundText.Count == 0)
        {
            _boundText.Add(_types[i].Name);
        }
        else
        {
            string text;

            if (_memberInfos[i].MemberType == MemberTypes.Method)
                text = _memberInfos[i].Name + "()";
            else
                text = _memberInfos[i].Name;

            _boundText.Add(text);
        }

        UpdateList();
    }

    #endregion

    #region Back
    private void Back()
    {
        _boundText.RemoveAt(_boundText.Count - 1);

        UpdateList();
    }

    #endregion

    #region GUI
    void OnGUI()
    {
        GUILayout.Space(5);

        #region TargetGameObject
        // ********************   TargetGameObject
        var gameObject = (GameObject)EditorGUILayout.ObjectField("Game Object", _targetGO, typeof(GameObject), true);

        if (_targetGO != gameObject)
        {
            _targetGO = gameObject;
            Reset();
            UpdateList();
        }

        if (_targetGO == null)
            return;

        #endregion

        #region Serialize text and back
        // ********************    Serialize text and back
        if (_boundText.Count > 0)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(_boundText.Aggregate((a, b) => a + "." + b), BindStringStyle);

            //GUILayout.FlexibleSpace();
            if (GUILayout.Button(" ◄ ", GUILayout.ExpandHeight(true)))
                Back();

            GUILayout.EndHorizontal();
        }

        EditorUtils.BoldSeparator();
        #endregion
        
        #region SEARCH
        // *******************   SEARCH
        if (Event.current.keyCode == KeyCode.DownArrow) { GUI.SetNextControlName("SearchToolbar"); }
        if (Event.current.keyCode == KeyCode.UpArrow) { GUI.SetNextControlName("SearchToolbar"); }
        GUILayout.BeginHorizontal();
        {
            GUI.SetNextControlName("SearchToolbar");
            _search = EditorGUILayout.TextField(_search, SearchTextFieldStyle);
            if (GUILayout.Button("", (GUIStyle)"ToolbarSeachCancelButton"))
            {
                _search = string.Empty;
                GUI.SetNextControlName("SearchToolbar");
            }
        }
        GUILayout.EndHorizontal();
        EditorUtils.BoldSeparator();
        #endregion
        
        #region Item List
        // ********************** Item List
        Type lastItemType = null;

        _scrollPos = GUILayout.BeginScrollView(_scrollPos);
        for (int i = 0; i < _menuItems.Count; i++)
        {
            if (_search != "")
                if (!_menuItems[i].StartsWith(_search, StringComparison.CurrentCultureIgnoreCase))
                    continue;

            if (IsDisplayingMembers())
            {
                if (lastItemType != _memberInfos[i].DeclaringType && lastItemType != null)
                    EditorUtils.BoldSeparator();

                lastItemType = _memberInfos[i].DeclaringType;
            }

            DrawItem(i);

            var lastRect = GUILayoutUtility.GetLastRect();

            if (GUI.Button(lastRect, GUIContent.none, GUIStyle.none))
            {
                AddLevel(i);
            }


        }
        GUILayout.EndScrollView(); 
        #endregion
    }
    #endregion

    #region DrawItem
    private void DrawItem(int index)
    {
        GUILayout.BeginHorizontal(ItemStyle);
        
        if (_boundText.Count == 0)
        {
            GUILayout.Label(_types[index].Name, ItemLableStyle, GUILayout.Width(200));
            GUILayout.Label($"({_types[index].FullName})", ItemLableStyle);
        }
        else
        {
            var lable1 = "";
            var lable2 = "";

            switch (_memberInfos[index].MemberType)
            {
                case MemberTypes.Field:
                    var fieldInfo = (FieldInfo)_memberInfos[index];

                    lable1 = fieldInfo.Name;
                    lable2 = $"{fieldInfo.FieldType.Name}";
                    break;

                case MemberTypes.Property:
                    var propertyInfo = _memberInfos[index] as PropertyInfo;

                    lable1 = propertyInfo.Name;
                    lable2 = $"{propertyInfo.PropertyType.Name}";
                    break;

                case MemberTypes.Method:
                    var methodInfo = _memberInfos[index] as MethodInfo;

                    lable1 = methodInfo.Name;
                    lable2 = $"{methodInfo.ReturnType.Name}(";

                    var parameters = methodInfo.GetParameters();

                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var parameterInfo = parameters[i];
                        lable2 += $"{parameterInfo.ParameterType.Name} {parameterInfo.Name}{(i == parameters.Length - 1 ? "" : ",  ")} ";
                    }

                    lable2 += ")";
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            GUILayout.Label(lable1, ItemLableStyle, GUILayout.Width(200));
            GUILayout.Label(_memberInfos[index].DeclaringType.Name, ItemLableStyle, GUILayout.Width(200));
            GUILayout.Label(lable2, ItemLableStyle);
        }
        GUILayout.EndHorizontal();
    }

    #endregion
}