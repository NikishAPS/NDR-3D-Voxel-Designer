using UnityEngine;
using System.Collections;

public class SelectVertexCommand : Command
{
    private Vertex _vertex;

    public SelectVertexCommand(Vertex vertex)
    {
        _vertex = new Vertex(vertex);
    }

    public override void Execute()
    {
        Presenter.SelectVertex(_vertex.GetOffset());
    }
}
