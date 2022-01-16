using UnityEngine;

public struct Vector3Byte
{
    public byte x, y, z;

    public Vector3Byte(byte x, byte y, byte z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3Int ToVector3Int()
    {
        return new Vector3Int(x, y, z);
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
