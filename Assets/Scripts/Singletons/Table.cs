using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    public int Size=19;

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
        SetSize();
    }

    [ContextMenu("SetSize")]
    public void SetSize()
    {
        transform.position =Vector3.zero;

        Size = GetComponent<MeshRenderer>().sharedMaterial.mainTexture.height;
        transform.localScale = new Vector3(Size, Size, 1);
        if (Size%2 == 0)
            transform.position = new Vector3(0.5f, 0.5f, 0);

    }


    // UpdateData is called once per frame
    void Update()
    {

    }
}
