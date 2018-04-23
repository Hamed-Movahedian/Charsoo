using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class CategoryDataManager : BaseObject
{
    private void Start()
    {
        CommandController.AddListenerForCommand("AddCategories", AddCategories);
    }

    private void AddCategories(JToken dataToken)
    {
        // Get new categories from json
        List<Category> newcategories = dataToken.Select(ct => ct.ToObject<Category>()).ToList();

        // Add or update local db
        foreach (Category newcategory in newcategories)
        {
            newcategory.Visit = false;

            LocalDatabase.InsertOrReplace(newcategory);
        }

        CommandController.LastCmdTime = newcategories.Max(c => c.LastUpdate);

    }
}
