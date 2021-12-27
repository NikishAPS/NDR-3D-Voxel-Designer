using System;
using UnityEngine;

public class VertexChunkManager : ChunkManager<VertexChunk, VertexUnit>
{
    public bool VerticesActive { set { foreach (VertexChunk chunk in Chunks) chunk.VerticesActive = value; } }

    public VertexUnit SelectedVertex { get; private set; }
    public Action<VertexUnit> EditSelectedVertexEvent;

    public VertexChunkManager(Vector3Int fieldSize, Vector3Int chunkSize) : base(fieldSize, chunkSize)
    {
        VerticesActive = false;
    }

    public void CreateVertices(Vector3Int voxelPosition)
    {
        foreach (Vector3Int position in GetSurroundingPositions(voxelPosition))
            CreateVertex(position);
    }

    public void DeleteVertex(Vector3Int vertexPosition)
    {
        VertexChunk chunk = GetChunk(vertexPosition);
        chunk.DeleteUnit(vertexPosition);
        AddChunkToUpdate(chunk);
    }

    public void SetVertexOffset(Vector3Int vertexPosition, Vector3 vertexOffset)
    {
        VertexChunk chunk = GetChunk(vertexPosition);
        if (chunk == null) return;

        VertexUnit vertex = chunk.GetUnit(vertexPosition);
        if (vertex == null) return;

        vertex.SetOffset(vertexOffset);

        AddChunkToUpdate(chunk);
    }

    public void SelectVertex(Vector3Int vertexPosition)
    {
        if (SelectedVertex != null)
            SelectedVertex.OffsetPosition.RemoveAction(OnEditSelectedVertex);

        SelectedVertex = GetUnit(vertexPosition);

        if (SelectedVertex != null)
        {
            SelectedVertex.OffsetPosition.BindAction(OnEditSelectedVertex);
            OnEditSelectedVertex(SelectedVertex.OffsetPosition.Value);
        }
    }

    protected override void OnLoadResources()
    {
        _chunkMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/Vertex");
    }

    protected override void OnInitChunk(int index, Vector3Int position, Vector3Int size)
    {
        Chunks[index] = new VertexChunk(position, size, _chunkMaterial, "VertexChunk");
    }

    private void CreateVertex(Vector3Int vertexPosition)
    {
        VertexChunk chunk = GetChunk(vertexPosition);
        if (chunk == null) return;

        VertexUnit vertex = chunk.GetUnit(vertexPosition);
        if (vertex != null) return;

        vertex = new VertexUnit(vertexPosition);
        chunk.SetUnit(vertex);

        AddChunkToUpdate(chunk);
    }

    private void OnEditSelectedVertex(Vector3 vertexOffsetPosition)
    {
        EditSelectedVertexEvent?.Invoke(SelectedVertex);
    }
}
