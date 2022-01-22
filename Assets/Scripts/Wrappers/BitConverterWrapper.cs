using System;
using UnityEngine;

public static class BitConverterWrapper
{
    public static byte[] GetBytes(Vector3 value)
    {
        byte[] result = new byte[24];

        byte[] bytes = BitConverter.GetBytes(value.x);
        for (int i = 0; i < bytes.Length; i++)
            result[i] = bytes[i];

        bytes = BitConverter.GetBytes(value.y);
        for (int i = 0; i < bytes.Length; i++)
            result[i + 8] = bytes[i];

        bytes = BitConverter.GetBytes(value.z);
        for (int i = 0; i < bytes.Length; i++)
            result[i + 16] = bytes[i];

        return result;
    }

    public static byte[] GetBytes(Vector3Int value)
    {
        byte[] result = new byte[12];

        byte[] bytes = BitConverter.GetBytes(value.x);
        for (int i = 0; i < bytes.Length; i++)
            result[i] = bytes[i];

        bytes = BitConverter.GetBytes(value.y);
        for (int i = 0; i < bytes.Length; i++)
            result[i + 4] = bytes[i];

        bytes = BitConverter.GetBytes(value.z);
        for (int i = 0; i < bytes.Length; i++)
            result[i + 8] = bytes[i];

        return result;
    }

    public static byte[] GetBytes(Vector3Byte value)
    {
        byte[] result = new byte[3]
        {
            value.x, value.y, value.z
        };

        return result;
    }

}