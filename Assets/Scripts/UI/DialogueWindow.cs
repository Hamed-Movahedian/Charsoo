using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MgsCommonLib.Utilities;

public class DialogueWindow : MonoBehaviour
{

    #region Actions

    private Dictionary<string, Action> _actionDic=
        new Dictionary<string, Action>();

    public void RunAction(string name)
    {
        if (_actionDic.ContainsKey(name))
            _actionDic[name]();
    }

    public void SetAction(string lable,Action action)
    {
        if (_actionDic.ContainsKey(lable))
            _actionDic[lable] = action;
        else
            _actionDic.Add(lable, action);
    }

    #endregion

    #region InputFields

    public string this[string name]
    {
        get {
            InputField inputField = GetInputField(name);

            if (inputField)
                return inputField.text;

            Debug.LogError("Input field " + name + " not found!!");

            return "";
        }
        set
        {
            InputField inputField = GetInputField(name);

            if (inputField)
                inputField.text=value;

            Debug.LogError("Input field " + name + " not found!!");
        }
    }
 

    private InputField GetInputField(string name)
    {
        var inputFields = GetComponentsInChildren<InputField>();

        foreach (var inputField in inputFields)
        {
            if (inputField.name.ToLower().Trim() == name.ToLower().Trim())
                return inputField;
        }
        return null;

    }

    #endregion

    #region Get Window

    public static DialogueWindow GetWindow(string name)
    {
        var window = FindObjectsOfType<DialogueWindow>()
            .FirstOrDefault(w => w.name == name);

        if (window != null)
            return window;

        Debug.LogError("Dialogue window "+name+" not found!!");

        return null;

    }
    

    #endregion

    public IEnumerator ShowWindowAndWaitForClose()
    {
        transform
            .GetChilds()
            .ForEach(c=>c.gameObject.SetActive(false));


        foreach (Transform childs in transform)
        {
            
        }
        yield return null;
    }
}

