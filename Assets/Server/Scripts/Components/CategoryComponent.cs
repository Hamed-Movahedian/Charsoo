using System.Linq;
using ArabicSupport;
using UnityEngine;

public class CategoryComponent : TableComponent
{
    #region Public

    public Category CategoryData;

    public string Name;
    public string Icon;
    public CategoryComponent Prerequisite;
    public int Price;

    #endregion
    
    #region Create

    public static Transform Create(Category category, Transform parenTransform)
    {
        // create game object
        GameObject go = new GameObject("None");
        go.transform.parent = parenTransform;

        // create CategoryComponent
        var categoryComponent = go.AddComponent<CategoryComponent>();

        // setup CategoryComponent
        categoryComponent.CategoryData = category;
        categoryComponent.ID = category.ID;
        categoryComponent.Name = category.Name;
        categoryComponent.Icon = category.Icon;
        categoryComponent.Price = category.Price;
        categoryComponent.Dirty = false;

        // return CategoryComponent transform
        return categoryComponent.transform;
    }


    #endregion
    
    #region UpdateData

    public override void UpdateData()
    {
        if (!Dirty)
            return;

        CategoryData.Name = Name;
        CategoryData.Icon = Icon;
        CategoryData.Price = Price;

        CategoryData.ParentID = GetParentID();
        CategoryData.PrerequisiteID = Prerequisite != null ? (int?)Prerequisite.CategoryData.ID : null;
        CategoryData.Row = transform.GetSiblingIndex();
        
        Dirty = false;

    }


    #endregion
    
    #region GetName

    protected override string GetName()
    {
        return "C - " + PersianFixer.Fix(Name);
    }
    

    #endregion

    #region Reload

    protected override void Reload()
    {
        if (CategoryData == null)
            gameObject.name = "Reload !!!";
        
    }

    #endregion

    #region Initialize

    public override void Initialize(TableComponent[] tableComponents)
    {
        if (CategoryData.PrerequisiteID != null)
            Prerequisite = tableComponents
                .OfType<CategoryComponent>()
                .FirstOrDefault(c => c.CategoryData.ID == (int)CategoryData.PrerequisiteID);

        transform.SetSiblingIndex(CategoryData.Row);

        base.Initialize(tableComponents);

    }

    #endregion

    #region IsChanged

    protected override bool IsChanged()
    {
        if (CategoryData == null)
            return false;

        if (CategoryData.Name != Name)
            return true;
        if (CategoryData.Icon != Icon)
            return true;
        if (CategoryData.Price != Price)
            return true;
        if (CategoryData.ParentID != GetParentID())
            return true;
        if (CategoryData.Row != transform.GetSiblingIndex())
            return true;
        if (CategoryData.PrerequisiteID != (Prerequisite != null ? (int?)Prerequisite.ID : null))
            return true;

        return false;
    }

    #endregion

    #region IsValid

    public override string IsValid()
    {
        if (GetParentID() == null)
            if (transform.parent.GetComponent<DatabaseComponent>() == null)
                return string.Format("Category \"{0}\" has unknown parent!!!", Name);

        return "";
    }


    #endregion

    #region AddSubcategory

    public bool AddSubcategory(Category newCategory)
    {
        Create(newCategory, transform);

        return true;
    }
    #endregion

    #region Delete

    public bool Delete()
    {
        DestroyImmediate(gameObject);
        return true;
    }
    

    #endregion

    public bool AddPuzzle(Puzzle puzzle)
    {
        // Create CategoryComponent
        PuzzleComponent.Create(puzzle, transform);

        return true; 
    }
}


