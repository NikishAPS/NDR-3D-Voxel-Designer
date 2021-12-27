using UnityEngine;

public static class Voxelator
{
    public static VoxelChunkManager VoxelChunkManager { get; private set; }
    public static VertexChunkManager VertexChunkManager => VoxelChunk.VertexChunkManager;
    public static Vector3Int FieldSize { get; private set; }
    public static int IncrementOption { get; private set; }
    public static int VoxelId
    {
        get => _voxelId;
        set
        {
            if (value < 0 || value > Mathf.Pow(SceneParameters.TextureSize, 2))
                _voxelId = 0;
            else _voxelId = value;
        }
    }

    public static Vector3Int Center => FieldSize / 2;

    private static int _voxelId;
    private static Vector3 _vertexOffsetLimit = new Vector3(1.5f, 1.5f, 1.5f);

    public static void Init(Vector3Int fieldSize, int incrementOption)
    {
        FieldSize = fieldSize;
        IncrementOption = incrementOption;

        VoxelChunkManager = new VoxelChunkManager(fieldSize, SceneParameters.ChunkSize);
    }
    public static void Release()
    {

    }
    public static VoxelUnit GetVoxel(Vector3Int voxelPosition) => VoxelChunkManager.GetUnit(voxelPosition);
    public static VertexUnit GetVertex(Vector3Int vertexPosition) => VertexChunkManager.GetUnit(vertexPosition);

    public static bool InsideField(Vector3Int position) => VoxelatorArray.WithinTheArray(FieldSize, position);

    public static void SetVoxelIdByColor(Color color)
    {
        int r = (int)(color.r * 255);
        int g = (int)(color.g * 255);
        int b = (int)(color.b * 255);

        int index = VoxelatorArray.GetIndex(Vector3Int.one * 256, new Vector3Int(r, g, b)) + 1;

        _voxelId = index;
    }

    public static void CreateVoxel(Vector3Int voxelPosition)
    {
        CreateVoxel(voxelPosition, _voxelId);
    }

    public static void CreateVoxel(Vector3Int voxelPosition, int id)
    {
        VoxelChunkManager.CreateVoxel(voxelPosition, id);
        VoxelChunk.VertexChunkManager.CreateVertices(voxelPosition);
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
