using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateVoxelsCommand : ICommand
{
    private Vector3Int _startVoxelArea, _endVoxelArea;

    public CreateVoxelsCommand(Vector3Int startVoxelArea, Vector3Int endVoxelArea)
    {
        _startVoxelArea = startVoxelArea;
        _endVoxelArea = endVoxelArea;
    }

    public void Execute()
    {
        Vector3Int steps = (_endVoxelArea - _startVoxelArea).Sign();
        _endVoxelArea += steps;

        for (int x = _startVoxelArea.x; x != _endVoxelArea.x; x += steps.x)
        {
            for (int y = _startVoxelArea.y; y != _endVoxelArea.y; y += steps.y)
            {
                for (int z = _startVoxelArea.z; z != _endVoxelArea.z; z += steps.z)
                {
                    Voxelator.CreateVoxel(new Vector3Int(x, y, z));
                }
            }
        }

        Voxelator.UpdateChunks();
    }

    public void Undo()
    {

    }

}
