using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ContentManager : BaseObject
{
    #region Public

    [Header("Base Parameters")]
    public UnlockPanel UnlockPanel;


    [Header("Header")]
    public Text HeaderTitle;
    public DynamicList DynamicList;

    [HideInInspector]
    public PuzzleMenuItem SelectedPuzzleItem;

    [Header("Events")]
    public UnityEvent OnWordSetSelected;
    public UnityEvent OnExitToMain;
    public UnityEvent OnShowPackPanel;

    #endregion

    #region Private

    private Category _parentCategory;

    #endregion

    #region Start

    void Start()
    {
    }

    #endregion

    #region ShowRoot

    public void ShowRoot()
    {
        ShowContent(null);
    }

    #endregion

    #region ShowContent

    public void ShowContent(Category parentCateroy)
    {
        // Set parent
        _parentCategory = parentCateroy;

        // Set Header
        HeaderTitle.text = parentCateroy!=null ? ArabicFixer.Fix(parentCateroy.Name) : "";

        // Clear list
        DynamicList.Clear();

        #region Get subCategories 

        int? id = parentCateroy != null ? (int?) parentCateroy.ID : null;

        var subCategories = LocalDBController
            .Table<Category>()
            .SqlWhere(c => c.ParentID == id);

        #endregion

        #region Create subcategories

        if (subCategories.Any())
        {
            foreach (var category in subCategories)
            {
                // get new item
                CategoryMenuItem categoryMenuItem = DynamicList.GetFreeItem<CategoryMenuItem>();
                
                // setup 
                categoryMenuItem.Setup(category);

                // Add to list
                DynamicList.Add(categoryMenuItem.GetComponent<RectTransform>());
            }

            // finalize list
            DynamicList.End();

            return;
        }

        #endregion

        #region Get Puzzles
        
        var puzzles = LocalDBController
            .Table<Puzzle>()
            .SqlWhere(puzzle => puzzle.CategoryID == parentCateroy.ID)
            .OrderBy(p=>p.Row);

        #endregion

        #region Create puzzles

        if (puzzles.Any())
        {
            foreach (var puzzle in puzzles)
            {
                // get new item
                var puzzleMenuItem = DynamicList.GetFreeItem<PuzzleMenuItem>();

                // setup 
                puzzleMenuItem.Setup(puzzle);

                // Add to list
                DynamicList.Add(puzzleMenuItem.GetComponent<RectTransform>());
            }

            // finalize list
            DynamicList.End();

            return;
        }

        #endregion
    }
    #endregion

    #region Back

    public void Back()
    {
        if (_parentCategory == null)
            OnExitToMain.Invoke();

        else if (_parentCategory.ParentID == null)
            ShowContent(null);
        else
        {
            var parentCategory = LocalDBController
                .Table<Category>()
                .FirstOrDefault(c => c.ID == _parentCategory.ParentID.Value);

            ShowContent(parentCategory);

        }
    }

    #endregion

    #region StartNextWordSet

    public void StartNextWordSet()
    {
        ContentManager.SelectedPuzzleItem.StartNext();

    }

    #endregion

    #region CurrentWordSolved

    public void CurrentWordSolved()
    {
        ContentManager.SelectedPuzzleItem.Solved();
    }
    
    #endregion
}
