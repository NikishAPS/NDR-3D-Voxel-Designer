using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : ChunkEmployee
{
    public int FaceCount { get; private set; }
    public Vector3 MiddleSelectedPos { get; private set; }
    public readonly List<int> SelectedVoxelIndices;

    public Selector(Chunk chunk) : base(chunk)
    {
        SelectedVoxelIndices = new List<int>();
    }

    public void SelectVoxel(Vector3Int pos)
    {
        int index = _chunk.GetIndexByPos(pos);

        if (index == -1 || _chunk.Voxels[index] == null || _chunk.SelectedVoxels[index] != null) return;

        Voxel newSelectedVoxel = new Voxel(0, pos);
        _chunk.SelectedVoxels[index] = newSelectedVoxel;
        SelectedVoxelIndices.Add(index);

        UpdateVoxel(index);

        MiddleSelectedPos = (MiddleSelectedPos * (SelectedVoxelIndices.Count - 1) + pos) / SelectedVoxelIndices.Count;
    }

    public void RemoveVoxel(Vector3Int pos)
    {
        int index = _chunk.GetIndexByPos(pos);

        FaceCount -= _chunk.SelectedVoxels[index].FaceCount;
        _chunk.SelectedVoxels[index] = null;
        SelectedVoxelIndices.Remove(index);

        UpdateVoxelsAround(pos);
    }

    public void UpdateAllVoxels()
    {
        for (int i = 0; i < SelectedVoxelIndices.Count; i++)
        {
            UpdateVoxel(SelectedVoxelIndices[i]);
        }
    }

    public void UpdateVoxel(int voxelIndex)
    {
        if (_chunk.GetSelectedVoxel(voxelIndex) == null) return;

        FaceCount -= _chunk.SelectedVoxels[voxelIndex].FaceCount;
        _chunk.SelectedVoxels[voxelIndex] = new Voxel(_chunk.Voxels[voxelIndex]);
        FaceCount += _chunk.SelectedVoxels[voxelIndex].FaceCount;
    }

    public void UpdateVoxelsAround(Vector3Int pos)
    {
        UpdateVoxel(_chunk.GetIndexByPos(pos - Vector3Int.left));
        UpdateVoxel(_chunk.GetIndexByPos(pos - Vector3Int.right));
        UpdateVoxel(_chunk.GetIndexByPos(pos - Vector3Int.down));
        UpdateVoxel(_chunk.GetIndexByPos(pos - Vector3Int.up));
        UpdateVoxel(_chunk.GetIndexByPos(pos - new Vector3Int().Back()));
        UpdateVoxel(_chunk.GetIndexByPos(pos - new Vector3Int().Forward()));
    }

    public void Reset()
    {
        for(int i = 0; i < SelectedVoxelIndices.Count; i++)
        {
            _chunk.SelectedVoxels[SelectedVoxelIndices[i]] = null;
        }
        FaceCount = 0;
        MiddleSelectedPos = Vector3.zero;
        SelectedVoxelIndices.Clear();
    }

    public bool TryMoveVoxels(Vector3 capturePos, Vector3Int offset)
    {
        if (offset == Vector3.zero || !ItCanMove(offset)) return false;

        Voxel[] buildedVoxels = new Voxel[SelectedVoxelIndices.Count];
        Vertex[][] vertices = new Vertex[buildedVoxels.Length][];
        for (int i = 0; i < buildedVoxels.Length; i++)
        {
            buildedVoxels[i] = new Voxel(_chunk.Voxels[SelectedVoxelIndices[i]]);
            vertices[i] = _chunk.Editor.CopyVertices(buildedVoxels[i].Position);

            _chunk.Builder.DeleteVoxel(SelectedVoxelIndices[i]);
            _chunk.Editor.DeleteVerticesByPos(buildedVoxels[i].Position);
        }

        Reset();

        for(int i = 0; i < buildedVoxels.Length; i++)
        {
            Vector3Int newVoxelPosition = buildedVoxels[i].Position + offset;

            _chunk.Builder.TryCreateVoxel(buildedVoxels[i].Id, newVoxelPosition);
            _chunk.Editor.PastVertices(vertices[i], newVoxelPosition);
            SelectVoxel(newVoxelPosition);
        }

        return true;
    }

    public Voxel GetVoxelBySelectedIndex(int selectedIndex)
    {
        return _chunk.SelectedVoxels[SelectedVoxelIndices[selectedIndex]];
    }

    private bool ItCanMove(Vector3Int offsetInt)
    {
        for (int i = 0; i < SelectedVoxelIndices.Count; i++)
        {
            Vector3Int newVoxelPos = GetVoxelBySelectedIndex(i).Position + offsetInt;
            if (!_chunk.InChunk(newVoxelPos) ||
                _chunk.GetVoxelByPos(newVoxelPos) != null && _chunk.GetSelectedVoxelByPos(newVoxelPos) == null)
            {
                return false;
            }
        }

        return true;
    }
}
