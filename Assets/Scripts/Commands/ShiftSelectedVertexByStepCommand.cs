using UnityEngine;

public class ShiftSelectedVertexByStepCommand : ICommand
{
    private Vector3Int _stepValue;

    public ShiftSelectedVertexByStepCommand(Vector3Int stepValue)
    {
        _stepValue = stepValue;
    }

    public void Execute()
    {
        Voxelator.ShiftSelectedVertexByStep(_stepValue);
        Voxelator.UpdateChunks();
    }

    public void Undo()
    {
        Voxelator.ShiftSelectedVertexByStep(_stepValue);
        Voxelator.UpdateChunks();
    }
}
