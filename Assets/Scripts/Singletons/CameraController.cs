using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : BaseObject
{
    public float ScrollZoomSpeed = 0.3f;
    public float FinalZoomDuration = 2;
    public float VerticalOffset = 0;
    public float ZoomRatio = 1.5f;

    public float OrthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.

    private Vector3 _startmousePosition;
    private Vector3 _camraStartPan;
    private Camera _camera;
    private Vector3 _lastMousePos;

    public void Start()
    {
        _camera = GetComponent<Camera>();
    }

    void Update()
    {
        float delta = Input.GetAxis("Mouse ScrollWheel");

        if (delta != 0)
            Zoom(Mathf.Sign(delta)*ScrollZoomSpeed);

        if (Input.touchCount == 2)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
            // If the camera is orthographic...

            // ... change the orthographic size based on the change in distance between the touches.
            _camera.orthographicSize += deltaMagnitudeDiff*OrthoZoomSpeed;

            // Make sure the orthographic size never drops below zero.
            _camera.orthographicSize = Mathf.Max(_camera.orthographicSize, 7);

            KeepCameraInTableBounds();
        }
    }

    private void Zoom(float z)
    {
        _camera.orthographicSize += z;

        KeepCameraInTableBounds();
    }


    public void StartPan()
    {
        _lastMousePos = Input.mousePosition;
    }

    public void Pan()
    {
        transform.position +=
            _camera.ScreenToWorldPoint(_lastMousePos) -
            _camera.ScreenToWorldPoint(Input.mousePosition);

        KeepCameraInTableBounds();

        _lastMousePos = Input.mousePosition;
    }

    private void KeepCameraInTableBounds()
    {
        if (_camera.orthographicSize * 2 > Table.BoundRect.height)
            _camera.orthographicSize = Table.BoundRect.height / 2;


        Vector2 min = _camera.ViewportToWorldPoint(Vector3.zero);
        Vector2 max = _camera.ViewportToWorldPoint(Vector3.one);

        Rect tableRect = Table.BoundRect;

        if (min.x < tableRect.min.x)
            transform.position += Vector3.right * (tableRect.min.x - min.x);

        if (min.y < tableRect.min.y)
            transform.position += Vector3.up * (tableRect.min.y - min.y);

        if (max.x > tableRect.max.x)
            transform.position += Vector3.left * (max.x - tableRect.max.x);

        if (max.y > tableRect.max.y)
            transform.position += Vector3.down * (max.y - tableRect.max.y);
    }

    public IEnumerator FocusToBound(Bounds bound)
    {
        // Start pos & size

        float startSize = _camera.orthographicSize;
        Vector3 startPos = transform.position;
        // End size
        float endSize = (Mathf.Max(bound.extents.x, bound.extents.y) * 2 + 3 / _camera.aspect);
        // End pos
        Vector3 endPos = bound.center + Vector3.down * VerticalOffset;

        endPos.z = transform.position.z;


        // ******** Start animation

        AnimationCurve ValueCurve = AnimationCurve.EaseInOut(0, 0, FinalZoomDuration, 1);

        float time = 0;

        while (time < FinalZoomDuration)
        {
            float value = ValueCurve.Evaluate(time);

            transform.position = Vector3.Lerp(startPos, endPos, value);

            _camera.orthographicSize = Mathf.Lerp(startSize, endSize, value);

            KeepCameraInTableBounds();

            yield return null;

            time += Time.deltaTime;
        }
    }

    public IEnumerator FocusAllLetters()
    {
        yield return FocusToBound(LetterController.GetLettersBound());
    }

    public void H_FocusAll()
    {
        StartCoroutine(FocusAllLetters());
    }
}
