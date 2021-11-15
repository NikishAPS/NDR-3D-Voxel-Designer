using UnityEngine;

public class SelectVoxelCommand : Command
{
    private Vector3Int _globalVoxelPosition;

    public SelectVoxelCommand(Vector3Int globalVoxelPosition)
    {
        _globalVoxelPosition = globalVoxelPosition;
    }

    public override void Execute()
    {
        ChunkManager.SelectVoxel(_globalVoxelPosition);
        Project.Saved = false;
    }
}
