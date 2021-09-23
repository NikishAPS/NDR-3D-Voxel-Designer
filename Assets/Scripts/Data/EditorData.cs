using System;
using UnityEngine;

[Serializable]
public class EditorData
{
    public int VertexCount;

    public EditorData(int vertexCount)
    {
        VertexCount = vertexCount;
    }
}
