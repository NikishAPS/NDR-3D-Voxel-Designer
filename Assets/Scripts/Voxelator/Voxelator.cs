using UnityEngine;

public static class Voxelator
{
    public static VoxelChunkManager VoxelChunkManager { get; private set; }
    public static VertexChunkManager VertexChunkManager => VoxelChunk.VertexChunkManager;
    public static Vector3Int FieldSize { get; private set; }
    public static int IncrementOption { get; private set; }
    public static Vector3Byte VoxelColor
    {
        get => _voxelColor;
        set => _voxelColor = value;
    }

    public static Vector3Int Center => FieldSize / 2;

    public static readonly int[] IncrementOptionTypes = new int[4] { 8, 16, 32, 86 };
    public enum IncrementOptionType { T8 = 8, T16 = 16, T32 = 32, T86 = 64 }

    private static Vector3Byte _voxelColor;
    private static Vector3 _vertexOffsetLimit = new Vector3(1.5f, 1.5f, 1.5f);

    public static void Init(Vector3Int fieldSize, IncrementOptionType incrementOptionType)
    {
        int incrementOption = (int)IncrementOptionType.T8;

        switch (incrementOptionType)
        {
            case IncrementOptionType.T8: incrementOption = 8; break;
            case IncrementOptionType.T16: incrementOption = 16; break;
            case IncrementOptionType.T32: incrementOption = 32; break;
            case IncrementOptionType.T86: incrementOption = 86; break;
        }

        FieldSize = fieldSize;
        IncrementOption = incrementOption;

        VoxelChunkManager = new VoxelChunkManager(fieldSize, SceneParameters.VoxelChunkSize);
    }

    public static void Release()
    {
        VoxelChunkManager?.Release();
        VertexChunkManager?.Release();
    }
    public static VoxelUnit GetVoxel(Vector3Int voxelPosition) => VoxelChunkManager.GetUnit(voxelPosition);
    public static VertexUnit GetVertex(Vector3Int vertexPosition) => VertexChunkManager.GetUnit(vertexPosition);

    public static bool InsideField(Vector3Int position) => VoxelatorArray.WithinTheArray(FieldSize, position);

    public static void CreateVoxel(Vector3Int voxelPosition, Vector3Byte color)
    {
        VoxelChunkManager.CreateVoxel(voxelPosition, color);
        VoxelChunk.VertexChunkManager.CreateVertices(voxelPosition);
    }

    public static void CreateVoxel(Vector3Int voxelPosition)
    {
        CreateVoxel(voxelPosition, _voxelColor);
    }

    public static void CreateVoxel(int zIndex, Vector3Byte color)
    {
        CreateVoxel(VoxelatorArray.GetPosition(FieldSize, zIndex), color);
    }

    public static void DeleteVoxel(Vector3Int voxelPosition)
    {
        VoxelChunkManager.DeleteVoxel(voxelPosition);
    }

    public static void SelectVoxel(Vector3Int voxelPosition, bool select)
    {
        VoxelChunkManager.SelectVoxel(voxelPosition, select);
    }

    public static void DragSelectedVoxels(Vector3 dragValue, out Vector3 dragResult)
    {
        VoxelChunkManager.DragSelectedVoxels(dragValue, out dragResult);
    }

    public static void DeleteSelectedVoxels()
    {
        VoxelChunkManager.DeleteSelectedVoxels();
    }

    public static void DeleteAllVoxels()
    {
        VoxelUnit iteratorVoxel = VoxelUnit.Head;
        while(iteratorVoxel != null)
        {
            Vector3Int voxelPosition = iteratorVoxel.Position;
            iteratorVoxel = iteratorVoxel.Prev;
            DeleteVoxel(voxelPosition);
        }
    }

    public static void ResetVoxelSelection()
    {
        VoxelChunkManager.ResetSelection();
    }

    public static void SelectVertex(Vector3Int vertexPosition)
    {
        VertexChunkManager.SelectVertex(vertexPosition);
    }

    public static void SetVertexOffset(Vector3Int vertexPosition, Vector3 vertexOffset)
    {
        ClampVertexOffset(ref vertexOffset);

        VertexChunkManager.SetVertexOffset(vertexPosition, vertexOffset);
        VoxelChunkManager.AddSurroundingChunksToUpdate(vertexPosition);
    }

    public static void OffsetSelectedVertex(Vector3 offset)
    {
        if (VertexChunkManager.SelectedVertex == null) return;

        VertexChunkManager.SelectedVertex.Offset(offset);
        VoxelChunkManager.AddSurroundingChunksToUpdate(VertexChunkManager.SelectedVertex.Position);
        VertexChunkManager.AddSurroundingChunksToUpdate(VertexChunkManager.SelectedVertex.Position);
    }

    public static void ShiftSelectedVertexByStep(Vector3Int stepValue)
    {
        if (VertexChunkManager.SelectedVertex == null) return;

        Vector3 vertexOffset = stepValue.ToVector3() / IncrementOption + VertexChunkManager.SelectedVertex.GetOffset();

        SetVertexOffset(VertexChunkManager.SelectedVertex.Position, vertexOffset);
    }

    public static void DragSelectedVertex(Vector3 dragValue, out Vector3 dragResult)
    {
        dragResult = Vector3.zero;

        ClampVertexOffset(ref dragValue);

        Vector3Bool checkingResult = CheckVertexOffsetLimit(VertexChunkManager.SelectedVertex.GetOffset() + dragValue);
        if (checkingResult.IsTrue)
        {
            if (!checkingResult.X) dragValue.x = 0;
            if (!checkingResult.Y) dragValue.y = 0;
            if (!checkingResult.Z) dragValue.z = 0;

            OffsetSelectedVertex(dragValue);
            dragResult = dragValue;
        }
    }

    public static void UpdateChunks()
    {
        VoxelChunkManager.UpdateChunks();
        VertexChunkManager.UpdateChunks();
    }

    public static VertexUnit GetUnit(Vector3Int vertexPosition)
    {
        return VoxelChunk.VertexChunkManager.GetUnit(vertexPosition);
    }

    private static void ClampVertexOffset(ref Vector3 vertexOffset)
    {
        vertexOffset = vertexOffset.Clamp(-_vertexOffsetLimit, _vertexOffsetLimit);

        vertexOffset *= IncrementOption;
        vertexOffset = vertexOffset.RoundToFloat();
        vertexOffset /= IncrementOption;
    }

    private static Vector3Bool CheckVertexOffsetLimit(Vector3 vertexOffset)
    {
        return new Vector3Bool(
            vertexOffset.x >= -_vertexOffsetLimit.x && vertexOffset.x <= _vertexOffsetLimit.x,
            vertexOffset.y >= -_vertexOffsetLimit.y && vertexOffset.y <= _vertexOffsetLimit.y,
            vertexOffset.z >= -_vertexOffsetLimit.z && vertexOffset.z <= _vertexOffsetLimit.z);
    }

}
