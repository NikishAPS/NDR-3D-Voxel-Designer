using System;
using UnityEngine;

[Serializable]
public class VoxelData
{
    public int Id;
    public Vector3Int Position;
    public bool[] Faces;
    public int FaceCount;

    public VoxelData(int id, Vector3Int position, bool[] faces, int faceCount)
    {
        Id = id;
        Position = position;
        Faces = faces;
        FaceCount = faceCount;
    }

    public VoxelData(Voxel voxel)
    {
        Id = voxel.Id;
        Position = voxel.Position;
        Faces = voxel.Faces;
        FaceCount = voxel.FaceCount;
    }
}
