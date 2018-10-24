using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using UnityEngine;

public class PuzzleComponent : TableComponent
{
    public Puzzle PuzzleData;
    public string Clue;

    [HideInInspector]
    public string Content;

    #region Create

    public static void Create(Puzzle puzzle, Transform parenTransform)
    {
        // Creat game object
        GameObject go = new GameObject();
        go.transform.parent = parenTransform;

        // Create PuzzleComponent
        var puzzleComponent = go.AddComponent<PuzzleComponent>();

        // Setup PuzzleComponent
        puzzleComponent.ID = puzzle.ID;
        puzzleComponent.PuzzleData = puzzle;
        puzzleComponent.Clue = puzzle.Clue;
        puzzleComponent.Content = puzzle.Content;
        puzzleComponent.Dirty = false;
    }
    

    #endregion

    #region GetName

    protected override string GetName()
    {
        return "P - " + Clue; 
        return "P - " + PersianFixer.Fix(Clue); 
    }


    #endregion

    #region Initialize

    public override void Initialize(TableComponent[] tableComponents)
    {
        transform.SetSiblingIndex(PuzzleData.Row);
        base.Initialize(tableComponents);
    }


    #endregion

    #region IsChanged

    protected override bool IsChanged()
    {
        if (PuzzleData == null)
            return false;

        if (PuzzleData.Clue != Clue)
            return true;

        if (PuzzleData.Content != Content)
            return true;

        if (PuzzleData.CategoryID != GetParentID())
            return true;

        if (PuzzleData.Row != transform.GetSiblingIndex())
            return true;
        
        return false;
    }

    #endregion

    #region UpdateData

    public override void UpdateData()
    {
        if (!Dirty)
            return;

        PuzzleData.Clue = Clue;
        PuzzleData.Content = Content;

        PuzzleData.CategoryID = GetParentID();
        PuzzleData.Row = transform.GetSiblingIndex();

        Dirty = false;
    }


    #endregion

    #region IsValid

    public override string IsValid()
    {
        if (GetParentID() == null)
        {
            return string.Format("Puzzle \"{0}\" has no category!!!", Clue);
        }
        if (transform.childCount != 0)
        {
            return string.Format("Puzzle \"{0}\" has child!!!", Clue);
        }

        return "";
    }


    #endregion

    #region Reload

    protected override void Reload()
    {
        if (PuzzleData == null)
          gameObject.name = "Reload !!!";
        
    }


    #endregion

    #region Delete

    public bool Delete()
    {
        DestroyImmediate(gameObject);
        return true;
    }


    #endregion

}
