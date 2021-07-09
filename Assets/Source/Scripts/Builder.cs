﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : ChunkEmployee
{
    public int FaceCount { get; private set; }
    public readonly List<int> BuildedVoxelIndices;

    public Builder(Chunk chunk) : base(chunk)
    {
        BuildedVoxelIndices = new List<int>();
    }

    public void CreateVoxel(int id, Vector3Int pos)
    {
        if (_chunk.GetVoxelByPos(pos) != null) return;

        int index = _chunk.GetIndexByPos(pos);
        Voxel newVoxel = new Voxel(id, pos);
        BuildedVoxelIndices.Add(index);
        _chunk.Voxels[index] = newVoxel;

        //creating faces
        UpdateVoxel(newVoxel);

        //updating adjacent Voxels
        UpdateVoxelsAround(pos);
    }

    public void UpdateVoxelsAround(Vector3Int pos)
    {
        UpdateVoxel(_chunk.GetVoxelByPos(pos - Vector3Int.left));
        UpdateVoxel(_chunk.GetVoxelByPos(pos - Vector3Int.right));
        UpdateVoxel(_chunk.GetVoxelByPos(pos - Vector3Int.down));
        UpdateVoxel(_chunk.GetVoxelByPos(pos - Vector3Int.up));
        UpdateVoxel(_chunk.GetVoxelByPos(pos - new Vector3Int().Back()));
        UpdateVoxel(_chunk.GetVoxelByPos(pos - new Vector3Int().Forward()));
    }

    public void UpdateAllVoxels()
    {
        for (int i = 0; i < BuildedVoxelIndices.Count; i++)
        {
            UpdateVoxel(GetVoxelByBuildedIndex(i));
        }
    }

    public void UpdateVoxel(Voxel voxel)
    {
        if (voxel == null) return;

        FaceCount -= voxel.FaceCount;

        if (_chunk.GetVoxelByPos(voxel.Position + Vector3Int.left) == null) voxel.SetLeftFace(true);
        else voxel.SetLeftFace(false);

        if (_chunk.GetVoxelByPos(voxel.Position + Vector3Int.right) == null) voxel.SetRightFace(true);
        else voxel.SetRightFace(false);

        if (_chunk.GetVoxelByPos(voxel.Position + Vector3Int.down) == null) voxel.SetBottomFace(true);
        else voxel.SetBottomFace(false);

        if (_chunk.GetVoxelByPos(voxel.Position + Vector3Int.up) == null) voxel.SetTopFace(true);
        else voxel.SetTopFace(false);

        if (_chunk.GetVoxelByPos(voxel.Position + new Vector3Int().Back()) == null) voxel.SetRearFace(true);
        else voxel.SetRearFace(false);

        if (_chunk.GetVoxelByPos(voxel.Position + new Vector3Int().Forward()) == null) voxel.SetFrontFace(true);
        else voxel.SetFrontFace(false);

        FaceCount += voxel.FaceCount;
    }

    public void OffsetBuildedIndices(Vector3Int offset)
    {
        for (int i = 0; i < BuildedVoxelIndices.Count; i++)
        {
            if(GetVoxelByBuildedIndex(i) == null)
            {
                BuildedVoxelIndices[i] = _chunk.GetIndexByPos(_chunk.GetPosByIndex(BuildedVoxelIndices[i]) + offset);
            }
        }
    }

    public void DeleteVoxel(int voxelIndex)
    {
        if (_chunk.Voxels[voxelIndex] == null) return;

        FaceCount -= _chunk.Voxels[voxelIndex].FaceCount;

        Vector3Int pos = _chunk.Voxels[voxelIndex].Position;

        _chunk.Voxels[voxelIndex] = null;
        BuildedVoxelIndices.Remove(voxelIndex);

        UpdateVoxelsAround(pos);
    }

    public Voxel GetVoxelByBuildedIndex(int buildedIndex)
    {
        return _chunk.Voxels[BuildedVoxelIndices[buildedIndex]];
    }
}
