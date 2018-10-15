
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

    private BoundData _boundData = null;

    private string _search = "";

    private Vector2 _scrollPos;
    private Type _requreType;
    private Action<GameObject, string> _onBound;

    #endregion


    // ********************* GUI
    #region OnGUI
    void OnGUI()
    {
        GUILayout.Space(5);

        #region TargetGameObject

        // ********************   TargetGameObject
        _boundData.GameObject =
            (GameObject)EditorGUILayout.ObjectField("Game Object", _boundData.GameObject, typeof(GameObject), true);

        if (_boundData.GameObject == null)
            return;

        #endregion

        #region Component

        // target game object components + GameObject
        List<Type> types = _boundData.GetCandidateBaseTypes();

        // Select _boundTypes first item
        int index = types.IndexOf(_boundData.BaseType);

        if (index == -1)
            index = types.Count - 1;

        index = EditorGUILayout.Popup(new GUIContent("Component"), index, types.Select(t => t.Name).ToArray());

        _boundData.BaseType = types[index];

        #endregion

        #region Serialize text and back

        // ********************    Serialize text and back

        GUILayout.BeginHorizontal();

        GUILayout.Label(_boundData.BoundText, BindStringStyle);

        GUILayout.FlexibleSpace();

        if (_boundData.HasMember)
            if (GUILayout.Button(" ◄ ", GUILayout.ExpandWidth(false), GUILayout.Height(20)))
                _boundData.RemoveLastMemeber();

        if (GUILayout.Button(" Bound ", GUILayout.ExpandWidth(false), GUILayout.Height(20)))
        {
            _onBound?.Invoke(_boundData.GameObject, _boundData.BoundText);
            Close();
        }

        GUILayout.EndHorizontal();

        BounderUtilitys.BoldSeparator();

        #endregion

        _boundData.OnGUI();

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
        var _memberInfos = _boundData.NextLevelMembers;
        for (int i = 0; i < _memberInfos.Count; i++)
        {
            if (_search != "")
                if (!_memberInfos[i].Name.StartsWith(_search, StringComparison.CurrentCultureIgnoreCase))
                    continue;

            if (lastItemType != _memberInfos[i].DeclaringType && lastItemType != null)
                BounderUtilitys.BoldSeparator();

            lastItemType = _memberInfos[i].DeclaringType;

            DrawMember(_memberInfos[i]);

            var lastRect = GUILayoutUtility.GetLastRect();

            if (GUI.Button(lastRect, GUIContent.none, GUIStyle.none))
                _boundData.Add(_memberInfos[i]);
        }

        GUILayout.EndScrollView();

        #endregion
    }



    #endregion

    #region DrawItem
    private void DrawMember(MemberInfo memberInfo)
    {
        GUILayout.BeginHorizontal(ItemStyle);

        var memberType = "";

        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                var fieldInfo = (FieldInfo)memberInfo;

                memberType = $"{fieldInfo.FieldType.Name}";
                break;

            case MemberTypes.Property:
                var propertyInfo = memberInfo as PropertyInfo;

                memberType = $"{propertyInfo.PropertyType.Name}";
                break;

            case MemberTypes.Method:
                var methodInfo = memberInfo as MethodInfo;

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

        GUILayout.Label(memberInfo.Name, ItemLableStyle, GUILayout.Width(200));
        GUILayout.Label(memberInfo.DeclaringType.Name, ItemLableStyle, GUILayout.Width(200));
        GUILayout.Label(memberType, ItemLableStyle);
        GUILayout.EndHorizontal();
    }

    #endregion

    #region EditBound
    public static void EditBound(GameObject boundObject, string boundText, Type requreType, Action<GameObject, string> OnBound)
    {
        BounderWindow window = EditorWindow.GetWindow<BounderWindow>();

        window.EditBounds(boundObject, boundText, requreType, OnBound);

        window.Show();
    }

    private void EditBounds(GameObject boundObject, string boundText, Type requreType, Action<GameObject, string> onBound)
    {
        _boundData = new BoundData(boundObject, boundText);
        _requreType = requreType;
        _onBound = onBound;
    }

    #endregion

}