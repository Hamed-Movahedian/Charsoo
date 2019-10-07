using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : BaseObject
{
    #region Public

    public LayerMask LetterLayerMask;
    private bool _isPlaying = true;
    #endregion

    #region Private

    private Camera _camera;
    private bool _letterDrag=false;
    private Vector3 _lastDragPos;
    private bool _cameraDrag = false;
    private bool _enablePan = true;

    #endregion

    #region Start

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    #endregion

    public void TriggerRaycast(bool playing)
    {
        _isPlaying = playing;
    }


    private void OnEnable()
    {
        EnablePan(true);
    }

    #region UpdateData

    void Update()
    {
/*
        
        if (!_enablePan)
            return;
*/

        if (Input.touchCount > 1)
        {
            if (_letterDrag)
            {
                StartCoroutine(LetterController.LetterUnselected());
                _letterDrag = false;
                return;

            }
        }

        bool startCondition=false, dragCondition=false, endCondition=false;
        Vector3 pos=Vector3.zero;
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                startCondition =  
                    Input.touches[0].phase == TouchPhase.Began || (!_letterDrag && !_cameraDrag);
                dragCondition=Input.touches[0].phase == TouchPhase.Moved;
                endCondition=Input.touches[0].phase == TouchPhase.Ended;
                pos = Input.touches[0].position;
            }
            else if (Input.touchCount == 2)
            {
                endCondition=true;
                pos = Input.touches[0].position;
            }

        }
        else
        {
            startCondition = Input.GetMouseButtonDown(0);
            dragCondition = Input.GetMouseButton(0);
            endCondition = Input.GetMouseButtonUp(0);
            pos = Input.mousePosition;
        }
        
        #region Start Drag letter or Pan

        if (startCondition)
        {
            _lastDragPos = _camera.ScreenToWorldPoint(pos);

            Collider2D collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(pos), LetterLayerMask);

            if (collider != null)
            {
                if (!_isPlaying)
                    return;

                Letter letter = collider.GetComponent<Letter>();

                if (letter != null /*&& !letter.IsLocked*/)
                {
                    LetterController.LetterSelected(letter);
                    _letterDrag = true;
                }
            }
            else if (_enablePan)
            {
                _cameraDrag = true;
                CameraController.StartPan();
            }


            return;
        }

        #endregion

        #region Letter Draging or Pan

        if (dragCondition)
        {
            Vector3 dragpos = _camera.ScreenToWorldPoint(pos);
            
            if(_letterDrag)
                LetterController.Move(dragpos-_lastDragPos);

            if (_cameraDrag)
                CameraController.Pan();

            _lastDragPos = dragpos;
        }

        #endregion

        #region Letter end drag

        if (endCondition)
        {
            if (_letterDrag)
            {
                StartCoroutine(LetterController.LetterUnselected());
                _letterDrag = false;
                
            }
            if (_cameraDrag)
            {
                _cameraDrag = false;
            }
        }

        #endregion

    }


    #endregion

    public void EnablePan(bool value)
    {
        _enablePan = value;
    }
}



/*
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycaster : BaseObject
{
    #region Public

    public LayerMask LetterLayerMask;
    private bool _isPlaying = true;
    #endregion

    #region Private

    private Camera _camera;
    private bool _letterDrag=false;
    private Vector3 _lastDragPos;
    private bool _cameraDrag = false;
    private bool _enablePan = true;
    private bool _lastButton;

    #endregion

    #region Start

    void Start()
    {
        _camera = GetComponent<Camera>();
    }

    #endregion

    public void TriggerRaycast(bool playing)
    {
        _isPlaying = playing;
    }


    #region UpdateData

    void Update()
    {
        if (!_enablePan)
            return;

        #region Start Drag letter or Pan

        if (GetButtonDown())
        {
            _lastDragPos = _camera.ScreenToWorldPoint(Input.mousePosition);

            Collider2D collider = Physics2D.OverlapPoint(_camera.ScreenToWorldPoint(Input.mousePosition), LetterLayerMask);

            if (collider != null)
            {
                if (!_isPlaying)
                    return;

                Letter letter = collider.GetComponent<Letter>();

                if (letter != null)
                    LetterController.LetterSelected(letter);
                else
                    Debug.LogError("Letter component doesn't exist!!!");

                _letterDrag = true;

            }
            else
            {
                _cameraDrag = true;
                CameraController.StartPan();
            }


            return;
        }

        #endregion

        #region Letter Draging or Pan

        if (GetButton())
        {
            Vector3 dragpos = _camera.ScreenToWorldPoint(Input.mousePosition);
            
            if(_letterDrag)
                LetterController.Move(dragpos-_lastDragPos);

            if (_cameraDrag)
                CameraController.Pan();

            _lastDragPos = dragpos;
        }

        #endregion

        #region Letter end drag

        //if (GetButtonUp() )
        if (GetButtonUp())
        {
            if (_letterDrag)
            {
                StartCoroutine(LetterController.LetterUnselected());
                _letterDrag = false;
                
            }
            if (_cameraDrag)
            {
                _cameraDrag = false;
            }
        }

        #endregion

        _lastButton = GetButton();
    }

    private bool GetButtonUp()
{
    if (Application.platform != RuntimePlatform.Android)
        return Input.GetMouseButtonUp(0);

    return Input.touchCount != 1 && _lastButton;

    if (Input.touchCount > 0)
        return Input.GetTouch(0).phase == TouchPhase.Ended;
    else
        return false;


}
private bool GetButtonDown()
{
    if (Application.platform != RuntimePlatform.Android)
        return Input.GetMouseButtonDown(0);

    return Input.touchCount == 1 && !_lastButton;


}

private bool GetButton()
{
    if (Application.platform == RuntimePlatform.Android)
        return Input.touchCount == 1;

    return Input.GetMouseButton(0);
}

#endregion

public void EnablePan(bool value)
{
    _enablePan = value;
}
}

 */