using System;
using UnityEngine;

[Serializable]
public class ChunksManagerData
{
    public int IncrementOption;
    public Vector3Int FieldSize;
    public Vector3Int ChunkSizes;
    public VertexData[] VerticesData;
    public ChunkData[] ChunksData;

    public ChunksManagerData(
        int incrementOption,
        Vector3Int fieldSize,
        Vector3Int chunkSizes,
        VertexData[] verticesData,
        ChunkData[] chunksData
        )
    {
        IncrementOption = incrementOption;
        FieldSize = fieldSize;
        ChunkSizes = chunkSizes;
        VerticesData = verticesData;
        ChunksData = chunksData;
    }
}
