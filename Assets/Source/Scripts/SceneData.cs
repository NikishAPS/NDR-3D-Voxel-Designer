using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneData
{
    static SceneData()
    {
        camera = Camera.main;
        mainCamera = Camera.main.transform;
        chunk = Object.FindObjectOfType<Chunk>();
        grid = Object.FindObjectOfType<Grid>();
        modeControl = Object.FindObjectOfType<ModeControl>();
        extractor = Object.FindObjectOfType<Extractor>();
        controlGUI = Object.FindObjectOfType<ControlGUI>();
        eventInput = Object.FindObjectOfType<EventInput>();
        dragSystem = Object.FindObjectOfType<DragSystem>();

        colorTest = Object.FindObjectOfType<ColorTest>();

        voxelVertices = new Vector3[]
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

        MonoBehaviour.print("aaaa");
    }
    
    //parameters
    public static readonly float rayLength = 500;
    public static readonly float rayStep = 0.1f;
    public static readonly int textureSize = 5;
    public static readonly float textureMul = 1.0f / textureSize;


    public static readonly Camera camera;
    public static readonly Transform mainCamera;
    public static readonly Chunk chunk;
    public static readonly Grid grid;
    public static readonly ModeControl modeControl;
    public static readonly Extractor extractor;
    public static readonly ControlGUI controlGUI;
    public static readonly EventInput eventInput;
    public static readonly DragSystem dragSystem;

    public static readonly ColorTest colorTest;

    public static readonly int debug;

    public static readonly Vector3[] voxelVertices;
    public static readonly Vector2[] uvVertices;

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

    public static Point3Int Vector3IntToPoint3Int(Vector3Int pos)
    {
        return new Point3Int(pos.x, pos.y, pos.z);
    }

    public static Vector3 Point3IntToVector3(Point3Int pos)
    {
        return new Vector3(pos.x, pos.y, pos.z);
    }

    public static Vector3Int Point3IntToVector3Int(Point3Int pos)
    {
        return new Vector3Int(pos.x, pos.y, pos.z);
    }
}
