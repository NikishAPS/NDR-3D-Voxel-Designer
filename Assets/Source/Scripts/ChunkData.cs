using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChunkData
{
    public int IncrementOption;
    public Vector3Int Size;
    public VoxelData[] VoxelsData;
    public VertexData[] VerticesData;
    public BuilderData BuilderData;
    public EditorData EditorData;

    public ChunkData(
        int incrementOption,
        Vector3Int size,
        VoxelData[] voxelsData,
        VertexData[] verticesData,
        BuilderData builderData,
        EditorData editorData
        )
    {
        IncrementOption = incrementOption;
        Size = size;
        VoxelsData = voxelsData;
        VerticesData = verticesData;
        BuilderData = builderData;
        EditorData = editorData;
    }
}
