using UnityEngine;
using System.Collections;

public static class Vector3Extension
{
    public static Vector3 Set(this Vector3 vector3, Vector3 value)
    {
        return new Vector3(value.x, value.y, value.z);
    }

    public static Vector3Int ToVector3Int(this Vector3 vector3)
    {
        return new Vector3Int((int)vector3.x, (int)vector3.y, (int)vector3.z);
    }

    public static Vector3Int RoundToInt(this Vector3 vector3)
    {
        return new Vector3Int(Mathf.RoundToInt(vector3.x), Mathf.RoundToInt(vector3.y), Mathf.RoundToInt(vector3.z));
    }

    public static Vector3 RoundToFloat(this Vector3 vector3)
    {
        return new Vector3(Mathf.Round(vector3.x), Mathf.Round(vector3.y), Mathf.Round(vector3.z));
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
    
    public static Vector3 Sign(this Vector3 vector3)
    {
        return new Vector3(Mathf.Sign(vector3.x), Mathf.Sign(vector3.y), Mathf.Sign(vector3.z));
    }

}
