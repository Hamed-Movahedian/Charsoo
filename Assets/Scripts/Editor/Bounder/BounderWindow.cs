
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
    private List<Type> _boundTypes = new List<Type>();

    private List<MemberInfo> _memberInfos = new List<MemberInfo>();

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

        if (_boundTypes.Count == 0)
        {
            _memberInfos.Clear();
            return;
        }

        var finalType = _boundTypes[_boundTypes.Count - 1];

        _memberInfos = finalType
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => IsValidMember(m))
            .OrderBy(m => m.Name)
            .OrderBy(m => GetTypeDistance(finalType, m.DeclaringType))
            .ToList();
    }
    #endregion

    #region Utils
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

    #region Frindly Names
    private string GetTypeFrindlyName(Type type)
    {
        return $"{type.Name,-35} ({type.FullName})";
    }
    #endregion

    #region Reset
    private void Reset()
    {
        _boundText.Clear();
        _memberInfos.Clear();
    }
    #endregion

    #region AddLevel
    private void AddLevel(int i)
    {

        string text;

        if (_memberInfos[i].MemberType == MemberTypes.Method)
            text = _memberInfos[i].Name + "()";
        else
            text = _memberInfos[i].Name;

        _boundText.Add(text);
        _boundTypes.Add(GetMemberType(_memberInfos[i]));

        UpdateList();
    }

    #endregion

    #region Back
    private void Back()
    {
        _boundText.RemoveAt(_boundText.Count - 1);
        _boundTypes.RemoveAt(_boundTypes.Count-1);
        UpdateList();
    }

    #endregion

    // ********************* GUI

    #region OnGUI
    void OnGUI()
    {
        GUILayout.Space(5);

        #region TargetGameObject

        // ********************   TargetGameObject
        var gameObject = (GameObject) EditorGUILayout.ObjectField("Game Object", _targetGO, typeof(GameObject), true);

        if (_targetGO != gameObject)
        {
            _targetGO = gameObject;
            _boundTypes.Clear();
            _boundText.Clear();
            UpdateList();
        }

        if (_targetGO == null)
            return;

        #endregion

        #region Component

        // target game object components + GameObject
        var types = _targetGO.GetComponents<Component>()
            .Select(c => c.GetType())
            .ToList();

        types.Insert(0, typeof(GameObject));

        // Select _boundTypes first item
        int index = 0;

        if (_boundTypes.Count == 0)
        {
            _boundTypes.Add(null);
        }
        
        index = types.IndexOf(_boundTypes[0]);

        if (index == -1)
            index = types.Count - 1;

        index = EditorGUILayout.Popup(new GUIContent("Component"), index, types.Select(t => t.Name).ToArray());

        if (types[index] != _boundTypes[0])
        {
            _boundTypes.Clear();
            _boundTypes.Add(types[index]);

            _boundText.Clear();
            _boundText.Add(types[index].Name);


            UpdateList();
        }

        #endregion

        #region Serialize text and back

        // ********************    Serialize text and back

        GUILayout.BeginHorizontal();

        GUILayout.Label(_boundText.Aggregate((a, b) => a + "." + b), BindStringStyle);

        if (_boundText.Count > 1)
            if (GUILayout.Button(" ◄ ", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)))
                Back();

        GUILayout.EndHorizontal();

        EditorUtils.BoldSeparator();

        #endregion

        #region SEARCH

        // *******************   SEARCH
        if (Event.current.keyCode == KeyCode.DownArrow)
        {
            GUI.SetNextControlName("SearchToolbar");
        }

        if (Event.current.keyCode == KeyCode.UpArrow)
        {
            GUI.SetNextControlName("SearchToolbar");
        }

        GUILayout.BeginHorizontal();
        {
            GUI.SetNextControlName("SearchToolbar");
            _search = EditorGUILayout.TextField(_search, SearchTextFieldStyle);
            if (GUILayout.Button("", (GUIStyle) "ToolbarSeachCancelButton"))
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
        for (int i = 0; i < _memberInfos.Count; i++)
        {
            if (_search != "")
                if (!_memberInfos[i].Name.StartsWith(_search, StringComparison.CurrentCultureIgnoreCase))
                    continue;

            if (lastItemType != _memberInfos[i].DeclaringType && lastItemType != null)
                EditorUtils.BoldSeparator();

            lastItemType = _memberInfos[i].DeclaringType;

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

        var memberType = "";

        switch (_memberInfos[index].MemberType)
        {
            case MemberTypes.Field:
                var fieldInfo = (FieldInfo)_memberInfos[index];

                memberType = $"{fieldInfo.FieldType.Name}";
                break;

            case MemberTypes.Property:
                var propertyInfo = _memberInfos[index] as PropertyInfo;

                memberType = $"{propertyInfo.PropertyType.Name}";
                break;

            case MemberTypes.Method:
                var methodInfo = _memberInfos[index] as MethodInfo;

                memberType = $"{methodInfo.ReturnType.Name}(";

                var parameters = methodInfo.GetParameters();

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameterInfo = parameters[i];
                    memberType +=
                        $"{parameterInfo.ParameterType.Name} {parameterInfo.Name}{(i == parameters.Length - 1 ? "" : ",  ")} ";
                }

                memberType += ")";
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        GUILayout.Label(_memberInfos[index].Name, ItemLableStyle, GUILayout.Width(200));
        GUILayout.Label(_memberInfos[index].DeclaringType.Name, ItemLableStyle, GUILayout.Width(200));
        GUILayout.Label(memberType, ItemLableStyle);
        GUILayout.EndHorizontal();
    }

    #endregion
}