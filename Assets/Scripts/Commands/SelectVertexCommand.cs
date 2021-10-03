using UnityEngine;
using System.Collections;

public class SelectVertexCommand : Command
{
    private Vertex _vertex;

    public SelectVertexCommand(Vertex vertex)
    {
        _vertex = vertex;
    }

    public override void Execute()
    {
        ChunkManager.SelectedVertex = _vertex;
        Presenter.EditVertex();
    }
}
