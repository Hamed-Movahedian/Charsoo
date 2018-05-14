using System;
using System.Collections;
using MgsCommonLib;

public class UIController : MgsSingleton<UIController>
{
    internal IEnumerator DisplayInprogress(string message)
    {
        yield return null;
    }
}