
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

    private GameObject _targetGO;

    private Type _baseType = null;

    private List<RTMemberInfo> _boundMembers = new List<RTMemberInfo>();

    private List<MemberInfo> _memberInfos = new List<MemberInfo>();
    private string _search;

    private Vector2 _scrollPos;

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

        if (_baseType == null)
        {
            _memberInfos.Clear();
            return;
        }

        var type = _baseType;

        if (_boundMembers.Count > 0)
            type = _boundMembers[_boundMembers.Count - 1].MemberType;

        _memberInfos = type
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => BounderUtilitys.IsValidMember(m))
            .OrderBy(m => m.Name)
            .OrderBy(m => type.GetTypeDistance(m.DeclaringType))
            .ToList();
    }
    #endregion

    // ********************* GUI
    #region OnGUI
    void OnGUI()
    {
        GUILayout.Space(5);

        #region TargetGameObject

        // ********************   TargetGameObject
        var gameObject = (GameObject)EditorGUILayout.ObjectField("Game Object", _targetGO, typeof(GameObject), true);

        if (_targetGO != gameObject)
        {
            _targetGO = gameObject;
            _baseType = null;
            _boundMembers.Clear();
            UpdateList();
        }

        if (_targetGO == null)
            return;

        #endregion

        #region Component

        // target game object components + GameObject
        List<Type> types = BounderUtilitys.GetGameObjectComponentTypes(_targetGO);

        // Select _boundTypes first item
        int index = 0;


        index = types.IndexOf(_baseType);

        if (index == -1)
            index = types.Count - 1;

        index = EditorGUILayout.Popup(new GUIContent("Component"), index, types.Select(t => t.Name).ToArray());

        if (types[index] != _baseType)
        {
            _baseType = types[index];
            _boundMembers.Clear();
            UpdateList();
        }

        #endregion

        #region Serialize text and back

        // ********************    Serialize text and back

        GUILayout.BeginHorizontal();

        var text = _baseType.Name;

        _boundMembers.ForEach(bm => text += "." + bm.Name);

        GUILayout.Label(text, BindStringStyle);

        GUILayout.FlexibleSpace();

        if (_boundMembers.Count > 0)
            if (GUILayout.Button(" ◄ ", GUILayout.ExpandWidth(false), GUILayout.Height(20)))
            {
                _boundMembers.RemoveAt(_boundMembers.Count - 1);
                UpdateList();
            }

        GUILayout.EndHorizontal();

        BounderUtilitys.BoldSeparator();

        #endregion

        #region Member GUI


        if (_boundMembers.Count > 0)
            _boundMembers.Last().OnGUI();


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
            if (GUILayout.Button("", (GUIStyle)"ToolbarSeachCancelButton"))
            {
                _search = string.Empty;
                GUI.SetNextControlName("SearchToolbar");
            }
        }
        GUILayout.EndHorizontal();
        BounderUtilitys.BoldSeparator();

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
                BounderUtilitys.BoldSeparator();

            lastItemType = _memberInfos[i].DeclaringType;

            DrawMember(i);

            var lastRect = GUILayoutUtility.GetLastRect();

            if (GUI.Button(lastRect, GUIContent.none, GUIStyle.none))
            {
                _boundMembers.Add(new RTMemberInfo(_memberInfos[i]));

                UpdateList();
            }
        }

        GUILayout.EndScrollView();

        #endregion
    }



    #endregion

    #region DrawItem
    private void DrawMember(int index)
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

    public static void EditBound(GameObject boundObject, string boundText, Type requreType, Action<GameObject, string> OnBound)
    {
        BounderWindow window = EditorWindow.GetWindow<BounderWindow>();

        window.EditBounds(boundObject, boundText, requreType, OnBound);

        window.Show();
    }

    private void EditBounds(GameObject boundObject, string boundText, Type requreType, Action<GameObject, string> onBound)
    {
        _targetGO = boundObject;
        _boundMembers = new List<RTMemberInfo>();

        if (boundObject == null || boundText == "")
        {
            _targetGO = null;
            _baseType = null;
        }
        else
        {
            var bTexts = boundText.Split('.').ToList();

            List<Type> types = BounderUtilitys.GetGameObjectComponentTypes(_targetGO);

            _baseType = types.FirstOrDefault(t => t.Name == bTexts[0]);

            if (_baseType != null)
            {
                var type = _baseType;

                for (int i = 1; i < bTexts.Count; i++)
                {
                    var memberInfo = new RTMemberInfo(type,bTexts[i]);

                    _boundMembers.Add(memberInfo);

                    type = memberInfo.GetType();
                }

            }
        }
    }
}