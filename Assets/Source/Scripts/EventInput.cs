using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventInput : MonoBehaviour
{
    public delegate void EventTemplate();

    public Vector2 MousePos => Input.mousePosition; //{ get { return Input.mousePosition; } private set }

    public EventTemplate alpha1;
    public EventTemplate alpha2;
    public EventTemplate alpha3;

    public bool LShift => Input.GetKeyDown(KeyCode.LeftShift);
    public bool Mouse0 => Input.GetMouseButtonDown(0);
    public bool Mouse1 => Input.GetMouseButtonDown(1);
    public bool Mouse2 => Input.GetMouseButtonDown(2);

    public EventTemplate tab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) alpha1?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha2)) alpha2?.Invoke();
        if (Input.GetKeyDown(KeyCode.Alpha3)) alpha3?.Invoke();



        if (Input.GetKeyDown(KeyCode.Tab)) tab?.Invoke();
    }
}
