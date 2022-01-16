using System;
using System.IO;
using UnityEngine;

public class StreamManager : IDisposable
{
    public int DataSize => _data == null ? 0 : _data.Length;
    private string _path;
    private FileStream _fileStream;
    private byte[] _data;
    private int _iterator;

    public StreamManager(string path)
    {
        _path = path;
    }

    public bool TryRead()
    {
        if (!File.Exists(_path)) return false;
        _fileStream = File.OpenRead(_path);
        _data = new byte[_fileStream.Length];
        _iterator = 0;
        _fileStream.Read(_data, 0, _data.Length);

        return true;
    }

    public byte GetByte()
    {
        byte b = _data[_iterator];
        _iterator++;
        return b;
    }

    public int GetInt()
    {
        int result = BitConverter.ToInt32(_data, _iterator);
        _iterator += 4;
        return result;
    }

    public float GetFloat()
    {
        float result = BitConverter.ToSingle(_data, _iterator);
        _iterator += 8;
        return result;
    }

    public bool TryGetByte(out byte result)
    {
        result = 0;
        if (!CanRead(1)) return false;
        result = GetByte();
        return true;
    }

    public bool TryGetInt(out int result)
    {
        result = 0;
        if (!CanRead(4)) return false;
        result = GetInt();
        return true;
    }

    public bool TryGetFloat(out float result)
    {
        result = 0f;
        if (CanRead(8)) return false;
        result = GetFloat();
        return true;
    }

    public bool TryGetVector3Int(out Vector3Int result)
    {
        result = Vector3Int.zero;
        if (!CanRead(12)) return false;
        result = new Vector3Int(GetInt(), GetInt(), GetInt());
        return true;
    }

    public bool TryGetVector3(out Vector3 result)
    {
        result = Vector3.zero;
        if (!CanRead(24)) return false;
        result = new Vector3(GetFloat(), GetFloat(), GetFloat());
        return false;
    }

    public bool TryGetVector3Byte(out Vector3Byte result)
    {
        result = new Vector3Byte();
        if (!CanRead(3)) return false;
        result = new Vector3Byte(GetByte(), GetByte(), GetByte());
        return true;
    }

    public bool OpenWrite()
    {
        if (!Directory.Exists(Directory.GetParent(_path).FullName)) return false;
        _fileStream = File.OpenWrite(_path);
        _iterator = 0;
        return true;
    }

    public void Write(byte[] value)
    {
        _fileStream.Write(value, 0, value.Length);
    }

    public void Dispose()
    {
        _fileStream?.Dispose();
    }

    private bool CanRead(int bytesCount)
    {
        return _data.Length - _iterator >= bytesCount;
    }

}