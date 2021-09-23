using System;
using UnityEngine;

[Serializable]
public class VertexData
{
    public Vector3 PivotPosition;
    public Vector3 Position;

    public VertexData(Vector3 pivotPosition, Vector3 position)
    {
        PivotPosition = pivotPosition;
        Position = position;
    }
}