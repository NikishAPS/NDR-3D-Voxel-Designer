using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateVoxelsCommand : Command
{
    private Vector3Int _startVoxelArea, _endVoxelArea;

    public void SetAndExe(Vector3Int startVoxelArea, Vector3Int endVoxelArea)
    {
        _startVoxelArea = startVoxelArea;
        _endVoxelArea = endVoxelArea;
        Invoker.Execute(this);
    }

    public override void Execute()
    {
        ChunksManager.CreateVoxels(_startVoxelArea, _endVoxelArea);
        Project.Saved = false;
    }
}
