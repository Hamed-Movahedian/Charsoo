using System.Collections;
using System.Collections.Generic;
using MgsCommonLib.UI;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldDone : MonoBehaviour 
{
     private InputField inputField;
     public Button button;

     private void Start()
     {
          inputField = GetComponent<InputField>();

          if (inputField && button)
          {
               inputField.onEndEdit.AddListener(Done);
          }
     }

     private void OnEnable()
     {
          if(inputField==null)
               inputField = GetComponent<InputField>();

          inputField.text = "";
     }

     private void Done(string s)
     {
          button.onClick.Invoke();
     }

}

