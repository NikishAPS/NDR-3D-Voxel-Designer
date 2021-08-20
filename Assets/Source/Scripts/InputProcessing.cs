using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProcessing : MonoBehaviour
{
    public delegate void EventTemplate();

    public Vector3 MousePosition => Input.mousePosition;
    
    

    private Vector3 _prevMousePosition;

    private void Update()
    {
        InputEvent.MouseSpeed = MousePosition - _prevMousePosition;
        if (InputEvent.MouseSpeed != Vector3.zero) InputEvent.MouseMove?.Invoke();
        _prevMousePosition = MousePosition;

        if (InputEvent.GetLMouseDown) InputEvent.LMouseDown?.Invoke();
        if (InputEvent.GetLMouseHold) InputEvent.LMouseHold?.Invoke();
        if (InputEvent.GetLMouseUp) InputEvent.LMouseUp?.Invoke();
        if (InputEvent.GetRMouseDown) InputEvent.RMouseDown?.Invoke();
        if (InputEvent.GetRMouseHold) InputEvent.RMouseHold?.Invoke();
        if (InputEvent.GetRMouseUp) InputEvent.RMouseUp?.Invoke();
    }

}