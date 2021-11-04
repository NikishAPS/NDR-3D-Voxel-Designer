using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public static class MeshGenerator
{
    public static CustomMesh GenerateHorizontalPlane()
    {
        return GenerateHorizontalPlane(Vector3.zero);
    }

    public static CustomMesh GenerateMesh(Vector3[] vertices)
    {
        if(vertices == null || vertices.Length !=4)
            throw new ArgumentException($"Array limited error: {vertices}");
        //try
        //{
        //    Vector3[] ver = new List<Vector3>(vertices.Take(4)).ToArray();
        //}
        //catch
        //{
        //    throw new ArgumentException($"Array limeted error: {vertices}");
        //}
        
        Vector2[] uv = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(1, 0),
            new Vector2(1,1),
            new Vector2(0, 1)
        };

        int[] triangles = new int[]
         {
            0, 1, 3,
            1, 2, 3,
         };

        return new CustomMesh(vertices, uv, triangles);

    }

    public static CustomMesh GenerateHorizontalPlane(Vector3 offset)
    {
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, 0, -0.5f) + offset,
            new Vector3(-0.5f, 0, +0.5f) + offset,
            new Vector3(+0.5f, 0, +0.5f) + offset,
            new Vector3(+0.5f, 0, -0.5f) + offset
        };

        Vector2[] uv = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

        int[] triangles = new int[]
        {
            0, 1, 3,
            1, 2, 3,
        };

        return new CustomMesh(vertices, uv, triangles);
    }
}
