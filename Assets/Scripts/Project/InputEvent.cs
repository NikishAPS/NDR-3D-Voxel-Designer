using UnityEngine;

public class InputEvent : MonoBehaviour
{
    //app events
    public static Void WindowResize;

    //mouse events
    public static bool GetLMouseDown => Input.GetMouseButtonDown(0);
    public static bool GetLMouseHold => Input.GetMouseButton(0);
    public static bool GetLMouseUp => Input.GetMouseButtonUp(0);
    public static bool GetRMouseDown => Input.GetMouseButtonDown(1);
    public static bool GetRMouseHold => Input.GetMouseButton(1);
    public static bool GetRMouseUp => Input.GetMouseButtonUp(1);
    public static bool GetMMouseDown => Input.GetMouseButtonDown(2);
    public static bool GetMMouseHold => Input.GetMouseButton(2);
    public static bool GetMMouseUp => Input.GetMouseButtonUp(2);
    public static float GetMouseScroll => Input.mouseScrollDelta.y;

    public static Void MouseMove;
    public static Void LMouseDown;
    public static Void LMouseHold;
    public static Void LMouseUp;
    public static Void NotMouse0;
    public static Void RMouseDown;
    public static Void RMouseHold;
    public static Void RMouseUp;
    public static Void MMouseDown;
    public static Void MMouseHold;
    public static Void MMouseUp;
    public static Void MouseScroll;

    public static Vector3 MousePosition => Input.mousePosition;
    public static Vector3 MouseSpeed { get; private set; }

    //keyboard events
    public static bool LShift => Input.GetKey(KeyCode.LeftShift);
    public static bool LShiftDown => Input.GetKeyDown(KeyCode.LeftShift);
    public static bool GetDelete => Input.GetKeyDown(KeyCode.Delete);
    public static bool GetXHold => Input.GetKey(KeyCode.X);
    public static bool GetYHold => Input.GetKey(KeyCode.Y);
    public static bool GetZHold => Input.GetKey(KeyCode.Z);

    public static Void Delete;
    public static Void XHold;
    public static Void YHold;
    public static Void ZHold;

    private Canvas _canvas;
    private Rect _prevCanvasRect;
    private Vector3 _prevMousePosition;

    private void Awake()
    {
        _canvas = FindObjectOfType<Canvas>();
    }

    private void Update()
    {
        if (_canvas.pixelRect != _prevCanvasRect)
        {
            WindowResize?.Invoke();
            _prevCanvasRect = _canvas.pixelRect;
        }

        MouseSpeed = MousePosition - _prevMousePosition;
        if (MouseSpeed != Vector3.zero)
        {
            MouseMove?.Invoke();
            _prevMousePosition = MousePosition;
        }

        if (GetLMouseDown) LMouseDown?.Invoke();
        if (GetLMouseHold) LMouseHold?.Invoke();
        if (GetLMouseUp) LMouseUp?.Invoke();
        if (GetRMouseDown) RMouseDown?.Invoke();
        if (GetRMouseHold) RMouseHold?.Invoke();
        if (GetRMouseUp) RMouseUp?.Invoke();
        if (GetMMouseDown) MMouseDown?.Invoke();
        if (GetMMouseHold) MMouseHold?.Invoke();
        if (GetMMouseUp) MMouseUp?.Invoke();
        if (GetMouseScroll != 0) MouseScroll?.Invoke();

        if (GetDelete) Delete?.Invoke();
        if (GetXHold) XHold?.Invoke();
        if (GetYHold) YHold?.Invoke();
        if (GetZHold) ZHold?.Invoke();
    }

}
