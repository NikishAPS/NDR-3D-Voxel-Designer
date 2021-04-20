using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneData
{
    public static Transform mainCamera;
    public static Chunk chunk;
    public static Grid grid;
    public static Extractor extractor;
    public static ControlGUI controlGUI;

    public static Vector3 Vector3IntToFloat(Vector3Int value)
    {
        return new Vector3((float)value.x, (float)value.y, (float)value.z);
    }

    public static Vector3Int Vector3FloatToInt(Vector3 value)
    {
        return new Vector3Int((int)value.x, (int)value.y, (int)value.z);
    }
}
