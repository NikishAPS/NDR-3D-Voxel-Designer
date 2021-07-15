using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexPoint
{
    public readonly Vector3 Position;
    public readonly List<int> VertexIndices;

    public VertexPoint(Vector3 position)
    {
        VertexIndices = new List<int>();
        Position = position;
    }

    public void AddVertexIndex(int index)
    {
        foreach (int verIndex in VertexIndices)
        {
            if (verIndex == index) return;
        }
        VertexIndices.Add(index);
    }

    public void RemoveVertexIndex(int index)
    {
        for (int i = 0; i < VertexIndices.Count; i++)
        {
            if(VertexIndices[i] == index)
            {
                VertexIndices.RemoveAt(i);
                break;
            }
        }
    }

    public bool IsEmpty()
    {
        return VertexIndices.Count == 0;
    }

    public int GetLastVertexIndex()
    {
        return VertexIndices.Count > 0 ? VertexIndices[VertexIndices.Count - 1] : -1;
    }
}
