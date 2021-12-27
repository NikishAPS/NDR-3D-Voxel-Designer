using UnityEngine;
using System.Collections;

public class SelectVertexCommand : ICommand
{
    private VertexUnit _vertex;
    private Vector3Int _vertexPosition;

    public SelectVertexCommand(Vector3Int vertexPosition)
    {
        _vertexPosition = vertexPosition;
    }

    public void Execute()
    {
        //ChunkManager.SelectedVertex = _vertex;
        //Presenter.EditVertex();
        Voxelator.SelectVertex(_vertexPosition);
    }

    public void Undo()
    {

    }
}
