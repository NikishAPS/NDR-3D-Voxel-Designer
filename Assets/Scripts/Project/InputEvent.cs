using UnityEngine;

public class InputEvent : MonoBehaviour
{
    //app events
    public static Void WindowResize;

    //mouse events
    public static bool IsLMouseDown => Input.GetMouseButtonDown(0);
    public static bool IsLMouseHold => Input.GetMouseButton(0);
    public static bool IsLMouseUp => Input.GetMouseButtonUp(0);
    public static bool IsRMouseDown => Input.GetMouseButtonDown(1);
    public static bool IsRMouseHold => Input.GetMouseButton(1);
    public static bool IsRMouseUp => Input.GetMouseButtonUp(1);
    public static bool IsMMouseDown => Input.GetMouseButtonDown(2);
    public static bool IsMMouseHold => Input.GetMouseButton(2);
    public static bool IsMMouseUp => Input.GetMouseButtonUp(2);
    public static float IsMouseScroll => Input.mouseScrollDelta.y;

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
    public static bool IsLShiftHold => Input.GetKey(KeyCode.LeftShift);
    public static bool IsLShiftDown => Input.GetKeyDown(KeyCode.LeftShift);
    public static bool IsDeleteDown => Input.GetKeyDown(KeyCode.Delete);
    public static bool IsXHold => Input.GetKey(KeyCode.X);
    public static bool IsYHold => Input.GetKey(KeyCode.Y);
    public static bool IsZHold => Input.GetKey(KeyCode.Z);
    public static bool IsSpaceDown => Input.GetKeyDown(KeyCode.Space);

    public static Void DeleteDown;
    public static Void XHold;
    public static Void YHold;
    public static Void ZHold;
    public static Void SpaceDown;

    [SerializeField] private Canvas _canvas;
    private Rect _prevCanvasRect;
    private Vector3 _prevMousePosition;

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

        if (IsLMouseDown) LMouseDown?.Invoke();
        if (IsLMouseHold) LMouseHold?.Invoke();
        if (IsLMouseUp) LMouseUp?.Invoke();
        if (IsRMouseDown) RMouseDown?.Invoke();
        if (IsRMouseHold) RMouseHold?.Invoke();
        if (IsRMouseUp) RMouseUp?.Invoke();
        if (IsMMouseDown) MMouseDown?.Invoke();
        if (IsMMouseHold) MMouseHold?.Invoke();
        if (IsMMouseUp) MMouseUp?.Invoke();
        if (IsMouseScroll != 0) MouseScroll?.Invoke();

        if (IsDeleteDown) DeleteDown?.Invoke();
        if (IsXHold) XHold?.Invoke();
        if (IsYHold) YHold?.Invoke();
        if (IsZHold) ZHold?.Invoke();

        if (IsSpaceDown) SpaceDown?.Invoke();
    }

}
