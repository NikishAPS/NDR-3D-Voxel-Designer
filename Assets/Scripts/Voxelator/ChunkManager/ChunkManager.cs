using System.Collections.Generic;
using UnityEngine;

public abstract class ChunkManager<C, U> where C : Chunk<U> where U : Unit
{
    public readonly Vector3Int FieldSize;
    public readonly Vector3Int ChunkSize;
    public C[] Chunks { get; private set; }

    protected Vector3Int _chunksCount;
    protected Material _chunkMaterial;

    private LinkedList<C> _notUpdatedChunks;

    public ChunkManager(Vector3Int fieldSize, Vector3Int chunkSize)
    {
        FieldSize = fieldSize;
        ChunkSize = chunkSize;
        _notUpdatedChunks = new LinkedList<C>();
        OnLoadResources();
        InitChunks();
    }

    public C GetChunk(Vector3 position) =>
        InsideField(position.ToVector3Int()) ? Chunks[VoxelatorArray.GetIndex(_chunksCount, GetChunkPosition(position))] : default;

    public U GetUnit(Vector3Int position)
    {
        C chunk = GetChunk(position);
        if (chunk == null) return null;

        return chunk.GetUnit(position);
    }

    public void UpdateChunks()
    {
        foreach (C currentChunk in _notUpdatedChunks)
            currentChunk.UpdateMesh();

        _notUpdatedChunks.Clear();
    }

    public void AddSurroundingChunksToUpdate(Vector3Int voxelPosition)
    {
        foreach (Vector3Int position in GetSurroundingPositions(voxelPosition))
            AddChunkToUpdate(GetChunk(position));
    }

    protected void InitChunks()
    {
        _chunksCount = new Vector3Int(
             FieldSize.x % ChunkSize.x != 0 ? (FieldSize.x / ChunkSize.x) + 1 : FieldSize.x / ChunkSize.x,
             FieldSize.y % ChunkSize.y != 0 ? (FieldSize.y / ChunkSize.y) + 1 : FieldSize.y / ChunkSize.y,
             FieldSize.z % ChunkSize.z != 0 ? (FieldSize.z / ChunkSize.z) + 1 : FieldSize.z / ChunkSize.z
             );

        Chunks = new C[_chunksCount.x * _chunksCount.y * _chunksCount.z];

        for (int x = 0; x < _chunksCount.x; x++)
        {
            for (int y = 0; y < _chunksCount.y; y++)
            {
                for (int z = 0; z < _chunksCount.z; z++)
                {
                    Vector3Int chunkPosition = new Vector3Int(x, y, z);

                    int index = VoxelatorArray.GetIndex(_chunksCount, chunkPosition);

                    Vector3Int chunkSize = new Vector3Int(
                        x + 1 == _chunksCount.x ? FieldSize.x - x * ChunkSize.x : ChunkSize.x,
                        y + 1 == _chunksCount.y ? FieldSize.y - y * ChunkSize.y : ChunkSize.y,
                        z + 1 == _chunksCount.z ? FieldSize.z - z * ChunkSize.z : ChunkSize.z
                        );

                    OnInitChunk(index, chunkPosition * ChunkSize, chunkSize);
                }
            }
        }
    }
    protected bool InsideField(Vector3Int point) => VoxelatorArray.WithinTheArray(FieldSize, point);
    protected Vector3Int GetChunkPosition(Vector3 point) => point.Div(ChunkSize).ToVector3Int();
    protected void AddChunkToUpdate(C chunk)
    {
        if (chunk == null) return;

        foreach (C currentChunk in _notUpdatedChunks)
            if (currentChunk == chunk) return;

        _notUpdatedChunks.AddLast(chunk);
    }
    protected Vector3Int[] GetSurroundingPositions(Vector3Int position)
    {
        return new Vector3Int[]
        {
            //bottom
            position + Vector3Int.zero,
            position + Vector3Int.right,
            position + new Vector3Int().Forward(),
            position + Vector3Int.right + new Vector3Int().Forward(),

            //top
            position + Vector3Int.up + Vector3Int.zero,
            position + Vector3Int.up + Vector3Int.right,
            position + Vector3Int.up + new Vector3Int().Forward(),
            position + Vector3Int.up + Vector3Int.right + new Vector3Int().Forward(),

        };
    }

    protected abstract void OnLoadResources();
    protected abstract void OnInitChunk(int index, Vector3Int position, Vector3Int size);

}

