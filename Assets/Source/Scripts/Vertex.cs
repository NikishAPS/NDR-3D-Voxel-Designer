using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public readonly Vector3 PivotPosition;
    public Vector3 Position { get; private set; }
    public int AdjacentVoxelsCount { get; set; }

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

    public Vertex(VertexData vertexData)
    {
        PivotPosition = vertexData.PivotPosition;
        Position = vertexData.Position;
    }

    public void Offset(Vector3 offset)
    {
        Position += offset;
    }

    public void SetOffset(Vector3 offset)
    {
        Position = PivotPosition + offset;
    }

    public void SetPosition(Vector3 position)
    {
        Position = position;
    }

    public Vector3 GetOffset()
    {
        return Position - PivotPosition;
    }

    public VertexData GetData()
    {
        return new VertexData(PivotPosition, Position);
    }

}
