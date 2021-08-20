using UnityEngine;

public class BuildPoints : MonoBehaviour
{
    [SerializeField]
    private Vector3Int _size = new Vector3Int(10, 10, 10);
    [SerializeField]
    private float _pointSize = 0.1f;

    private void Start()
    {
        CreateMesh();
    }

    private void CreateMesh()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;



        Vector3[] vertices = new Vector3[_size.x * _size.y * _size.z * 8];
        int[] triangles = new int[vertices.Length * 9 / 2];



        int v = 0; //vertex index
        int t = 0; //triangle index
        for (int x = 0; x < _size.x; x++)
        {
            for (int y = 0; y < _size.y; y++)
            {
                for (int z = 0; z < _size.z; z++)
                {
                    for (int j = 0; j < VoxelMesh.OptimizedVertices.Length; j++)
                    {
                        vertices[v + j] = VoxelMesh.OptimizedVertices[j] * _pointSize + new Vector3(x, y, z);
                    }

                    for (int j = 0; j < VoxelMesh.OptimizedTriangles.Length; j++)
                    {
                        triangles[t + j] = VoxelMesh.OptimizedTriangles[j] + v;
                    }

                    v += 8;
                    t += 36;
                }
            }
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
