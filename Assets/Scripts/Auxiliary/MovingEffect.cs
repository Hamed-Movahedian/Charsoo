using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovingEffect : MonoBehaviour
{
    private Transform _parent;
    public float MoveTime;
    public UnityEvent OnArrive;
    public float LifeTime;
    private float _velocity;
    private Vector3 _startPos;
    // Use this for initialization
	void Start ()
	{
	    _startPos = transform.localPosition;
	}

    private void OnEnable()
    {
        _parent = transform.parent;
        _velocity = (_parent.position - transform.position).magnitude/MoveTime;
        StartCoroutine(GotoParrent());
    }

    private IEnumerator GotoParrent()
    {
        while ((transform.position -_parent.position).magnitude >0.5f)
            {
                transform.position=
                Vector3.MoveTowards
                (transform.position,_parent.position,_velocity*Time.deltaTime);
                yield return null;
            }
        transform.position = _parent.position;
        OnArrive.Invoke();
        //Destroy(_parent.gameObject,LifeTime);
        transform.localPosition = _startPos;
        _parent.gameObject.SetActive(false);
    }

}
