using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Void MoveEvent;
    public static float Distance => Camera.main.orthographicSize;

    [SerializeField] private Camera _axesCamera;
    [SerializeField] private float _sensitivity = 250f;
    [SerializeField] private float _minZoom = 1f, _maxZoom = 100f;
    [SerializeField] private float _zoomSpeed = 0.2f;

    [SerializeField]
    private Vector3 _target;

    private Vector2 angles;
    private float _zoom;

    public static float GetDistance(Vector3 targerPosition)
    {
        return Vector3.Distance(Camera.main.transform.position, targerPosition) + Camera.main.orthographicSize;
    }

    private void Awake()
    {
        InputEvent.MMouseHold += OnMMouseHold;
        InputEvent.MouseScroll += OnMouseScroll;

        _zoom = Camera.main.orthographicSize;
        Camera.main.nearClipPlane = -_maxZoom;
        Camera.main.nearClipPlane = -20;

        _axesCamera.orthographicSize = Camera.main.orthographicSize;
        _axesCamera.nearClipPlane = Camera.main.nearClipPlane;

        _target.y = 0;
        angles = new Vector2(320f, -50f);
    }

    private void OnMMouseHold()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            angles.x = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Time.deltaTime * _sensitivity;
            angles.y += Input.GetAxis("Mouse Y") * Time.deltaTime * _sensitivity;

            angles.y = Mathf.Clamp(angles.y, -90, 90);
        }
        else
        {
            Vector2 mouseDir = -InputEvent.MouseSpeed * _sensitivity * Time.deltaTime  * Distance * 0.0005f;

            _target += transform.TransformDirection(mouseDir);
        }

        transform.localEulerAngles = new Vector3(-angles.y, angles.x, 0);
        //transform.position = transform.localRotation * (Vector3.forward) + _target;
        //transform.position = transform.localRotation * (-Vector3.forward) + _target;
        transform.position = _target;

        MoveEvent?.Invoke();
    }

    private void OnMouseScroll()
    {
        float lastZoom = _zoom;
        float speed = _zoom * _zoomSpeed;

        _zoom -= InputEvent.GetMouseScroll * speed;
        _zoom = Mathf.Clamp(_zoom, 1, 100);
        StartCoroutine(CameraZoom(lastZoom, _zoom, speed));
    }

    private IEnumerator CameraZoom(float lastZoom, float zoom, float speed)
    {
        float curzoom = lastZoom;

        while(curzoom != zoom)
        {
            float smooth = curzoom;
            curzoom = Mathf.MoveTowards(curzoom, zoom, Time.deltaTime * 5 * speed);
            smooth = curzoom - smooth;

            Camera.main.orthographicSize += smooth;
            _axesCamera.orthographicSize = Camera.main.orthographicSize;

            MoveEvent?.Invoke();

            yield return null;
        }

        yield return null;
    }

}

