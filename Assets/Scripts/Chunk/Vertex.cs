using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public readonly Vector3 PivotPosition;
    public Vector3 Position { get; set; }
    public Vector3 Offset {
        get => Position - PivotPosition;
        set => Position = PivotPosition + value;
    }
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

    public void Shift(Vector3 value)
    {
        Position += value;
    }

    public VertexData GetData()
    {
        return new VertexData(PivotPosition, Position);
    }

}
