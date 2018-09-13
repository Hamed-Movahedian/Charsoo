using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class UICategoryMenuItem : UIMenuItem
{
    private bool _avalable;
    public Image Icon;
    public Text Name;


    [Header("Conditions")]
    public GameObject SubCategoryGameObject;
    public GameObject CheckMarckGameObject;
    public GameObject BuyGameObject;
    public Text CounterText;
    public GameObject NewIconGameObject;

    protected override void Refresh(object data)
    {
        var category = (Category)data;
        _avalable = IsCategoryAvalable(category);

        Name.text = ArabicFixer.Fix(category.Name);

        NewIconGameObject.SetActive(!category.Visit);


        if (LocalDBController.Table<Category>().SqlWhere(c => c.ParentID == category.ID).Any())
        {
            SubCategoryGameObject.SetActive(true);
            CheckMarckGameObject.SetActive(false);
            BuyGameObject.SetActive(false);
            CounterText.gameObject.SetActive(false);
            GetComponent<RectTransform>().localScale = Vector3.one;
            return;
        }

        var puzzles = LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == category.ID).ToList();

        var solveCount = puzzles.Count(p => p.Solved);

        BuyGameObject.SetActive(_avalable);

        SubCategoryGameObject.SetActive(false);
        CheckMarckGameObject.SetActive(solveCount == puzzles.Count);
        CounterText.gameObject.SetActive(solveCount != puzzles.Count);

        CounterText.text = string.Format("{0}/{1}",
            ArabicFixer.Fix(solveCount.ToString(), true, true),
            ArabicFixer.Fix(puzzles.Count.ToString(), true, true));


        GetComponent<RectTransform>().localScale = Vector3.one;
    }

    private static bool IsCategoryAvalable(Category category)
    {
        if (category.Price <= 0) return true;
        List<Purchases> list = LocalDBController.Table<Purchases>().SqlWhere(p => p.PurchaseID == "C-" + category.ID).ToList();
        Debug.Log(list.Count);
        if (list.Count>0) return true;
        return false;
    }

    public override void Select()
    {
        if (_avalable)
            base.Select();
        else
            ((LocalCategorySelectionWindow)_list).LockSelect();
    }
}
