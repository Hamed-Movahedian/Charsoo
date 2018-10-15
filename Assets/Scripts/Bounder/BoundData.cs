using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

public class BoundData
{
    private GameObject gameObject;

    private object _baseObject;

    private List<BoundMember> _boundMembers = new List<BoundMember>();

    private List<MemberInfo> _nextLevelMembers;

    public BoundData(GameObject boundObject, string boundText)
    {
        _boundMembers = new List<BoundMember>();

        if (boundObject != null && boundText != "")
        {
            gameObject = boundObject;

            var bTexts = boundText.Split('.').ToList();

            List<Type> types = GetCandidateBaseTypes();

            BaseType = types.FirstOrDefault(t => t.Name == bTexts[0]);
            
            if (BaseType != null)
            {
                var type = BaseType;

                for (int i = 1; i < bTexts.Count; i++)
                {
                    var memberInfo = new BoundMember(type, bTexts[i]);

                    _boundMembers.Add(memberInfo);

                    type = memberInfo.ReturnType;
                }
            }
        }
    }

    public GameObject GameObject
    {
        get
        {
            return gameObject;
        }
        set
        {
            if (value != gameObject)
            {
                gameObject = value;
                _baseObject = null;
                _boundMembers.Clear();
                _nextLevelMembers=null;
            }
        }
    }

    public Type BaseType
    {
        get
        {
            return _baseObject.GetType();
        }
        set
        {
            if (value != _baseObject.GetType())
            {
                if (value == null)
                    _baseObject = null;
                else if (value == typeof(GameObject))
                    _baseObject = gameObject;
                else
                    _baseObject = gameObject.GetComponent(value);

                _boundMembers.Clear();
                _nextLevelMembers=null;
            }
        }
    }

    public List<MemberInfo> NextLevelMembers
    {
        get
        {
            if(_nextLevelMembers==null)
                UpdateNextLevelMembers();
            return _nextLevelMembers;
        }
    }

    public string BoundText
    {
        get
        {
            var text = BaseType.Name;

            _boundMembers.ForEach(bm => text += "." + bm.Name);

            return text;
        }
    }

    public bool HasMember => _boundMembers.Count > 0;

    private void UpdateNextLevelMembers()
    {
        if (BaseType == null)
        {
            _nextLevelMembers = new List<MemberInfo>();
            return;
        }

        var type = BaseType;

        if (_boundMembers.Count > 0)
            type = _boundMembers[_boundMembers.Count - 1].ReturnType;

        _nextLevelMembers = type
            .GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => BounderUtilitys.IsValidMember(m))
            .OrderBy(m => m.Name)
            .OrderBy(m => type.GetTypeDistance(m.DeclaringType))
            .ToList();

    }

    public List<Type> GetCandidateBaseTypes()
    {
        var types = gameObject.GetComponents<Component>()
            .Select(c => c.GetType())
            .ToList();

        types.Insert(0, typeof(GameObject));
        return types;
    }
    
    public void RemoveLastMemeber()
    {
        if (_boundMembers.Count > 0)
        {
            _boundMembers.RemoveAt(_boundMembers.Count - 1);
            _nextLevelMembers = null;

        }
    }

    public void Add(MemberInfo memberInfo)
    {
        _boundMembers.Add(new BoundMember(memberInfo));
        _nextLevelMembers = null;
    }

    public object GetValue(Type type)
    {
        var obj = _baseObject;

        if(obj==null)
            throw new Exception($"Invalid bound data!!!");

        foreach (var member in _boundMembers)
            obj = member.GetValue(obj);

        return obj;
    }

    public void OnGUI()
    {
        if(_boundMembers.Count > 0)
            _boundMembers[_boundMembers.Count - 1].OnGUI();
    }
}
