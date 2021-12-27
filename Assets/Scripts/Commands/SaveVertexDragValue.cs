using UnityEngine;

public class SaveVertexDragValue : ICommand
{
    private Vector3 _dragValue;

    public SaveVertexDragValue(Vector3 dragValue)
    {
        _dragValue = dragValue;
    }

    public void Execute()
    {

    }

    public void Undo()
    {
        Voxelator.OffsetSelectedVertex(_dragValue);
        Voxelator.UpdateChunks();
    }
}
