using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneData
{
    static SceneData()
    {
        Camera = Camera.main;
        MainCamera = Camera.main.transform;

        VoxelVertices = new Vector3[]
        {
            //left
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),

            //right
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),

            //bottom
            new Vector3(-0.5f, -0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, 0.5f),

            //top
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(+0.5f, 0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),

            //rear
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, 0.5f, -0.5f),
            new Vector3(0.5f, -0.5f, -0.5f),

            //front
            new Vector3(0.5f, -0.5f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, 0.5f, 0.5f),
            new Vector3(-0.5f, -0.5f, 0.5f)
    };
        sphereVertices = new Vector3[]
        {

        };
    }
    
    //parameters
    public static readonly float RayLength = 500 / 5;
    public static readonly float RayStep = 0.1f;
    public static readonly int TextureSize = 5;
    public static readonly float TextureMul = 1.0f / TextureSize;


    public static readonly Camera Camera;
    public static readonly Transform MainCamera;

    public static readonly Vector3[] VoxelVertices;
    public static readonly Vector2[] VoxelUV;

    public static readonly Vector3[] sphereVertices;

    public static Vector3 Vector3IntToFloat(Vector3Int value)
    {
        return new Vector3((float)value.x, (float)value.y, (float)value.z);
    }

    public static Vector3Int Vector3FloatToInt(Vector3 value)
    {
        return new Vector3Int((int)value.x, (int)value.y, (int)value.z);
    }

    public static Vector3Int Vector3FloatRound(Vector3 value)
    {
        return new Vector3Int((int)Mathf.Round(value.x), (int)Mathf.Round(value.y), (int)Mathf.Round(value.z));
    }
}
