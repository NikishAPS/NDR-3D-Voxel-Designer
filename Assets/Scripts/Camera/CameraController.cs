﻿using System.Collections;
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
    [SerializeField] private float _sensitivity = 250f;
    [SerializeField] private float _minZoom = 1f, _maxZoom = 100f;
    [SerializeField] private float _zoomSpeed = 0.2f;
    [SerializeField] private float _viewChangingSpeed = 100f;

    public static float GetDistance(Vector3 targerPosition)
    {
        return Vector3.Distance(MainCamera.transform.position, targerPosition) + MainCamera.orthographicSize;
    }

    public static void SetViewDirectino(Vector3 viewDirection)
    {
        _this.StartCoroutine(_this.Rotating(viewDirection));
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

        float speed = MainCamera.orthographicSize * _zoomSpeed;
        float zoom = MainCamera.orthographicSize - InputEvent.IsMouseScroll * speed;
        StartCoroutine(Zooming(zoom, speed));
    }

    private void OnSpaceDown()  
    {
        Vector3 target = ChunkManager.MiddleSelectedPos;
        StartCoroutine(Moving(target, 10));
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

    //private IEnumerator Zooming(float lastZoom, float value, float speed)
    private IEnumerator Zooming(float target, float speed)
    {
       //float curZoom = lastZoom;
        float currentZoom = MainCamera.orthographicSize;

        while(currentZoom != target)
        {
            float delta = currentZoom;
            currentZoom = Mathf.MoveTowards(currentZoom, target, Time.deltaTime * 5 * speed);
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


public struct Sq
{

}

public static class Ex
{
    public static void F(this Sq sq)
    {

    }
}
