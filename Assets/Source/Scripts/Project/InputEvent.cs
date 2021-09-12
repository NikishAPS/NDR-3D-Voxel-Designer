using UnityEngine;

public class InputEvent : MonoBehaviour
{
    //mouse events
    public static bool GetLMouseDown => Input.GetMouseButtonDown(0);
    public static bool GetLMouseHold => Input.GetMouseButton(0);
    public static bool GetLMouseUp => Input.GetMouseButtonUp(0);
    public static bool GetRMouseDown => Input.GetMouseButtonDown(1);
    public static bool GetRMouseHold => Input.GetMouseButton(1);
    public static bool GetRMouseUp => Input.GetMouseButtonUp(1);
    public static Void MouseMove;
    public static Void LMouseDown;
    public static Void LMouseHold;
    public static Void LMouseUp;
    public static Void NotMouse0;
    public static Void RMouseDown;
    public static Void RMouseHold;
    public static Void RMouseUp;

    public static Vector3 MousePosition => Input.mousePosition;
    public static Vector3 MouseSpeed { get; private set; }

    //keyboard events
    public static Void Delete;
    public static bool LShift => Input.GetKey(KeyCode.LeftShift);
    public static bool LShiftDown => Input.GetKeyDown(KeyCode.LeftShift);
    public static bool GetDelete => Input.GetKeyDown(KeyCode.Delete);

    private Vector3 _prevMousePosition;



    private void Update()
    {
        MouseSpeed = MousePosition - _prevMousePosition;
        if (MouseSpeed != Vector3.zero) MouseMove?.Invoke();
        _prevMousePosition = MousePosition;

        if (GetLMouseDown) LMouseDown?.Invoke();
        if (GetLMouseHold) LMouseHold?.Invoke();
        if (GetLMouseUp) LMouseUp?.Invoke();
        if (GetRMouseDown) RMouseDown?.Invoke();
        if (GetRMouseHold) RMouseHold?.Invoke();
        if (GetRMouseUp) RMouseUp?.Invoke();
        if (GetDelete) Delete?.Invoke();
    }
}
