using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInput : MonoBehaviour
{
    public delegate void EventTemplate();

    public Vector3 MousePos => Input.mousePosition; //{ get { return Input.mousePosition; } private set }

    public EventTemplate mouseDown0, mouse0, mouseUp0;

    public EventTemplate alpha1;
    public EventTemplate alpha2;
    public EventTemplate alpha3;

    public bool MouseDown0 => Input.GetMouseButtonDown(0);
    public bool MouseDown1 => Input.GetMouseButtonDown(1);
    public bool MouseDown2 => Input.GetMouseButtonDown(2);
    public bool LShift => Input.GetKey(KeyCode.LeftShift);
    public bool LShiftDown => Input.GetKeyDown(KeyCode.LeftShift);

    public EventTemplate tab;

    private void Update()
    {

        if (Input.GetMouseButtonDown(0)) mouseDown0?.Invoke();
        if (Input.GetMouseButton(0)) mouse0?.Invoke();
        if (Input.GetMouseButtonUp(0)) mouseUp0?.Invoke();

        if (Input.GetKeyDown(KeyCode.Alpha1)) alpha1?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha2)) alpha2?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha3)) alpha3?.Invoke();



        if (Input.GetKeyDown(KeyCode.Tab)) tab?.Invoke();
    }
}
