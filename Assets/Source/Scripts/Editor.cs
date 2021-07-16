using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor : ChunkEmployee
{
    public int VertexCount { get; private set; }

    public Editor(Chunk chunk) : base(chunk)
    {
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

    public void PasteVertices(Vertex[] vertices, Vector3Int voxelPos)
    {
        TryPasteVertex(vertices[0], voxelPos + new Vector3(-0.5f, -0.5f, -0.5f));
        TryPasteVertex(vertices[1], voxelPos + new Vector3(+0.5f, -0.5f, -0.5f));
        TryPasteVertex(vertices[2], voxelPos + new Vector3(+0.5f, +0.5f, -0.5f));
        TryPasteVertex(vertices[3], voxelPos + new Vector3(-0.5f, +0.5f, -0.5f));

        TryPasteVertex(vertices[4], voxelPos + new Vector3(-0.5f, -0.5f, +0.5f));
        TryPasteVertex(vertices[5], voxelPos + new Vector3(+0.5f, -0.5f, +0.5f));
        TryPasteVertex(vertices[6], voxelPos + new Vector3(+0.5f, +0.5f, +0.5f));
        TryPasteVertex(vertices[7], voxelPos + new Vector3(-0.5f, +0.5f, +0.5f));
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

    public Vertex[] CopyVertices(Vector3Int voxelPos)
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

    public bool TryOffsetVertex(Vector3 startPos, ref Vector3 offset)
    {
        if (offset == Vector3.zero) return false;

        VertexPoint vertexPoint = _chunk.GetVertexPointByPos(startPos);

        if (vertexPoint == null || _chunk.GetVertexPointIndexByPos(startPos + offset) < 0) return false;

        Vertex vertex = _chunk.GetVertex(vertexPoint.GetLastVertexIndex());
        Vector3Bool vertexLimits = CheckVertexOffsetLimits(vertex.GetOffset() + offset);

        if (!vertexLimits.IsTrue) return false;

        if (!vertexLimits.X) offset.x = 0;
        if (!vertexLimits.Y) offset.y = 0;
        if (!vertexLimits.Z) offset.z = 0;

        if (offset == Vector3.zero) return false;

        vertex.Offset(offset);
        AddVertexIndexToVertexPoint(vertexPoint.GetLastVertexIndex(), vertex.Position);

        RemoveVertexIndexFromVertexPoint(vertexPoint.GetLastVertexIndex(), vertexPoint.Position);

        return true;



        //Vertex vertex = _chunk.GetVertexByPos(startPos);

        if(vertex != null)
        {
            Vector3 newPos = vertex.Position + offset;
            int index = _chunk.GetVertexIndexByPos(newPos);

            if (index > 0)
            {
                if (_chunk.GetVertexPoint(index) == null)
                    _chunk.VertexPoints[index] = new VertexPoint(newPos);
                else
                    vertex.SetPosition(newPos);

                return true;
            }
        }

        return false;
    }

    public EditorData GetData()
    {
        return new EditorData(VertexCount);
    }

    public void SetData(EditorData editorData)
    {
        VertexCount = editorData.VertexCount;
    }


    private bool TryCreateVertexByPos(Vector3 vertexPos)
    {
        int index = _chunk.GetVertexIndexByPos(vertexPos);

        if (index == -1 || _chunk.Vertices[index] != null) return false;

        _chunk.Vertices[index] = new Vertex(vertexPos);
        VertexCount++;

        //creating VertexPoint
        AddVertexIndexToVertexPoint(index, vertexPos);
        return true;
    }

    private bool TryPasteVertex(Vertex vertex, Vector3 vertexPos)
    {
        if(_chunk.GetVertexByPos(vertexPos) == null)
        {
            Vector3 vertexOffset = vertex.GetOffset();
            if (!_chunk.SetInsideChunk(vertexPos + vertexOffset)) vertexOffset = Vector3.zero;

            if(!_chunk.SetInsideChunk(vertex.Position)) MonoBehaviour.print(true);

            int index = _chunk.GetVertexIndexByPos(vertexPos);
            _chunk.Vertices[index] = new Vertex(vertexPos, vertexOffset);
            VertexCount++;

            AddVertexIndexToVertexPoint(index, vertexPos + vertexOffset);
            return true;
        }

        return false;
    }

    private bool TryDeleteVertexByPos(Vector3 vertexPos)
    {
        if (!CheckVoxelsAround(vertexPos))
        {
            int index = _chunk.GetVertexIndexByPos(vertexPos);
            RemoveVertexIndexFromVertexPoint(index, _chunk.Vertices[index].Position);
            _chunk.Vertices[index] = null;
            VertexCount--;

            return true;
        }

        return false;
    }

    private bool TryCreateVertexPoint(Vector3 vertexPointPos)
    {
        int index = _chunk.GetVertexPointIndexByPos(vertexPointPos);

        if (index >= 0 && _chunk.GetVertexPoint(index) == null)
        {
            _chunk.VertexPoints[index] = new VertexPoint(vertexPointPos);
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

    private void AddVertexIndexToVertexPoint(int vertexIndex, Vector3 vertexPointPos)
    {
        int vertexPointIndex = _chunk.GetVertexPointIndexByPos(vertexPointPos);

        if (vertexPointIndex > 0)
        {
            if (_chunk.GetVertexPoint(vertexPointIndex) == null)
                CreateVertexPoint(vertexPointPos);

            _chunk.VertexPoints[vertexPointIndex].AddVertexIndex(vertexIndex);
        }
    }

    private void AddVertexIndexToVertexPoint(Vector3 vertexPos, Vector3 vertexPointPos)
    {
        AddVertexIndexToVertexPoint(_chunk.GetVertexIndexByPos(vertexPos), vertexPointPos);
    }

    private void RemoveVertexIndexFromVertexPoint(int vertexIndex, Vector3 vertexPointPos)
    {
        int index = _chunk.GetVertexPointIndexByPos(vertexPointPos);

        _chunk.VertexPoints[index].RemoveVertexIndex(vertexIndex);
        if (_chunk.VertexPoints[index].IsEmpty())
            _chunk.VertexPoints[index] = null;
    }

    private void CreateVertexPoint(Vector3 vertexPos)
    {
        _chunk.VertexPoints[_chunk.GetVertexPointIndexByPos(vertexPos)] = new VertexPoint(vertexPos);
    }

    private Vector3Bool CheckVertexOffsetLimits(Vector3 vertexOffset)
    {
        return new Vector3Bool(
            vertexOffset.x >= -1.5f && vertexOffset.x <= 1.5f,
            vertexOffset.y >= -1.5f && vertexOffset.y <= 1.5f,
            vertexOffset.z >= -1.5f && vertexOffset.z <= 1.5f
            );
    }

}
