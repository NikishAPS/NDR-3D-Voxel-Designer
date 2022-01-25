using System.IO;
using UnityEngine;

public static class BinaryReaderExtension
{
    public static Vector3Int ReadVector3Int(this BinaryReader binaryReader) =>
        new Vector3Int(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());

    public static Vector3 ReadVector3(this BinaryReader binaryReader) =>
        new Vector3(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());

    public static Vector3Byte ReadVector3Byte(this BinaryReader binaryReader) =>
        new Vector3Byte(binaryReader.ReadByte(), binaryReader.ReadByte(), binaryReader.ReadByte());

    public static bool TryRead(this BinaryReader binaryReader, out int result)
    {
        if (CanRead(binaryReader, sizeof(int)))
        {
            result = binaryReader.ReadInt32();
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryRead(this BinaryReader binaryReader, out float result)
    {
        if (CanRead(binaryReader, sizeof(float)))
        {
            result = binaryReader.ReadSingle();
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryRead(this BinaryReader binaryReader, out string result)
    {
        result = binaryReader.ReadString();
        return result != null;
    }

    public static bool TryRead(this BinaryReader binaryReader, out Vector3Int result)
    {
        if (CanRead(binaryReader, sizeof(int) * 3))
        {
            result = binaryReader.ReadVector3Int();
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryRead(this BinaryReader binaryReader, out Vector3 result)
    {
        if (CanRead(binaryReader, sizeof(float) * 3))
        {
            result = ReadVector3(binaryReader);
            return true;
        }

        result = default;
        return false;
    }

    public static bool TryRead(this BinaryReader binaryReader, out Vector3Byte result)
    {
        if(CanRead(binaryReader, sizeof(byte) * 3))
        {
            result = ReadVector3Byte(binaryReader);
            return true;
        }

        result = default;
        return false;
    }

    private static bool CanRead(BinaryReader binaryReader, int bytesCount)
    {
        return binaryReader.BaseStream.Length - binaryReader.BaseStream.Position >= bytesCount;
    }

}