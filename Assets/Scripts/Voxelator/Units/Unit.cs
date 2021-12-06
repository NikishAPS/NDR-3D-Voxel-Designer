using UnityEngine;
using System.Collections;

public abstract class Unit
{
    public readonly Vector3Int Position;

    public Unit(Vector3Int position)
    {
        Position = position;
    }

    public abstract void Release();

}
