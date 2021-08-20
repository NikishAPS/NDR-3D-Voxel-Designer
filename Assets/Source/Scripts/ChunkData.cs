using System;
using UnityEngine;

[Serializable]
public class ChunkData
{
    public Vector3Int Position;
    public Vector3Int Size;
    public int FaceCount;
    public VoxelData[] VoxelsData;


    public ChunkData(
        Vector3Int position,
        Vector3Int size,
        int faceCount,
        VoxelData[] voxelsData
        )
    {
        Position = position;
        Size = size;
        FaceCount = faceCount;
        VoxelsData = voxelsData;
    }
}
