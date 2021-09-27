using System;
using UnityEngine;

[Serializable]
public class ChunkManagerData
{
    public int IncrementOption;
    public Vector3Int FieldSize;
    public Vector3Int ChunkSizes;
    public VertexData[] VerticesData;
    public ChunkData[] ChunksData;

    public ChunkManagerData(
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
