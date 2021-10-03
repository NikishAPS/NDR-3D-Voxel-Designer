using UnityEngine;

public class SetVertexPositionCommand : Command
{
    private Vector3 _pivotVertexPosition;
    private Vector3 _position;

    public SetVertexPositionCommand(Vector3 pivotVertexPosition, Vector3 position)
    {
        _pivotVertexPosition = pivotVertexPosition;
        _position = position;
    }

    public override void Execute()
    {
        Presenter.SetVertexPosition(_pivotVertexPosition, _position);
        Axes.Position = _position;
    }
}
