using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ArabicSupport;
using MgsCommonLib.Theme;
using UnityEngine;
using UnityEngine.UI;

public class CategoryMenuItem : BaseObject
{


    #region Public

    public Image Icon;
    public Text Name;

    [Header("Conditions")]
    public GameObject SubCategoryGameObject;
    public GameObject CheckMarckGameObject;
    public GameObject BuyGameObject;
    public Text CounerText;
    public GameObject NewIconGameObject;

    #endregion

    #region Private

    private Category _data;

    #endregion

    #region Setup

    public void Setup(Category category)
    {
        _data = category;

        SetVisuals();

        gameObject.SetActive(true);
    }

    #endregion

    #region SetVisuals

    private void SetVisuals()
    {
        Name.text = ArabicFixer.Fix(_data.Name);
        NewIconGameObject.SetActive(!_data.Visit);
        // if has subcategory
        if (LocalDBController.Table<Category>()
            .SqlWhere(c => c.ParentID == _data.ID)
            .Any())
        {
            SubCategoryGameObject.SetActive(true);
            CheckMarckGameObject.SetActive(false);
            BuyGameObject.SetActive(false);
            CounerText.gameObject.SetActive(false);
            return;
        }

        
        // if has puzzles
        var puzzles = LocalDBController.Table<Puzzle>()
            .SqlWhere(p => p.CategoryID == _data.ID)
            .ToList();

        var solveCount = puzzles.Count(p => p.Solved);

        SubCategoryGameObject.SetActive(false);
        CheckMarckGameObject.SetActive(solveCount==puzzles.Count);
        BuyGameObject.SetActive(false);
        CounerText.gameObject.SetActive(solveCount != puzzles.Count);

        CounerText.text = string.Format("{0}/{1}", 
            ArabicFixer.Fix(solveCount.ToString(),true,true),
            ArabicFixer.Fix(puzzles.Count.ToString(), true, true));

    }

    #endregion

    #region Select

    public void Select()
    {
        if (_data.Price > 0 || _data.PrerequisiteID != null)
            if (!CheckCategoryLock())
            {
                UnlockCategory();
                return;
            }

        ShowSubContent();

    }

    private void ShowSubContent()
    {
        ContentManager.ShowContent(_data);
        if (!_data.Visit)
        {
            _data.Visit = true;
            LocalDBController.Update(_data);
            SetVisuals();
        }
    }

    private bool CheckCategoryLock()
    {
        return false;
    }

    private void UnlockCategory()
    {
/*
        if (PurchaseManager.PayCoin(_data.Price != 0 ? _data.Price :200))
        {
            _data.PaieD = true;
            ShowSubContent();

        }
        else
        {
            OnLowMoney.Invoke();
        }
#1#
*/
    }

    #endregion

}
