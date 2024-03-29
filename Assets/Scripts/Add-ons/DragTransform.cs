﻿using UnityEngine;

public struct DragTransform
{
    public Vector3 Position;
    public Vector3 Scale;
    public static DragTransform Zero => new DragTransform(Vector3.zero, Vector3.zero);

    public DragTransform(Vector3 position)
    {
        Position = position;
        Scale = Vector3.zero;
    }

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

    public static DragTransform operator +(DragTransform summand1, DragTransform summand2)
    {
        return new DragTransform(summand1.Position + summand2.Position, summand1.Scale + summand2.Scale);
    }

}
