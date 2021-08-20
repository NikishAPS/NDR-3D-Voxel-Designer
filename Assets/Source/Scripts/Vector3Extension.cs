using UnityEngine;
using System.Collections;

public static class Vector3Extension
{
    public static Vector3 Set(this Vector3 vector3, Vector3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }

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

    public static Vector3 Div(this Vector3 vector3, Vector3 value)
    {
        return new Vector3(vector3.x / value.x, vector3.y / value.y, vector3.z / value.z);
    }

    public static Vector3 Mul(this Vector3 vector3, Vector3 value)
    {
        return new Vector3(vector3.x * value.x, vector3.y * value.y, vector3.z * value.z);
    }

    public static Vector3 Abs(this Vector3 vector3)
    {
        return new Vector3(Mathf.Abs(vector3.x), Mathf.Abs(vector3.y), Mathf.Abs(vector3.z));
    }
    
}
