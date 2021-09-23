using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelMesh
{
    public static readonly Vector3[] VoxelVertices;
    public static readonly int[] VoxelFaceTriangles;

    public static readonly Vector3[] SelectedVoxelVertices;
    public static readonly int[] SelectedVoxelFaceTriangles;

    public static readonly Vector3[] OptimizedVertices;
    public static readonly int[] OptimizedTriangles;

    static VoxelMesh()
    {
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
        VoxelFaceTriangles = new int[]
        {
            0, 1, 3,
            1, 2, 3
        };

        SelectedVoxelVertices = new Vector3[]
        {
            //left
            new Vector3(-0.5f, -0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),

            new Vector3(-0.5f, -0.45f, +0.45f),
            new Vector3(-0.5f, +0.45f, +0.45f),
            new Vector3(-0.5f, +0.45f, -0.45f),
            new Vector3(-0.5f, -0.45f, -0.45f),

            //right
            new Vector3(+0.5f, -0.5f, -0.5f),
            new Vector3(+0.5f, +0.5f, -0.5f),
            new Vector3(+0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, -0.5f, +0.5f),

            new Vector3(+0.5f, -0.45f, -0.45f),
            new Vector3(+0.5f, +0.45f, -0.45f),
            new Vector3(+0.5f, +0.45f, +0.45f),
            new Vector3(+0.5f, -0.45f, +0.45f),

            //bottom
            new Vector3(-0.5f, -0.5f, +0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(+0.5f, -0.5f, -0.5f),
            new Vector3(+0.5f, -0.5f, +0.5f),

            new Vector3(-0.45f, -0.5f, +0.45f),
            new Vector3(-0.45f, -0.5f, -0.45f),
            new Vector3(+0.45f, -0.5f, -0.45f),
            new Vector3(+0.45f, -0.5f, +0.45f),

            //top
            new Vector3(-0.5f, +0.5f, -0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, +0.5f, -0.5f),

            new Vector3(-0.45f, +0.5f, -0.45f),
            new Vector3(-0.45f, +0.5f, +0.45f),
            new Vector3(+0.45f, +0.5f, +0.45f),
            new Vector3(+0.45f, +0.5f, -0.45f),

            //rear
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, +0.5f, -0.5f),
            new Vector3(+0.5f, +0.5f, -0.5f),
            new Vector3(+0.5f, -0.5f, -0.5f),

            new Vector3(-0.45f, -0.45f, -0.5f),
            new Vector3(-0.45f, +0.45f, -0.5f),
            new Vector3(+0.45f, +0.45f, -0.5f),
            new Vector3(+0.45f, -0.45f, -0.5f),

            //front
            new Vector3(+0.5f, -0.5f, +0.5f),
            new Vector3(+0.5f, +0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(-0.5f, -0.5f, +0.5f),

            new Vector3(+0.45f, -0.45f, +0.5f),
            new Vector3(+0.45f, +0.45f, +0.5f),
            new Vector3(-0.45f, +0.45f, +0.5f),
            new Vector3(-0.45f, -0.45f, +0.5f)
        };
        SelectedVoxelFaceTriangles = new int[]
        {
            0, 1, 4,
            1, 5, 4,

            1, 2, 5,
            2, 6, 5,

            2, 3, 6,
            3, 7, 6,

            3, 0, 7,
            0, 4, 7
        };

        OptimizedVertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, -0.5f),
            new Vector3(-0.5f, +0.5f, -0.5f),
            new Vector3(+0.5f, +0.5f, -0.5f),
            new Vector3(+0.5f, -0.5f, -0.5f),

            new Vector3(-0.5f, -0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, +0.5f, +0.5f),
            new Vector3(+0.5f, -0.5f, +0.5f)
        };

        OptimizedTriangles = new int[]
        {
            //left
            4, 5, 0,
            5, 1, 0,

            //right
            3, 2, 7,
            2, 6, 7,

            //bottom
            4, 0, 7,
            0, 3, 7,

            //top
            1, 5, 2,
            5, 6, 2,

            //rear
            7, 6, 4,
            6, 5, 4,

            //front
            0, 1, 3,
            1, 2, 3
        };
    }
}
