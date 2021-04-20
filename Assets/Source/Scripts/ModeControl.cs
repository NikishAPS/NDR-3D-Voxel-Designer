using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeControl : MonoBehaviour
{
    public int mode { get; set; }

    MonoBehaviour[] employees;

    void Start()
    {
        employees = new MonoBehaviour[]
        {
            GetComponent<Builder>(),
            new Mover(),
            new Editor()
        };
    }

    void Update()
    {
        
    }
}
