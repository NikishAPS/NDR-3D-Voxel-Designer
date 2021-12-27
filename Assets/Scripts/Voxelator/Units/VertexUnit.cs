using System.ComponentModel;
using UnityEngine;

public class VertexUnit : Unit
{
    public ReportField<Vector3> OffsetPosition { get; set; }

    private static Vector3 _offset = new Vector3(0.5f, 0.5f, 0.5f);

    public VertexUnit(Vector3Int position) : base(position)
    {
        OffsetPosition = new ReportField<Vector3>();
        OffsetPosition.Value = Position - _offset;
    }

    public override void Release()
    {
    }

    public Vector3 GetOffset()
    {
        return OffsetPosition.Value - Position + _offset;
    }

    public void SetOffset(Vector3 value)
    {
        OffsetPosition.Value = Position + value - _offset;
    }

    public void Offset(Vector3 value)
    {
        OffsetPosition.Value += value;
    }

}
