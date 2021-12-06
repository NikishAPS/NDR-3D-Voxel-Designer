using UnityEngine;

public static class Voxelator
{
    public static VoxelChunkManager VoxelChunkManager { get; set; }

    public static void CreateVoxel(Vector3Int voxelPosition, int id)
    {
        VoxelChunkManager.CreateVoxel(voxelPosition, id);
        VoxelChunk.VertexChunkManager.CreateVertices(voxelPosition);
    }

    public static void DeleteVoxel(Vector3Int voxelPosition)
    {
        VoxelChunkManager.DeleteVoxel(voxelPosition);
    }
    public static void DeleteSelectedVoxels()
    {
        VoxelChunkManager.DeleteSelectedVoxels();
    }

    public static void MoveVertex(Vector3Int vertexPosition, Vector3 vertexOffset)
    {
        VoxelChunk.VertexChunkManager.MoveVertex(vertexPosition, vertexOffset);
        VoxelChunkManager.AddSurroundingChunksToUpdate(vertexPosition);
    }

    public static void SelectVoxel(Vector3Int voxelPosition, bool select)
    {
        VoxelChunkManager.SelectVoxel(voxelPosition, select);
    }

    public static void MoveSelectedVoxels(DragTransform dragValue)
    {
        VoxelChunkManager.MoveSelectedVoxels(dragValue);
    }

    public static void UpdateChunks()
    {
        VoxelChunkManager.UpdateChunks();
        VoxelChunk.VertexChunkManager.UpdateChunks();
    }

}
