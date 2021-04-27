using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTest : MonoBehaviour
{
    public int id = 1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            id += (id == 0) ? 7 : -1;
        if (Input.GetKeyDown(KeyCode.RightArrow))
            id += (id == 7) ? 0 : 1;
    }
}
