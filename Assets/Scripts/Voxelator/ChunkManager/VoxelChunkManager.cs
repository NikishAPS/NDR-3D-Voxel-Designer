using System.Collections.Generic;
using UnityEngine;

public class VoxelChunkManager : ChunkManager<VoxelChunk, VoxelUnit>
{
    public bool GridVoxelActivity { set { foreach (VoxelChunk chunk in Chunks) chunk.GridVoxelActive = value; } }
    public bool GridSelectedVoxelActivity { set { foreach (VoxelChunk chunk in Chunks) chunk.GridSelectedVoxelActive = value; } }

    private Material _gridVoxelMaterial;
    private Material _gridSelectedVoxelMaterial;

    public VoxelChunkManager(Vector3Int fieldSize, Vector3Int chunkSize) : base(fieldSize, chunkSize)
    {
        VoxelChunk.VertexChunkManager = new VertexChunkManager(fieldSize + Vector3Int.one, chunkSize);
    }

    public void CreateVoxel(Vector3Int position, int id)
    {
        VoxelChunk chunk = GetChunk(position);
        if (chunk == null) return;

        VoxelUnit voxel = chunk.GetUnit(position);
        if (voxel != null) return;

        voxel = new VoxelUnit(position, id);
        chunk.SetUnit(voxel);

        UpdateVoxel(voxel);
        UpdateVoxelsAround(position);
    }

    public void DeleteVoxel(Vector3Int position)
    {
        VoxelChunk chunk = GetChunk(position);
        if (chunk == null) return;

        chunk.DeleteUnit(position);
        AddChunkToUpdate(chunk, true, true);

        UpdateVoxelsAround(position);

        DeleteVertices(position);
    }

    public void SelectVoxel(Vector3Int voxelPosition, bool select)
    {
        VoxelChunk chunk = GetChunk(voxelPosition);
        if (chunk == null) return;

        VoxelUnit voxel = chunk.GetUnit(voxelPosition);
        if (voxel == null || voxel.IsSelected == select) return;

        chunk.SelectedFaceCount += select ? voxel.FaceCount : -voxel.FaceCount;

        voxel.Select(select);

        AddChunkToUpdate(chunk, false, true);

        //calculation MiddleSelectedPosition
        //int prevSelectedVoxelCount = VoxelUnit.SelectedCount + (select ? -1 : 1); 
        //MiddleSelectedPosition = MiddleSelectedPosition * prevSelectedVoxelCount + voxelPosition;
        //MiddleSelectedPosition /= VoxelUnit.SelectedCount;
    }

    public void DeleteSelectedVoxels()
    {
        VoxelUnit cur = VoxelUnit.SelectedHead;
        while(cur != null)
        {
            Vector3Int position = cur.Position;
            cur = cur.SelectedPrev;
            DeleteVoxel(position);
        }
    }

    public void DragSelectedVoxels(Vector3 dragValue, out Vector3 dragResult)
    {
        dragResult = Vector3.zero;

        if (VoxelUnit.SelectedCount == 0) return;

        Vector3Int roundedOffset = dragValue.RoundToInt();
        if (roundedOffset == Vector3Int.zero) return;

        //checking limits
        VoxelUnit selectedVoxelIterator = VoxelUnit.SelectedHead;
        while (selectedVoxelIterator != null)
        {
            if (!InsideField(selectedVoxelIterator.Position + roundedOffset) ||
                GetUnit(selectedVoxelIterator.Position + roundedOffset) != null &&
                !GetUnit(selectedVoxelIterator.Position + roundedOffset).IsSelected) return;

            selectedVoxelIterator = selectedVoxelIterator.SelectedPrev;
        }

        //copying voxels
        VoxelUnit[] voxels = new VoxelUnit[VoxelUnit.SelectedCount];
        VertexUnit[] vertices = new VertexUnit[voxels.Length * 8];
        {
            int i = 0;
            selectedVoxelIterator = VoxelUnit.SelectedHead;
            while (selectedVoxelIterator != null)
            {
                voxels[i] = selectedVoxelIterator;
                selectedVoxelIterator = selectedVoxelIterator.SelectedPrev;
                i++;
            }
        }

        //deleting voxels
        DeleteSelectedVoxels();

        //creating voxels
        for (int i = 0; i < voxels.Length; i++)
        {
            CreateVoxel(voxels[i].Position + roundedOffset, voxels[i].Id);
            VoxelChunk.VertexChunkManager.CreateVertices(voxels[i].Position + roundedOffset);
            SelectVoxel(voxels[i].Position + roundedOffset, true);
        }

        //MiddleSelectedPosition += roundedOffset;
        //dragValue.Position = roundedOffset;

        dragResult = roundedOffset;
    }

    //public void MoveSelectedVoxels(Vector3 dragValue)
    //{
    //    if (VoxelUnit.SelectedCount == 0) return;

    //    Vector3Int roundedOffset = dragValue.Position.RoundToInt();
    //    if (roundedOffset == Vector3Int.zero) return;

    //    //checking limits
    //    VoxelUnit curVoxel = VoxelUnit.SelectedHead;
    //    while (curVoxel != null)
    //    {
    //        if (!InsideField(curVoxel.Position + roundedOffset) ||
    //            GetUnit(curVoxel.Position + roundedOffset) != null &&
    //            !GetUnit(curVoxel.Position + roundedOffset).IsSelected) return;

    //        curVoxel = curVoxel.Prev;
    //    }

    //    //copying voxels
    //    VoxelUnit[] voxels = new VoxelUnit[VoxelUnit.SelectedCount];
    //    VertexUnit[] vertices = new VertexUnit[voxels.Length * 8];
    //    {
    //        int i = 0;
    //        curVoxel = VoxelUnit.SelectedHead;
    //        while(curVoxel != null)
    //        {
    //            voxels[i] = curVoxel;
    //            curVoxel = curVoxel.SelectedPrev;
    //            i++;
    //        }
    //    }

    //    //deleting voxels
    //    DeleteSelectedVoxels();

    //    //creating voxels
    //    for (int i = 0; i < voxels.Length; i++)
    //    {
    //        CreateVoxel(voxels[i].Position + roundedOffset, voxels[i].Id);
    //        VoxelChunk.VertexChunkManager.CreateVertices(voxels[i].Position + roundedOffset);
    //    }

    //    //MiddleSelectedPosition += roundedOffset;
    //    dragValue.Position = roundedOffset;
    //}

    public void ResetSelection()
    {
        VoxelUnit voxelIterator = VoxelUnit.Head;
        while (voxelIterator != null)
        {
            SelectVoxel(voxelIterator.Position, false);

            voxelIterator = voxelIterator.Prev;
        }
    }

    protected override void OnLoadResources()
    {
        _chunkMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/Chunk");
        _gridVoxelMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/GridVoxel");
        _gridSelectedVoxelMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/GridSelectedVoxel");
    }

    protected override void OnInitChunk(int index, Vector3Int position, Vector3Int size)
    {
        Chunks[index] = new VoxelChunk(position, size, _chunkMaterial, "VoxelChunk", _gridVoxelMaterial, _gridSelectedVoxelMaterial);
    }

    private void UpdateVoxel(VoxelUnit voxel)
    {
        if (voxel == null) return;

        byte facesFlags = 0;
        for (int i = 0; i < Direction.Directions.Length; i++)
            if (GetUnit(voxel.Position + Direction.Directions[i]) == null)
                facesFlags |= Direction.Masks[i];

        if (voxel.FacesFlags != facesFlags)
        {
            VoxelChunk chunk = GetChunk(voxel.Position);
            chunk.FaceCount -= voxel.FaceCount;
            voxel.FacesFlags = facesFlags;
            chunk.FaceCount += voxel.FaceCount;

            AddChunkToUpdate(chunk, true, true);
        }

    }

    private void UpdateVoxelsAround(Vector3Int voxelPosition)
    {
        foreach (Vector3Int direction in Direction.Directions)
            UpdateVoxel(GetUnit(voxelPosition + direction));
    }

    private void DeleteVertex(Vector3Int vertexPosition)
    {
        foreach (Vector3Int voxelPosition in GetSurroundingPositions(vertexPosition - Vector3Int.one))
            if (GetUnit(voxelPosition) != null) return;

        VoxelChunk.VertexChunkManager.DeleteVertex(vertexPosition);
    }

    private void DeleteVertices(Vector3Int voxelPosition)
    {
        foreach (Vector3Int vertexPosition in GetSurroundingPositions(voxelPosition))
            DeleteVertex(vertexPosition);
    }

    private void AddChunkToUpdate(VoxelChunk chunk, bool updateVoxelMeshFlag, bool updateGridSelectedVoxelMeshFlag)
    {
        chunk.UpdateVoxelMeshFlag = updateVoxelMeshFlag;
        chunk.UpdateGridSelectedVoxelMeshFlag = updateGridSelectedVoxelMeshFlag;

        AddChunkToUpdate(chunk);
    }

}
