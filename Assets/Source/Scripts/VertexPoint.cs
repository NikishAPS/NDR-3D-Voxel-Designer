using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexPoint
{
    public readonly Vector3 Position;
    public readonly List<Vertex> Vertices;

    public VertexPoint(Vector3 position)
    {
        Position = position;
    }

}
