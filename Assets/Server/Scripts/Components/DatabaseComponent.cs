using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class DatabaseComponent : MonoBehaviour
{
    #region Public

    

    #endregion

    #region Private

    private List<Category> _Categories = new List<Category>();
    private List<Puzzle> _puzzles = new List<Puzzle>();

    #endregion

    #region ReloadAll

    public void ReloadAll()
    {

        // delete all childs
        while (transform.childCount > 0)
            DestroyImmediate(transform.GetChild(0).gameObject);


        CreateCategory(transform);

        // Initialize 
        var tableComponents = GetComponentsInChildren<TableComponent>();

        foreach (var child in tableComponents)
            child.Initialize(tableComponents);
    }

 


    #endregion
    
    #region CreateCategory

    private void CreateCategory(Transform parenTransform)
    {
        // Get category ID
        var categoryComponent = parenTransform.GetComponent<CategoryComponent>();
        int? categoryID = categoryComponent != null ? (int?)categoryComponent.CategoryData.ID : null;

        // Find all subCategories
        var subCategories = _Categories.Where(c => c.ParentID == categoryID);

        // Create category components
        foreach (Category subCategory in subCategories)
            CreateCategory(CategoryComponent.Create(subCategory, parenTransform, this));

        // Find all puzzles
        var puzzles = _puzzles.Where(p => p.CategoryID == categoryID);

        // Create puzzle components
        foreach (var puzzle in puzzles)
        {
            PuzzleComponent.Create(puzzle, parenTransform, this);
        }
    }


    #endregion
 
    public T Reload<T>(int id) where T : BaseTable, new()
    {

        return null; //DataService.Connection.Table<T>().SqlWhere(r => r.ID == id).FirstOrDefault();
    }

    public bool Create(object o)
    {
        return true; //DataService.Connection.Insert(o)!=0;

    }

    public bool Delete(object o)
    {
        return true; //DataService.Connection.Delete(o) != 0;
    }


    #region Set Categories and puzzles from server

    public void SetCategories(List<Category> categories)
    {
        _Categories = categories;
    }

    public void SetPuzzles(List<Puzzle> puzzles)
    {
        _puzzles = puzzles;
    }

    #endregion


}
