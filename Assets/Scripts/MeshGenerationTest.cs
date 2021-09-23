using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerationTest : MonoBehaviour
{
    Mesh mesh;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-0.5f, -0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, +0.5f),
            new Vector3(-0.5f, +0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f, -0.5f),

            new Vector3(-0.5f, -0.45f, +0.45f),
            new Vector3(-0.5f, +0.45f, +0.45f),
            new Vector3(-0.5f, +0.45f, -0.45f),
            new Vector3(-0.5f, -0.45f, -0.45f)
        };

        int[] triangles = new int[]
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

        for(int i = 0; i < triangles.Length; i++)
        {
            triangles[i + 24] = triangles[i] + 8;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
