using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using SQLite4Unity3d;
using UnityEngine;
using UnityEngine.UI;

public class UICategoryMenuItem : UIMenuItem
{
    //private bool _avalable;
    public Image Icon;
    public Text Name;


    [Header("Conditions")]
    public GameObject SubCategoryGameObject;
    public GameObject CheckMarckGameObject;
    public GameObject BuyGameObject;
    public Text CounterText;
    public GameObject NewIconGameObject;
    private Category _category;

    protected override void Refresh(object data)
    {
        _category = (Category)data;
        //_avalable = IsCategoryAvalable(category);

        Name.text = ArabicFixer.Fix(_category.Name);

        NewIconGameObject.SetActive(!_category.Visit);


        if (LocalDBController.Table<Category>().SqlWhere(c => c.ParentID == _category.ID).Any())
        {
            SubCategoryGameObject.SetActive(true);
            CheckMarckGameObject.SetActive(false);
            BuyGameObject.SetActive(false);
            CounterText.gameObject.SetActive(false);
            GetComponent<RectTransform>().localScale = Vector3.one;
            return;
        }
        SubCategoryGameObject.SetActive(false);

        var puzzles = LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == _category.ID).ToList();

        var solveCount = puzzles.Count(p => p.Solved);

        BuyGameObject.SetActive(!IsCategoryAvalable());

        CheckMarckGameObject.SetActive(solveCount == puzzles.Count);
        CounterText.gameObject.SetActive(solveCount != puzzles.Count);

        CounterText.text = string.Format("{0}/{1}",
            ArabicFixer.Fix(solveCount.ToString(), true, true),
            ArabicFixer.Fix(puzzles.Count.ToString(), true, true));

        if (!IsCategoryAvalable())
            CounterText.text = string.Format(ArabicFixer.Fix("300", true, true));
        else
            CounterText.text = string.Format("{0}/{1}",
                ArabicFixer.Fix(solveCount.ToString(), true, true),
                ArabicFixer.Fix(puzzles.Count.ToString(), true, true));

        GetComponent<RectTransform>().localScale = Vector3.one;

    }

    private bool IsCategoryAvalable()
    {
        bool preIsSolved = true;

        if (_category.PrerequisiteID!=null)
        {

            Category preCat = LocalDBController.Table<Category>().FirstOrDefault(c => c.ID == _category.PrerequisiteID);

            if (preCat != null)
                foreach (Puzzle p in LocalDBController.Table<Puzzle>().Where(p => p.CategoryID == preCat.ID))
                    preIsSolved = p.Solved;
        }


        if (_category.Price <= 0 && preIsSolved)
            return true;

        if (
            LocalDBController.Table<Purchases>()
            .FirstOrDefault(p => p.PurchaseID == "C-" + _category.ID) != null
            )
            return true;

        return false;
    }

    public override void Select()
    {
        if (IsCategoryAvalable())
        {
            if (!_category.Visit)
            {
                _category.Visit = true;
                LocalDBController.InsertOrReplace(_category);
            }
            base.Select();
        }
        else
            ((LocalCategorySelectionWindow)_list).LockSelect(_category);
    }
}
