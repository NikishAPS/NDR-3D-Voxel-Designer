using UnityEngine;

public class SaveSelectedVoxelDragValue : ICommand
{
    private Vector3 _dragValue;

    public SaveSelectedVoxelDragValue(Vector3 dragValue)
    {
        _dragValue = dragValue;
    }

    public void Execute()
    {
        
    }

    public void Undo()
    {
        Vector3 dragResult;
        Voxelator.DragSelectedVoxels(-_dragValue, out dragResult);
        MonoBehaviour.print(_dragValue);
        Voxelator.UpdateChunks();
    }
}
