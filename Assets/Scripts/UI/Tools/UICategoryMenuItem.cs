using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ArabicSupport;
using UnityEngine;
using UnityEngine.UI;

public class UICategoryMenuItem : UIMenuItem
{

    public Image Icon;
    public Text Name;

    [Header("Conditions")]
    public GameObject SubCategoryGameObject;
    public GameObject CheckMarckGameObject;
    public GameObject BuyGameObject;
    public Text CounerText;
    public GameObject NewIconGameObject;


    protected override void Refresh(object data)
    {
        var category = (Category)data;

        Name.text = ArabicFixer.Fix(category.Name);

        NewIconGameObject.SetActive(!category.Visit);
        BuyGameObject.SetActive(category.Price > 0);

        if (LocalDBController.Table<Category>().SqlWhere(c => c.ParentID == category.ID).Any())
        {
            SubCategoryGameObject.SetActive(true);
            CheckMarckGameObject.SetActive(false);
            BuyGameObject.SetActive(false);
            CounerText.gameObject.SetActive(false);
            GetComponent<RectTransform>().localScale = Vector3.one;
            return;
        }

        var puzzles = LocalDBController.Table<Puzzle>().SqlWhere(p => p.CategoryID == category.ID).ToList();

        var solveCount = puzzles.Count(p => p.Solved);

        SubCategoryGameObject.SetActive(false);
        CheckMarckGameObject.SetActive(solveCount == puzzles.Count);
        CounerText.gameObject.SetActive(solveCount != puzzles.Count);

        CounerText.text = string.Format("{0}/{1}",
            ArabicFixer.Fix(solveCount.ToString(), true, true),
            ArabicFixer.Fix(puzzles.Count.ToString(), true, true));


        GetComponent<RectTransform>().localScale = Vector3.one;
    }

}
