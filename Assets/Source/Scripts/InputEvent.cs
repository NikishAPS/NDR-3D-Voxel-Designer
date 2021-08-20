using UnityEngine;

public static class InputEvent
{
    public delegate void EventTemplate();

    public static Vector3 MouseSpeed { get; set; }

    //mouse events
    public static bool GetLMouseDown => Input.GetMouseButtonDown(0);
    public static bool GetLMouseHold => Input.GetMouseButton(0);
    public static bool GetLMouseUp => Input.GetMouseButtonUp(0);
    public static bool GetRMouseDown => Input.GetMouseButtonDown(1);
    public static bool GetRMouseHold => Input.GetMouseButton(1);
    public static bool GetRMouseUp => Input.GetMouseButtonUp(1);
    public static EventTemplate MouseMove;
    public static EventTemplate LMouseDown;
    public static EventTemplate LMouseHold;
    public static EventTemplate LMouseUp;
    public static EventTemplate NotMouse0;
    public static EventTemplate RMouseDown;
    public static EventTemplate RMouseHold;
    public static EventTemplate RMouseUp;

    //keyboard events
    public static bool LShift => Input.GetKey(KeyCode.LeftShift);
    public static bool LShiftDown => Input.GetKeyDown(KeyCode.LeftShift);
    public static bool GetDelete => Input.GetKeyDown(KeyCode.Delete);
}
