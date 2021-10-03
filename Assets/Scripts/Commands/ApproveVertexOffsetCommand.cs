using UnityEngine;

public class ApproveVertexOffsetCommand : Command
{
    private Vector3 position;
    private Vector3 offset;

    public ApproveVertexOffsetCommand(Vertex vertex, Vector3 offset)
    {
        position = vertex.Position;
        offset = vertex.Position;
    }

    public override void Execute()
    {
        Presenter.EditVertex();
    }
}
