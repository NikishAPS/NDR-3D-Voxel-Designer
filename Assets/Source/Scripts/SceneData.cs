using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneData
{
    //parameters
    public static readonly float rayLength = 500;
    public static readonly int textureSize = 5;
    public static readonly float textureMul = 1.0f / textureSize;


    public static Camera camera;
    public static Transform mainCamera;
    public static Chunk chunk;
    public static Grid grid;
    public static Extractor extractor;
    public static ControlGUI controlGUI;
    public static EventInput eventInput;

    public static ColorTest colorTest;


    public static int debug;

    public static Vector3[] voxelVertices;
    public static Vector2[] uvVertices;

    public static Vector3 Vector3IntToFloat(Vector3Int value)
    {
        return new Vector3((float)value.x, (float)value.y, (float)value.z);
    }

    public static Vector3Int Vector3FloatToInt(Vector3 value)
    {
        return new Vector3Int((int)value.x, (int)value.y, (int)value.z);
    }

    public static Point3Int Vector3IntToPoint3Int(Vector3Int pos)
    {
        return new Point3Int(pos.x, pos.y, pos.z);
    }

    public static Vector3 Point3IntToVector3(Point3Int pos)
    {
        return new Vector3(pos.x, pos.y, pos.z);
    }
}
