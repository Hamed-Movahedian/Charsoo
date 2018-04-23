using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : UIDialogueWindowBase<LoginWindow>
{
    public InputField NameField;

    public Action OnHasAccountAction;

    public string Name
    {
        get { return NameField.text; }
        set { NameField.text = value; }
    }

    public string Avatar
    {
        get { return ""; }
        set { }
    }

    public void OnHasAccount()
    {
        if(OnHasAccountAction!=null)
            OnHasAccountAction();
    }
   
}
