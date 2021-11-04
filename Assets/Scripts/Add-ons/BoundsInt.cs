using System;
using UnityEngine;

public struct BoundsInt
{
    public Vector3Int Size { get; private set; }
    public Vector3 Extents { get; private set; }
    public Vector3 Center { get; private set; }
    public Vector3Int Min { get; private set; }
    public Vector3Int Max { get; private set; }

    public BoundsInt(Vector3Int firstPoint, Vector3Int secondPoint)
    {
        Size = (secondPoint - firstPoint).Abs();
        Extents = Size.ToVector3() * 0.5f;
        Center = (firstPoint + secondPoint).ToVector3() * 0.5f;
        Min = (Center - Extents).ToVector3Int();
        Max = (Center + Extents).ToVector3Int();
    }

    public bool Contains(Vector3 position)
    {
        return
            position.x >= Min.x && position.x <= Max.x &&
            position.y >= Min.y && position.y <= Max.y &&
            position.z >= Min.z && position.z <= Max.z;
    }

    

}