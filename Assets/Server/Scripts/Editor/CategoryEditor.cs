using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CategoryComponent))]
public class CategoryEditor : Editor
{
    private CategoryComponent _categoryComponent;

    public override void OnInspectorGUI()
    {
        _categoryComponent = target as CategoryComponent;


        EditorGUILayout.BeginHorizontal();

        #region Add Subcategory

        if (GUILayout.Button("Add Subcategory"))
        {
            // create category object
            var category = new Category()
            {
                ID=1,
                PrerequisiteID=null,
                ParentID = _categoryComponent.ID,
                Name = "No Name",
                Row = _categoryComponent.transform.childCount,
                Icon = "22",
                Visit = true,
                Price = 0,
                LastUpdate = DateTime.Now
            };

            // Add to server
            Category newCategory = ServerEditor.Post<Category>(@"Categories/Create", category, "Create Category", "Create");

            if (newCategory==null)
            {
                EditorUtility.DisplayDialog("Error", "Can't create Subcategory in server", "Ok");
                return;
            }

            _categoryComponent.AddSubcategory(newCategory);

        }
        

        #endregion

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        #region UpdateData

        if (GUILayout.Button("UpdateData"))
            UpdateServer(_categoryComponent);

        #endregion

        #region Delete

        if (GUILayout.Button("Delete!!"))
        {
            if (_categoryComponent.transform.childCount != 0)
            {
                EditorUtility.DisplayDialog("Error", "Can't delete Category with child", "Ok");
                return;
            }

            if(EditorUtility.DisplayDialog("Delete Category","Are you sure?","Delete","Cancel"))
            {
                if (!ServerEditor.Post(@"Categories/Delete/" + _categoryComponent.ID, null, "Delete Category", "Delete"))
                {
                    EditorUtility.DisplayDialog("Error", "Can't delete Category in server", "Ok");
                    return;
                }
                _categoryComponent.Delete();
                return;
            }

        }
        

        #endregion

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("ID", _categoryComponent.ID.ToString());

        DrawDefaultInspector();
    }

    #region UpdateServer

    public static void UpdateServer(CategoryComponent component)
    {
        component.UpdateData();

        if (!ServerEditor.Post(@"Categories/Update/" + component.CategoryData.ID, component.CategoryData, "Update category", "Update"))
            component.CategoryData = null;
    }
    
    #endregion
}
