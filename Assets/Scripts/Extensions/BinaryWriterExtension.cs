using System.IO;
using UnityEngine;

public static class BinaryWriterExtension
{
    public static void Write(this BinaryWriter binaryWriter, Vector3Int value)
    {
        binaryWriter.Write(value.x);
        binaryWriter.Write(value.y);
        binaryWriter.Write(value.z);
    }

    public static void Write(this BinaryWriter binaryWriter, Vector3 value)
    {
        binaryWriter.Write(value.x);
        binaryWriter.Write(value.y);
        binaryWriter.Write(value.z);
    }
}