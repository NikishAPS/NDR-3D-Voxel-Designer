using System.Collections.Generic;
using UnityEngine;

public class VoxelChunk : Chunk<VoxelUnit>
{
    public static VertexChunkManager VertexChunkManager { get; set; }

    public bool UpdateVoxelMeshFlag { get; set; }
    public bool UpdateGridSelectedVoxelMeshFlag { get; set; }

    public int FaceCount { get; set; }
    public int SelectedFaceCount { get; set; }

    public bool GridVoxelActive { get => _gridVoxelGameObject.activeSelf; set => _gridVoxelGameObject.SetActive(value); }
    public bool GridSelectedVoxelActive { get => _gridSelectedVoxelGameObject.activeSelf; set => _gridSelectedVoxelGameObject.SetActive(value); }

    private readonly static GameObject _voxelChunksParent;
    private readonly static GameObject _gridChunksParent;
    private readonly static GameObject _gridSelectedChunksParent;
    private readonly GameObject _gridVoxelGameObject;
    private readonly GameObject _gridSelectedVoxelGameObject;
    private readonly Mesh _gridVoxelMesh;
    private readonly Mesh _gridSelectedVoxelMesh;

    private static readonly Vector3Int[] VertexPositions = new Vector3Int[]
    {
            //left
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 1, 1),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 0, 0),

            //right
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 1, 1),
            new Vector3Int(1, 0, 1),

            //bottom
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, 1),

            //top
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 1, 1),
            new Vector3Int(1, 1, 1),
            new Vector3Int(1, 1, 0),

            //rear
            new Vector3Int(0, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
            new Vector3Int(1, 0, 0),

            //front
            new Vector3Int(1, 0, 1),
            new Vector3Int(1, 1, 1),
            new Vector3Int(0, 1, 1),
            new Vector3Int(0, 0, 1)
    };

    static VoxelChunk()
    {
        _voxelChunksParent = new GameObject("VoxelChunks");
        _gridChunksParent = new GameObject("GridChunks");
        _gridSelectedChunksParent = new GameObject("GridSelectedChunks");
    }

    public VoxelChunk(Vector3Int position, Vector3Int size, Material material, string name, Material gridChunkMaterial, Material gridSelectedChunkMaterial) :
        base(position, size, material, name)
    {
        _gameObject.transform.SetParent(_voxelChunksParent.transform);

        _gridVoxelMesh = new Mesh();
        _gridVoxelGameObject = new GameObject("GridVoxelChunk");
        _gridVoxelGameObject.transform.SetParent(_gridChunksParent.transform);
        _gridVoxelGameObject.AddComponent<MeshFilter>().mesh = _gridVoxelMesh;
        _gridVoxelGameObject.AddMeshRenderer(gridChunkMaterial, false);

        _gridSelectedVoxelMesh = new Mesh();
        _gridSelectedVoxelGameObject = new GameObject("GridSelectedVoxelChunk");
        _gridSelectedVoxelGameObject.transform.SetParent(_gridSelectedChunksParent.transform);
        _gridSelectedVoxelGameObject.AddComponent<MeshFilter>().mesh = _gridSelectedVoxelMesh;
        _gridSelectedVoxelGameObject.AddMeshRenderer(gridSelectedChunkMaterial, false);
    }

    public override void UpdateMesh()
    {
        if (UpdateGridSelectedVoxelMeshFlag) UpdateGridSelectedVoxelMesh();

        UpdateVoxelMeshFlag = false;

        _mesh.Clear();
        _gridVoxelMesh.Clear();

        if (FaceCount == 0) return;

        InitializedArray<Vector3> vertices = new InitializedArray<Vector3>(FaceCount * 4);
        InitializedArray<int> triangles = new InitializedArray<int>(FaceCount * 6);
        InitializedArray<Color32> colors = new InitializedArray<Color32>(vertices.Length);

        InitializedArray<Vector2> gridUV = new InitializedArray<Vector2>(FaceCount * 4);

        int iTriang = 0;
        foreach (VoxelUnit voxel in Units)
        {
            if (voxel != null)
            {
                for (int i = 0; i < Direction.Masks.Length; i++)
                {
                    if (voxel.CheckFace(i))
                    {
                        //voxels
                        for (int j = 0; j < 4; j++)
                        {
                            vertices.Init(VertexChunkManager.GetUnit(VertexPositions[i * 4 + j] + voxel.Position).OffsetPosition.Value);
                            colors.Init(new Color32(voxel.Color.x, voxel.Color.y, voxel.Color.z, 1));
                        }

                        for (int j = 0; j < 6; j++)
                            triangles.Init(VoxelMesh.VoxelFaceTriangles[j] + iTriang);
                        iTriang += 4;

                        //gridVoxel
                        gridUV.Init(new Vector2(0, 0));
                        gridUV.Init(new Vector2(1, 0));
                        gridUV.Init(new Vector2(1, 1));
                        gridUV.Init(new Vector2(0, 1));

                    }
                }
            }
        }

        _mesh.vertices = vertices.Array;
        _mesh.triangles = triangles.Array;
        _mesh.colors32 = colors.Array;
        //_mesh.Optimize();
        _mesh.RecalculateNormals();

        _gridVoxelMesh.vertices = vertices.Array;
        _gridVoxelMesh.triangles = triangles.Array;
        _gridVoxelMesh.uv = gridUV.Array;
        //_gridVoxelMesh.Optimize();
        _gridVoxelMesh.RecalculateNormals();
    }

    public void UpdateGridSelectedVoxelMesh()
    {
        UpdateGridSelectedVoxelMeshFlag = false;

        _gridSelectedVoxelMesh.Clear();

        if (SelectedFaceCount == 0) return;

        InitializedArray<Vector3> selectedVertices = new InitializedArray<Vector3>(SelectedFaceCount * 4);
        InitializedArray<Vector2> selectedUV = new InitializedArray<Vector2>(SelectedFaceCount * 4);
        InitializedArray<int> selectedTriangles = new InitializedArray<int>(SelectedFaceCount * 6);

        Vector3 vertexPosition;
        int iSelectedTriang = 0;
        foreach (VoxelUnit voxel in Units)
        {
            if (voxel != null && voxel.IsSelected)
            {
                for (int i = 0; i < Direction.Masks.Length; i++)
                {
                    if (voxel.CheckFace(i))
                    {
                        //gridSelectedVoxel
                        for (int j = 0; j < 4; j++)
                        {
                            vertexPosition = VertexChunkManager.GetUnit(VertexPositions[i * 4 + j] + voxel.Position).OffsetPosition.Value;
                            selectedVertices.Init(vertexPosition + (vertexPosition - voxel.Position).Sign() * SceneParameters.VertexConvexity);
                        }

                        for (int j = 0; j < 6; j++)
                            selectedTriangles.Init(VoxelMesh.VoxelFaceTriangles[j] + iSelectedTriang);
                        iSelectedTriang += 4;

                        selectedUV.Init(new Vector2(0, 0));
                        selectedUV.Init(new Vector2(1, 0));
                        selectedUV.Init(new Vector2(1, 1));
                        selectedUV.Init(new Vector2(0, 1));
                    }
                }
            }
        }

        if (SelectedFaceCount == 0) return;
        _gridSelectedVoxelMesh.vertices = selectedVertices.Array;
        _gridSelectedVoxelMesh.triangles = selectedTriangles.Array;
        _gridSelectedVoxelMesh.uv = selectedUV.Array;
        //_gridSelectedVoxelMesh.Optimize();
        _gridSelectedVoxelMesh.RecalculateNormals();


    }

    protected override void OnBeforeDeleteUnit(Vector3Int position)
    {
        FaceCount -= GetUnit(position).FaceCount;
    }

    protected override void OnRelease()
    {
        Object.Destroy(_gridSelectedVoxelGameObject);
        Object.Destroy(_gridVoxelGameObject);
    }

}

