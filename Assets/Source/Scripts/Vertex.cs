﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public readonly Vector3 PivotPosition;
    public Vector3 Position { get; private set; }

    public Vertex()
    {

    }

    public Vertex(Vector3 position)
    {
        PivotPosition = Position = position;
    }

    public Vertex(Vertex vertex)
    {
        PivotPosition = vertex.PivotPosition;
        Position = vertex.Position;
    }

    public Vertex(Vector3 pivotPosition, Vector3 offset)
    {
        PivotPosition = pivotPosition;
        Position = PivotPosition + offset;
    }

    public void SetOffset(Vector3 offset)
    {
        Position = PivotPosition + offset;
    }

    public Vector3 GetOffset()
    {
        return Position - PivotPosition;
    }
}
