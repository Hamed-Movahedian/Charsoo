using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public float Velocity = 1;
    public Vector3 Translate = Vector3.right;
    public float Delay = 0.5f;


    private Vector3 _startPos;
    private Vector3 _targetPos;
    // Use this for initialization
    void Start()
    {

    }

    private void OnEnable()
    {
        _startPos = transform.position;
        _targetPos = _startPos + Translate;
        StartCoroutine(MoveTo(Delay));
    }

    private void OnDisable()
    {
        transform.position = _startPos;
    }



    public IEnumerator MoveTo(float delay)
    {
        yield return new WaitForSeconds(delay);

        while ((transform.position - _targetPos).magnitude > .1)
        {
            transform.position = Vector3.MoveTowards(transform.position, _targetPos, Velocity * Time.deltaTime);
            yield return null;
        }
        transform.position = _targetPos;
        gameObject.SetActive(false);
    }


    // UpdateData is called once per frame
    void Update()
    {


    }
}
