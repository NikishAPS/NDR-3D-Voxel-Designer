using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputKeys : MonoBehaviour
{
    public bool lmb, rmb, mmb, shift, lCtrl;


    void Start()
    {
        
    }

    void Update()
    {
        lmb = Input.GetMouseButtonDown(0);
        rmb = Input.GetMouseButtonDown(1);
        mmb = Input.GetMouseButtonDown(2);

        shift = Input.GetKeyDown(KeyCode.LeftControl);
    }
}
