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

    public List<Sprite> Sprites;
    private int _iconIndex=0;

    protected override void Refresh(object data)
    {
        _category = (Category)data;
        //_avalable = IsCategoryAvalable(category);

        Name.text = PersianFixer.Fix(_category.Name);
        NewIconGameObject.SetActive(!_category.Visit);

        _iconIndex = int.Parse(_category.Icon);
        Icon.sprite = Sprites[_iconIndex];


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

        CheckMarckGameObject.SetActive(_category.Completed);
        CounterText.gameObject.SetActive(!_category.Completed);

        if (!IsCategoryAvalable())
            CounterText.text = string.Format(PersianFixer.Fix("300", true, true));
        else
            CounterText.text =
                $"{PersianFixer.Fix(solveCount.ToString(), true, true)}/{PersianFixer.Fix(puzzles.Count.ToString(), true, true)}";

        GetComponent<RectTransform>().localScale = Vector3.one;

    }

    private bool IsCategoryAvalable()
    {
        if (_category.PrerequisiteID != null)
        {
            Category preCat = LocalDBController.Table<Category>().FirstOrDefault(c => c.ID == _category.PrerequisiteID);

            if (preCat != null)
            {
                if (preCat.Completed)
                    return true;

                return _category.IsPurchased;
            }
        }

        return _category.IsPurchased || _category.Price == 0;
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
            Singleton.Instance.Table.SetBackground(_iconIndex);
            base.Select();
        }
        else
            ((LocalCategorySelectionWindow)_list).LockSelect(_category);
    }
}
