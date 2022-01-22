using UnityEngine;

public class VertexUnit : Unit
{
    public Vector3 OffsetPosition { get; set; }

    private static Vector3 _offset = new Vector3(0.5f, 0.5f, 0.5f);

    public VertexUnit(Vector3Int position) : base(position)
    {
        OffsetPosition = Position - _offset;
    }

    public override void Release()
    {
    }

    public Vector3 GetOffset()
    {
        return OffsetPosition - Position + _offset;
    }

    public void SetOffset(Vector3 value)
    {
        OffsetPosition = Position + value - _offset;
    }

    public void Offset(Vector3 value)
    {
        OffsetPosition += value;
    }

}
