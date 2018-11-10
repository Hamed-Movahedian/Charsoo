using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : BaseObject
{
    public int Size=19;
    public List<Texture2D> BackGrounds=new List<Texture2D>();

    private Material _material;


    #region BoundRect

    private Rect _boundRect=Rect.zero;

    public Rect BoundRect
    {
        get
        {
            if (_boundRect.height == 0)
            {
                Bounds bounds = GetComponent<Renderer>().bounds;
                _boundRect=new Rect(bounds.min,bounds.size);
            }
            return _boundRect;
        }
    }
    
    #endregion

    // Use this for initialization
    void Start()
    {
        _material = GetComponent<MeshRenderer>().sharedMaterial;
        SetSize();
    }

    public void SetRandomBackground()
    {
        if(BackGrounds.Count>0) _material.mainTexture = BackGrounds[Random.Range(0, BackGrounds.Count)];
        SetSize();
    }

    public void ResetBackground()
    {
        if (BackGrounds.Count > 0) _material.mainTexture = BackGrounds[0];
        SetSize();
        _boundRect=new Rect(0,0,0,0);
    }

    [ContextMenu("SetSize")]
    public void SetSize()
    {
        transform.position =5*Vector3.forward;

        Size = _material.mainTexture.height;
        transform.localScale = new Vector3(Size, Size, 1);
        if (Size%2 == 0)
            transform.position = new Vector3(0.5f, 0.5f, 5);

    }
}
