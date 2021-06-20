using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChunkData
{
    public ChunkData(Vector3Int size, List<int> buildedIndices,
        List<int> verticesIndices, Vector3[] offsetsVertices, int faceCount, VoxelData[] voxelData)
    {
        _size = size;

        _buildedIndices = buildedIndices;
        _verticesIndices = verticesIndices;
        _offsetsVertices = offsetsVertices;
        _faceCount = faceCount;


        _voxelData = voxelData;
    }

    [SerializeField]
    private Vector3Int _size;
    [SerializeField]
    private List<int> _buildedIndices;
    [SerializeField]
    private List<int> _verticesIndices;
    [SerializeField]
    private Vector3[] _offsetsVertices;
    [SerializeField]
    private int _faceCount;
    [SerializeField]
    private VoxelData[] _voxelData;

    public Vector3Int Size => _size;
    public List<int> BuildedIndices => _buildedIndices;
    public List<int> VerticesIndices => _verticesIndices;
    public Vector3[] OffsetsVertices => _offsetsVertices;
    public int FaceCount => _faceCount;
    public VoxelData[] VoxelData => _voxelData;
}
