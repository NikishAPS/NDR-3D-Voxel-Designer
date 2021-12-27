using UnityEngine;

public class SetVertexOffsetCommand : ICommand
{
    private Vector3Int _vertexPosition;
    private Vector3 _currentVertexOffset, _vertexOffset;

    public SetVertexOffsetCommand(Vector3 vertexOffset)
    {
        _vertexPosition = Voxelator.VertexChunkManager.SelectedVertex.Position;
        _currentVertexOffset = Voxelator.VertexChunkManager.SelectedVertex.GetOffset();
        _vertexOffset = vertexOffset;
    }

    public SetVertexOffsetCommand(Vector3Int vertexPosition, Vector3 vertexOffset)
    {
        _vertexPosition = vertexPosition;
        _vertexOffset = vertexOffset;
    }

    public void Execute()
    {
        Voxelator.SetVertexOffset(_vertexPosition, _vertexOffset);
        Voxelator.UpdateChunks();
    }

    public void Undo()
    {
        Voxelator.SetVertexOffset(_vertexPosition, _currentVertexOffset);
        Voxelator.UpdateChunks();
    }

}
