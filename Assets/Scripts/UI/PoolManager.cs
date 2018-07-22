using System;
using System.Collections.Generic;
using MgsCommonLib;
using UnityEngine;

internal class PoolManager : MgsSingleton<PoolManager>
{
    private readonly Dictionary<Type, List<Component>> _poolDic = new Dictionary<Type, List<Component>>();
    public Component Get(Component component, Transform parent=null)
    {
        Component component1;

        // get type
        Type type = component.GetType();

        // add key if needed 
        if (!_poolDic.ContainsKey(type))
            _poolDic.Add(type, new List<Component>());

        if (_poolDic[type].Count > 0)
        {
            component1 = _poolDic[type][0];
            _poolDic[type].Remove(component1);
        }
        else
        {
            component1 = Instantiate(component);
        }

        // Set parent if needed
        if(parent!=null)
            component1.transform.SetParent(parent);

        component1.gameObject.SetActive(true);

        return component1;
    }

    public void Return(Component component)
    {
        // disable
        component.gameObject.SetActive(false);

        // add to this game object childs
        component.transform.SetParent(this.transform);

        // get type
        Type type = component.GetType();

        // add key if needed 
        if (!_poolDic.ContainsKey(type))
            _poolDic.Add(type, new List<Component>());

        // add to list
        _poolDic[type].Add(component);

    }
}