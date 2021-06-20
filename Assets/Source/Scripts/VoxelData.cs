using System;
using UnityEngine;

[Serializable]
public class VoxelData
{
    public VoxelData(int id, Vector3Int position, bool[] faces)
    {
        _id = id;
        _position = position;
        _faces = faces;
    }

    public VoxelData(Voxel voxel)
    {
        _id = voxel.Id;
        _position = voxel.Position;
        _faces = voxel.Faces;
    }

    [SerializeField]
    private int _id;
    [SerializeField]
    private Vector3Int _position;
    [SerializeField]
    private bool[] _faces;

    public int Id => _id;
    public Vector3Int Position => _position;
    public bool[] Faces => _faces;
}
