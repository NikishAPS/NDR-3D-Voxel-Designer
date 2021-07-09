using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor : ChunkEmployee
{
    public readonly List<int> CreatedVertexIndices;

    public Editor(Chunk chunk) : base(chunk)
    {
        CreatedVertexIndices = new List<int>();
    }

    public void CreateVertices(Vector3Int voxelPos)
    {
        TryCreateVertexByPos(voxelPos + new Vector3(-0.5f, -0.5f, -0.5f));
        TryCreateVertexByPos(voxelPos + new Vector3(+0.5f, -0.5f, -0.5f));
        TryCreateVertexByPos(voxelPos + new Vector3(+0.5f, +0.5f, -0.5f));
        TryCreateVertexByPos(voxelPos + new Vector3(-0.5f, +0.5f, -0.5f));

        TryCreateVertexByPos(voxelPos + new Vector3(-0.5f, -0.5f, +0.5f));
        TryCreateVertexByPos(voxelPos + new Vector3(+0.5f, -0.5f, +0.5f));
        TryCreateVertexByPos(voxelPos + new Vector3(+0.5f, +0.5f, +0.5f));
        TryCreateVertexByPos(voxelPos + new Vector3(-0.5f, +0.5f, +0.5f));
    }

    public void AddVertices(Vertex[] vertices, Vector3Int voxelPos)
    {
        TryAddVertex(vertices[0], voxelPos + new Vector3(-0.5f, -0.5f, -0.5f));
        TryAddVertex(vertices[1], voxelPos + new Vector3(+0.5f, -0.5f, -0.5f));
        TryAddVertex(vertices[2], voxelPos + new Vector3(+0.5f, +0.5f, -0.5f));
        TryAddVertex(vertices[3], voxelPos + new Vector3(-0.5f, +0.5f, -0.5f));

        TryAddVertex(vertices[4], voxelPos + new Vector3(-0.5f, -0.5f, +0.5f));
        TryAddVertex(vertices[5], voxelPos + new Vector3(+0.5f, -0.5f, +0.5f));
        TryAddVertex(vertices[6], voxelPos + new Vector3(+0.5f, +0.5f, +0.5f));
        TryAddVertex(vertices[7], voxelPos + new Vector3(-0.5f, +0.5f, +0.5f));
    }

    public void DeleteVerticesByPos(Vector3Int voxelPos)
    {
        TryDeleteVertexByPos(voxelPos + new Vector3(-0.5f, -0.5f, -0.5f));
        TryDeleteVertexByPos(voxelPos + new Vector3(+0.5f, -0.5f, -0.5f));
        TryDeleteVertexByPos(voxelPos + new Vector3(+0.5f, +0.5f, -0.5f));
        TryDeleteVertexByPos(voxelPos + new Vector3(-0.5f, +0.5f, -0.5f));

        TryDeleteVertexByPos(voxelPos + new Vector3(-0.5f, -0.5f, +0.5f));
        TryDeleteVertexByPos(voxelPos + new Vector3(+0.5f, -0.5f, +0.5f));
        TryDeleteVertexByPos(voxelPos + new Vector3(+0.5f, +0.5f, +0.5f));
        TryDeleteVertexByPos(voxelPos + new Vector3(-0.5f, +0.5f, +0.5f));
    }

    public Vertex GetVertexByCreatedIndices(int createdIndices)
    {
        return _chunk.Vertices[CreatedVertexIndices[createdIndices]];
    }

    public Vertex[] GetVertices(Vector3Int voxelPos)
    {
        Vertex[] vertices = new Vertex[8];

        vertices[0] = new Vertex(_chunk.GetVertexByPos(voxelPos + new Vector3(-0.5f, -0.5f, -0.5f)));
        vertices[1] = new Vertex(_chunk.GetVertexByPos(voxelPos + new Vector3(+0.5f, -0.5f, -0.5f)));
        vertices[2] = new Vertex(_chunk.GetVertexByPos(voxelPos + new Vector3(+0.5f, +0.5f, -0.5f)));
        vertices[3] = new Vertex(_chunk.GetVertexByPos(voxelPos + new Vector3(-0.5f, +0.5f, -0.5f)));

        vertices[4] = new Vertex(_chunk.GetVertexByPos(voxelPos + new Vector3(-0.5f, -0.5f, +0.5f)));
        vertices[5] = new Vertex(_chunk.GetVertexByPos(voxelPos + new Vector3(+0.5f, -0.5f, +0.5f)));
        vertices[6] = new Vertex(_chunk.GetVertexByPos(voxelPos + new Vector3(+0.5f, +0.5f, +0.5f)));
        vertices[7] = new Vertex(_chunk.GetVertexByPos(voxelPos + new Vector3(-0.5f, +0.5f, +0.5f)));

        return vertices;
    }

    private void TryCreateVertexByPos(Vector3 vertexPos)
    {
        int index = _chunk.GetVertexIndexByPos(vertexPos);

        if (index == -1 || _chunk.Vertices[index] != null) return;

        _chunk.Vertices[index] = new Vertex(vertexPos);
        CreatedVertexIndices.Add(index); 
    }

    private void TryAddVertex(Vertex vertex, Vector3 vertexPos)
    {
        if(_chunk.GetVertexByPos(vertexPos) == null)
        {
            _chunk.Vertices[_chunk.GetVertexIndexByPos(vertexPos)] = new Vertex(vertexPos, vertex.GetOffset());
        }
    }

    private bool TryDeleteVertexByPos(Vector3 vertexPos)
    {
        if (!CheckVoxelsAround(vertexPos))
        {
            int index = _chunk.GetVertexIndexByPos(vertexPos);
            _chunk.Vertices[index] = null;
           CreatedVertexIndices.Remove(index);

            return true;
        }

        return false;
    }

    private bool CheckVoxelsAround(Vector3 vertexPos)
    {
        return
            _chunk.GetVoxelByPos((vertexPos + new Vector3(-0.5f, -0.5f, -0.5f)).ToVector3Int()) != null ||
            _chunk.GetVoxelByPos((vertexPos + new Vector3(+0.5f, -0.5f, -0.5f)).ToVector3Int()) != null ||
            _chunk.GetVoxelByPos((vertexPos + new Vector3(+0.5f, +0.5f, -0.5f)).ToVector3Int()) != null ||
            _chunk.GetVoxelByPos((vertexPos + new Vector3(-0.5f, +0.5f, -0.5f)).ToVector3Int()) != null ||

            _chunk.GetVoxelByPos((vertexPos + new Vector3(-0.5f, -0.5f, +0.5f)).ToVector3Int()) != null ||
            _chunk.GetVoxelByPos((vertexPos + new Vector3(+0.5f, -0.5f, +0.5f)).ToVector3Int()) != null ||
            _chunk.GetVoxelByPos((vertexPos + new Vector3(+0.5f, +0.5f, +0.5f)).ToVector3Int()) != null ||
            _chunk.GetVoxelByPos((vertexPos + new Vector3(-0.5f, +0.5f, +0.5f)).ToVector3Int()) != null;
    }
}
