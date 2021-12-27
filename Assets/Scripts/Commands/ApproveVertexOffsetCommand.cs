using UnityEngine;

public class ApproveVertexOffsetCommand : ICommand
{
    private Vector3 position;
    private Vector3 offset;

    public ApproveVertexOffsetCommand(VertexUnit vertex, Vector3 offset)
    {
        position = vertex.Position;
        offset = vertex.Position;
    }

    public void Execute()
    {
        Presenter.EditVertex();
    }

    public void Undo()
    {
        throw new System.NotImplementedException();
    }

    public void Updo()
    {

    }
}
