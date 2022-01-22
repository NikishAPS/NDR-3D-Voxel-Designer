using System;
using UnityEngine;

[Serializable]
public class VertexData
{
    public readonly Vector3Int Position;
    public readonly Vector3 OffsetPosition;
    public readonly Vector3 Offset;

    public VertexData(VertexUnit vertex)
    {
        Position = vertex.Position;
        OffsetPosition = vertex.OffsetPosition;
        Offset = vertex.GetOffset();
    }
}