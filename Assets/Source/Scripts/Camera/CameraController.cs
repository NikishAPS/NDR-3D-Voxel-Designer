using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Void MoveEvent;
    public static float Distance => Camera.main.orthographicSize;

    private static CameraController _this;

    [SerializeField] private Camera _axesCamera;
    [SerializeField] private float _sensitivity = 250f;
    [SerializeField] private float _minZoom = 1f, _maxZoom = 100f;
    [SerializeField] private float _zoomSpeed = 0.2f;
    [SerializeField] private float _viewChangingSpeed = 100f;

    [SerializeField]
    private Vector3 _target;

    private float _zoom;

    public static float GetDistance(Vector3 targerPosition)
    {
        return Vector3.Distance(Camera.main.transform.position, targerPosition) + Camera.main.orthographicSize;
    }

    public static void SetViewDirectino(Vector3 viewDirection)
    {
        _this.StartCoroutine(_this.Rotating(viewDirection));
    }

    private void Awake()
    {
        _this = FindObjectOfType<CameraController>();
        InputEvent.MMouseHold += OnMMouseHold;
        InputEvent.MouseScroll += OnMouseScroll;

        _zoom = Camera.main.orthographicSize;
        Camera.main.nearClipPlane = -_maxZoom;
        Camera.main.nearClipPlane = -20;

        _axesCamera.orthographicSize = Camera.main.orthographicSize;
        _axesCamera.nearClipPlane = Camera.main.nearClipPlane;

        _target.y = 0;
        transform.position = _target;
        transform.localEulerAngles = new Vector2(20f, 50f);
    }

    private void OnMMouseHold()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            float offsetAngleY = (transform.localEulerAngles.x) > 90f ? 360f : 0f;
            float angleX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Time.deltaTime * _sensitivity;
            float angleY = transform.localEulerAngles.x - offsetAngleY - Input.GetAxis("Mouse Y") * Time.deltaTime * _sensitivity;

            angleY = Mathf.Clamp(angleY, -90f, 90f);

            transform.localEulerAngles = new Vector3(angleY, angleX, 0);
        }
        else
        {
            Vector2 mouseDir = -InputEvent.MouseSpeed * _sensitivity * Time.deltaTime  * Distance * 0.0005f;

            _target += transform.TransformDirection(mouseDir);
        }

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
        StartCoroutine(Zooming(lastZoom, _zoom, speed));
    }

    

    private IEnumerator Zooming(float lastZoom, float zoom, float speed)
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
    }

    private IEnumerator Rotating(Vector3 viewDirection)
    {
        Quaternion rotation = Quaternion.LookRotation(viewDirection);
        while(transform.rotation != rotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * 10 * _viewChangingSpeed);
            //transform.localEulerAngles = new Vector3(-angles.y, angles.x);
            MoveEvent?.Invoke();

            yield return null;
        }
    }

}

