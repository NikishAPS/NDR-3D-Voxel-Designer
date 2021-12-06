using UnityEngine;

public class VertexUnit : Unit
{
    public Vector3 OffsetPosition { get; set; }
    public Vector3 Offset { get => OffsetPosition - Position; set { OffsetPosition = Position + value - _offset; } }

    private static Vector3 _offset = new Vector3(0.5f, 0.5f, 0.5f);

    public VertexUnit(Vector3Int position) : base(position)
    {
        OffsetPosition = Position - _offset;
    }

    public override void Release()
    {
    }
}
