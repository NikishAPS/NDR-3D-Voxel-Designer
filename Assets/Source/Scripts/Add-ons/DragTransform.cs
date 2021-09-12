using UnityEngine;

public class DragTransform
{
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }

    public DragTransform(Vector3 position, Vector3 scale)
    {
        Position = position;
        Scale = scale;
    }

    public DragTransform (Transform transform)
    {
        Position = transform.position;
        Scale = transform.localScale;
    }
}
