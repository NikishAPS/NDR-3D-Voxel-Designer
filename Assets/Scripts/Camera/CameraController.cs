using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static Vector3 Position
    {
        get => _this.transform.position;
        set => _this.transform.position = value;
    }
    public static Camera MainCamera { get; private set; }
    public static Void MoveEvent;
    public static float Size => MainCamera.orthographicSize;
    public static float Distance => MainCamera.nearClipPlane;
    public static Vector3 ViewDirection => _this.transform.forward;
    public static Vector3 WorldMouse => MainCamera.ScreenToWorldPoint(Input.mousePosition) + ViewDirection * Distance;

    private static CameraController _this;
    [SerializeField] private float _sensitivity = 350f;
    [SerializeField] private float _movingSpeed = 10f;
    [SerializeField] private float _rotationSpeed = 700f;
    [SerializeField] private float _zoomingSpeed = 5f;
    [SerializeField] private float _zoomingStep = 0.2f;
    [SerializeField] private float _minZoom = 1f, _maxZoom = 100f;

    public static float GetDistance(Vector3 targerPosition)
    {
        return Vector3.Distance(MainCamera.transform.position, targerPosition) + MainCamera.orthographicSize;
    }

    public static void OffsetPosition(Vector3 target)
    {
        float speed = (Position - target).magnitude * _this._movingSpeed;
        _this.StartCoroutine(_this.Moving(target, speed));
    }

    public static void SetViewDirectino(Vector3 viewDirection)
    {
        _this.StartCoroutine(_this.Rotating(viewDirection, _this._rotationSpeed));
    }

    private void Awake()
    {
        MainCamera = GetComponent<Camera>();
        _this = FindObjectOfType<CameraController>();

        InputEvent.MMouseHold += OnMMouseHold;
        InputEvent.MouseScroll += OnMouseScroll;
        InputEvent.SpaceDown += OnSpaceDown;

        MainCamera.nearClipPlane = -_maxZoom;
        MainCamera.nearClipPlane = -20;

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
            Vector2 mouseDirection = -InputEvent.MouseSpeed * _sensitivity * Time.deltaTime  * Size * 0.0005f;
            transform.position += transform.TransformDirection(mouseDirection);
        }

        MoveEvent?.Invoke();
    }

    private void OnMouseScroll()
    {
        if (InputEvent.IsXHold || InputEvent.IsYHold || InputEvent.IsZHold) return;

        float target = MainCamera.orthographicSize - InputEvent.IsMouseScroll * MainCamera.orthographicSize * _zoomingStep;
        float speed = Mathf.Sqrt(Mathf.Pow(target - MainCamera.orthographicSize, 2)) * _zoomingSpeed;
        StartCoroutine(Zooming(target, speed));
    }

    private void OnSpaceDown()  
    {
        Vector3 target = ChunkManager.SelectedVoxelCount == 0 ? 
            new Vector3(ChunkManager.Center.x, 0, ChunkManager.Center.z) : 
            ChunkManager.MiddleSelectedPos;

        OffsetPosition(target);
    }

    private IEnumerator Moving(Vector3 target, float speed)
    {
        Vector3 currentPosition = transform.position;

        while (currentPosition != target)
        {
            Vector3 deltaPosition = currentPosition;
            currentPosition = Vector3.MoveTowards(currentPosition, target, Time.deltaTime * speed);
            deltaPosition = currentPosition - deltaPosition;

            transform.position += deltaPosition;

            MoveEvent?.Invoke();

            yield return null;
        }
    }

    private IEnumerator Rotating(Vector3 viewDirection, float speed)
    {
        Quaternion rotation = Quaternion.LookRotation(viewDirection);
        while(transform.rotation != rotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * speed);
            MoveEvent?.Invoke();

            yield return null;
        }
    }

    private IEnumerator Zooming(float target, float speed)
    {
        float currentZoom = MainCamera.orthographicSize;

        while(currentZoom != target)
        {
            float delta = currentZoom;
            currentZoom = Mathf.MoveTowards(currentZoom, target, Time.deltaTime * speed);
            delta = currentZoom - delta;

            MainCamera.orthographicSize += delta;

            bool isClamp = false;
            if (MainCamera.orthographicSize <= _minZoom)
            {
                MainCamera.orthographicSize = _minZoom;
                isClamp = true;
            }
            else if (MainCamera.orthographicSize >= _maxZoom)
            {
                MainCamera.orthographicSize = _maxZoom;
                isClamp = true;
            }

            MoveEvent?.Invoke();

            if (isClamp) break;

            yield return null;
        }
    }

}

