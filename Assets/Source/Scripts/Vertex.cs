using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public static Vertex()
    {
        offset = 1;
    }


    public static int offset;
    public Vector3 Position { get; private set; }
    private Vector3 _offset;
}
