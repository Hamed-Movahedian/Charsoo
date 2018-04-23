using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TableComponent : MonoBehaviour
{
    [HideInInspector]
    public int ID;

    [HideInInspector]
    public DatabaseComponent Database;

    private bool _dirty = true;

    public bool Dirty
    {
        get { return _dirty; }
        set
        {
            gameObject.name = GetName() + (value ? " *" : "");

            _dirty = value;
        }
    }

    protected virtual string GetName()
    {
        return "";
    }


    private void OnValidate()
    {
        Reload();

        if (IsChanged())
            Dirty = true;
    }

    protected virtual void Reload()
    {
        
    }

    public virtual void Initialize(TableComponent[] tableComponents)
    {

    }

    void OnEnable()
    {
#if UNITY_EDITOR
        EditorApplication.hierarchyWindowChanged += HierarchyChanged;
#endif
        Debug.Log("OnEnable");
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        EditorApplication.hierarchyWindowChanged -= HierarchyChanged;
#endif
    }

    private void HierarchyChanged()
    {
        if (IsChanged())
            Dirty = true;
    }

    protected virtual bool IsChanged()
    {
        return false;
    }

    protected int? GetParentID()
    {
        var parent = transform.parent.GetComponent<CategoryComponent>();
        return parent != null ? (int?) parent.ID : null;
    }

    public virtual void UpdateData()
    {
    }

    public virtual string IsValid()
    {
        return "";
    }
}
