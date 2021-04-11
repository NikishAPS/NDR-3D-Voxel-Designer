using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    public Vector3[] vartices;
    public Vector3[] normals;
    public int[] triangles;

    private Mesh mesh;

    public Material material;

    void Start()
    {
        GameObject go = new GameObject();
        mesh = new Mesh();
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = material;

        mesh.SetVertices(vartices);
        mesh.SetNormals(normals);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateBounds();
    }

    void Update()
    {
        
    }
}
