using UnityEngine;
using System.Collections;

public static class Vector3Extension
{
    public static Vector3Int ToVector3Int(this Vector3 value)
    {
        return new Vector3Int((int)value.x, (int)value.y, (int)value.z);
    }

    public static Vector3Int RoundToInt(this Vector3 value)
    {
        return new Vector3Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y), Mathf.RoundToInt(value.z));
    }

    public static Vector3 RoundToFloat(this Vector3 value)
    {
        return new Vector3(Mathf.Round(value.x), Mathf.Round(value.y), Mathf.Round(value.z));
    }
}
