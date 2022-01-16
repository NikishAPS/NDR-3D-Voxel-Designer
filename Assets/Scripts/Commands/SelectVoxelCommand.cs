using UnityEngine;

public class SelectVoxelCommand : ICommand
{
    private Vector3Int _globalVoxelPosition;

    public SelectVoxelCommand(Vector3Int globalVoxelPosition)
    {
        _globalVoxelPosition = globalVoxelPosition;
    }

    public void Execute()
    {
        Voxelator.SelectVoxel(_globalVoxelPosition, true);
    }

    public void Undo()
    {

    }
}
