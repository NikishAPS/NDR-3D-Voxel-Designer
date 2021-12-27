using UnityEngine;

public class VertexChunk : Chunk<VertexUnit>
{
    public bool VerticesActive { get => _parent.activeSelf; set => _parent.SetActive(value); }

    private readonly static GameObject _parent;

    static VertexChunk()
    {
        _parent = new GameObject("VertexChunks");
    }

    public VertexChunk(Vector3Int position, Vector3Int size, Material material, string name) : base(position, size, material, name)
    {
        _gameObject.transform.SetParent(_parent.transform);
    }

    public override void UpdateMesh()
    {
        _mesh.Clear();

        if (UnitsCount == 0) return;

        InitializedArray<Vector3> vertices = new InitializedArray<Vector3>(UnitsCount * MeshGenerator.SphereMesh.Vertices.Length);
        InitializedArray<int> triangles = new InitializedArray<int>(UnitsCount * MeshGenerator.SphereMesh.Triangles.Length);

        int iTriang = 0;
        foreach (VertexUnit vertex in Units)
        {
            if (vertex != null)
            {
                foreach (Vector3 meshVertex in MeshGenerator.SphereMesh.Vertices)
                    vertices.Init(meshVertex + vertex.OffsetPosition.Value);

                foreach (int triangleIndex in MeshGenerator.SphereMesh.Triangles)
                    triangles.Init(triangleIndex + iTriang);

                iTriang += MeshGenerator.SphereMesh.Vertices.Length;
            }
        }

        _mesh.vertices = vertices.Array;
        _mesh.triangles = triangles.Array;
        //_mesh.Optimize();
        //_mesh.RecalculateNormals();
    }

}
