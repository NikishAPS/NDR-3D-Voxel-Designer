using UnityEngine;
using System.Collections;

public class CustomMesh
{
    public readonly Vector3[] Vertices;
    public readonly Vector2[] UV;
    public readonly int[] Triangles;

    public CustomMesh(Vector3[] vertices, Vector2[] uv, int[] triangles)
    {
        Vertices = vertices;
        UV = uv;
        Triangles = triangles;

    }
}
