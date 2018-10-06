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


    #region UpdateData

    void Update()
    {
        if (!_enablePan)
            return;

        if (Input.touchCount > 1)
            return;

        #region Start Drag letter or Pan

        if (Input.GetMouseButtonDown(0))
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

        if (Input.GetMouseButton(0))
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

        if (Input.GetMouseButtonUp(0) )
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
