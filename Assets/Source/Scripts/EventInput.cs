using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInput : MonoBehaviour
{
    public delegate void EventTemplate();

    public Vector3 MousePos => Input.mousePosition; //{ get { return Input.mousePosition; } private set }

    public EventTemplate MouseMove, MouseDown0, Mouse0, MouseUp0, NotMouse0;
    public EventTemplate Delete;

    public EventTemplate alpha1;
    public EventTemplate alpha2;
    public EventTemplate alpha3;


    public bool GetMouseHold0 => Input.GetMouseButton(0);
    public bool GetMouseDown0 => Input.GetMouseButtonDown(0);
    public bool GetMouseDown1 => Input.GetMouseButtonDown(1);
    public bool GetMouseDown2 => Input.GetMouseButtonDown(2);
    public bool LShift => Input.GetKey(KeyCode.LeftShift);
    public bool LShiftDown => Input.GetKeyDown(KeyCode.LeftShift);
    public bool GetDelete => Input.GetKeyDown(KeyCode.Delete); 

    public EventTemplate tab;

    private Vector3 _mousePos;

    private void Update()
    {
        if (MousePos - _mousePos != Vector3.zero) MouseMove?.Invoke();

        if (Input.GetMouseButtonDown(0)) MouseDown0?.Invoke();
        if (Input.GetMouseButton(0)) Mouse0?.Invoke(); else NotMouse0?.Invoke();
        if (Input.GetMouseButtonUp(0)) MouseUp0?.Invoke();

        if (Input.GetKeyDown(KeyCode.Alpha1)) alpha1?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha2)) alpha2?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha3)) alpha3?.Invoke();
        if (Input.GetKeyDown(KeyCode.Delete)) Delete.Invoke();



        if (Input.GetKeyDown(KeyCode.Tab)) tab?.Invoke();
    }
}
