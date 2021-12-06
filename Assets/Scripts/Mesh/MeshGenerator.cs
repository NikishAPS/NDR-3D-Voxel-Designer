using System;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public readonly static CustomMesh SphereMesh;

    static MeshGenerator()
    {
        //Mesh sphereMesh = ResourcesLoader.Load<GameObject>("Objects/VertexCube").GetComponent<MeshFilter>().sharedMesh;
        Mesh sphereMesh = ResourcesLoader.Load<GameObject>("Objects/VertexSphere").GetComponent<MeshFilter>().sharedMesh;

        SphereMesh = new CustomMesh(sphereMesh.vertices, null, sphereMesh.triangles);
    }

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

    public static CustomMesh GenereteSphere(Vector3Int vertexOffset, int trianglesOffset)
    {
        Vector3[] vertices = new Vector3[SphereMesh.Vertices.Length];
        int[] triangles = new int[SphereMesh.Triangles.Length];

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = SphereMesh.Vertices[i] + vertexOffset;

        for (int i = 0; i < triangles.Length; i++)
            triangles[i] = SphereMesh.Triangles[i] + trianglesOffset;

        return new CustomMesh(vertices, null, triangles);
    }
}
