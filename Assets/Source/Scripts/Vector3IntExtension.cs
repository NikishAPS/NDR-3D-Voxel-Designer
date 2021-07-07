using UnityEngine;
using System.Collections;

public static class Vector3IntExtension
{
    public static Vector3Int Forward(this Vector3Int value)
    {
        return new Vector3Int(0, 0, 1);
    }

    public static Vector3Int Back(this Vector3Int value)
    {
        return new Vector3Int(0, 0, -1);
    }
}
