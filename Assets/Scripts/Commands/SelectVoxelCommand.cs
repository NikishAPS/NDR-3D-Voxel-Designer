using UnityEngine;

public class SelectVoxelCommand : Command
{
    private Vector3Int _globalVoxelPos;

    public void SetAndExe(Vector3Int globalVoxelPosition)
    {
        _globalVoxelPos = globalVoxelPosition;
        Invoker.Execute(this);
    }

    public override void Execute()
    {
        ChunkManager.SelectVoxel(_globalVoxelPos);
        Project.Saved = false;
    }
}
