using UnityEngine;

public class VertexChunkManager : IChunkManager<VertexChunk, VertexUnit>
{
    public VertexChunkManager(Vector3Int fieldSize, Vector3Int chunkSize) : base(fieldSize, chunkSize)
    {

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

    public void MoveVertex(Vector3Int vertexPosition, Vector3 vertexOffset)
    {
        VertexChunk chunk = GetChunk(vertexPosition);
        if (chunk == null) return;

        VertexUnit vertex = chunk.GetUnit(vertexPosition);
        if (vertex == null) return;

        vertex.Offset = vertexOffset;

        AddChunkToUpdate(chunk);
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
}
