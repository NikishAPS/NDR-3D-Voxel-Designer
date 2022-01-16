using UnityEngine;
using System.Collections;

public static class Vector3IntExtension
{
    public static Vector3Int Set(this Vector3Int vector3Int, Vector3Int value)
    {
        return new Vector3Int(value.x, value.y, value.y);
    }

    public static Vector3 ToVector3(this Vector3Int vector3Int)
    {
        return new Vector3(vector3Int.x, vector3Int.y, vector3Int.z);
    }

    public static Vector3Byte ToVector3Byte(this Vector3Int vector3)
    {
        return new Vector3Byte((byte)vector3.x, (byte)vector3.y, (byte)vector3.z);
    }

    public static Vector3Int Forward(this Vector3Int vector3Int)
    {
        return new Vector3Int(0, 0, 1);
    }

    public static Vector3Int Back(this Vector3Int vector3Int)
    {
        return new Vector3Int(0, 0, -1);
    }

    public static Vector3Int Div(this Vector3Int vector3Int, Vector3Int value)
    {
        return new Vector3Int(vector3Int.x / value.x, vector3Int.y / value.y, vector3Int.z / value.z);
    }

    public static Vector3Int Mul(this Vector3Int vector3Int, Vector3Int value)
    {
        return new Vector3Int(vector3Int.x * value.x, vector3Int.y * value.y, vector3Int.z * value.z);
    }

    public static Vector3Int Abs(this Vector3Int vector3Int)
    {
        return new Vector3Int(Mathf.Abs(vector3Int.x), Mathf.Abs(vector3Int.y), Mathf.Abs(vector3Int.z));
    }

    public static Vector3Int Sign(this Vector3Int vector3Int)
    {
        return new Vector3(Mathf.Sign(vector3Int.x), Mathf.Sign(vector3Int.y), Mathf.Sign(vector3Int.z)).ToVector3Int();
    }

    public static int Max(this Vector3Int vector3Int)
    {
        return vector3Int.x > vector3Int.y ? (vector3Int.x > vector3Int.z ? vector3Int.x : vector3Int.z) :
           vector3Int.y > vector3Int.z ? vector3Int.y : vector3Int.z;

        if (vector3Int.x > vector3Int.y)
        {
            if (vector3Int.x > vector3Int.z) return vector3Int.x;
            else return vector3Int.z;
        }
        else if (vector3Int.y > vector3Int.z)
        {
            return vector3Int.y;
        }
        else return vector3Int.z;
    }

}
