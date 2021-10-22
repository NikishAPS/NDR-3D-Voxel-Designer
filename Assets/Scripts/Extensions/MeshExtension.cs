using UnityEngine;
using UnityEditor;

public static class MeshExtension
{
    public static void SetCustomMesh(this Mesh mesh, CustomMesh customMesh)
    {
        mesh.Clear();

        mesh.vertices = customMesh.Vertices;
        mesh.uv = customMesh.UV;
        mesh.triangles = customMesh.Triangles;
    }
}