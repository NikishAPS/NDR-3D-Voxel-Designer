using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class VoxelChunk : IChunk<VoxelUnit>
{
    public static VertexChunkManager VertexChunkManager { get; set; }
    public static bool GridVoxelActive { get => _gridChunksParent.activeSelf; set => _gridChunksParent.SetActive(value); }

    private static Vector3Int[] VertexPosition = new Vector3Int[]
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

    public int FaceCount { get; set; }
    public int SelectedFaceCount { get; set; }

    private readonly static GameObject _voxelChunksParent;
    private readonly static GameObject _gridChunksParent;
    private readonly static GameObject _gridSelectedChunksParent;
    private readonly GameObject _gridVoxelGameObject;
    private readonly GameObject _gridSelectedVoxelGameObject;
    private readonly Mesh _gridVoxelMesh;
    private readonly Mesh _gridSelectedVoxelMesh;

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

    public void UpdateMesh0()
    {
        _mesh.Clear();
        _gridVoxelMesh.Clear();
        _gridSelectedVoxelMesh.Clear();

        if (FaceCount == 0) return;

        Vector3[] vertices = new Vector3[FaceCount * 4];
        Vector2[] uv = new Vector2[FaceCount * 4];
        int[] triangles = new int[FaceCount * 6];

        Vector2[] gridUV = new Vector2[FaceCount * 4];
        Vector3[] selectedVertices = new Vector3[FaceCount * 4];
        Vector2[] selectedUV = new Vector2[FaceCount * 4];
        int[] selectedTriangles = new int[FaceCount * 6];

        float uvX, uvY;
        float uvOffset = SceneParameters.TextureMul / 2f;

        int
            verticesIterator = 0,
            trianglesIterator = 0,
            uvIterator = 0,
            gridUVIterator = 0,
            gridSelectedVerticesIterator = 0,
            gridSelectedTrianglesIterator = 0,
            gridSelectedUVIterator = 0;

        int iTriang = 0;
        int iSelectedTriang = 0;
        foreach (VoxelUnit voxel in Units)
        {
            if (voxel != null)
            {
                for (int i = 0; i < Direction.Masks.Length; i++)
                {
                    if (voxel.CheckFace(i))
                    {
                        //voxel
                        for (int j = 0; j < 4; j++, verticesIterator++)
                            vertices[verticesIterator] = (VertexChunkManager.GetUnit(VertexPosition[i * 4 + j] + voxel.Position).OffsetPosition);

                        for (int j = 0; j < 6; j++, trianglesIterator++)
                            triangles[trianglesIterator] = (VoxelMesh.VoxelFaceTriangles[j] + iTriang);
                        iTriang += 4;

                        uvX = ((voxel.Id - 1) % SceneParameters.TextureSize + 1) / (float)SceneParameters.TextureSize;
                        uvY = 1 - ((voxel.Id - 1) / (SceneParameters.TextureSize) + 1) / (float)SceneParameters.TextureSize;
                        uv[uvIterator + 0] = (new Vector2(uvX - uvOffset, uvY + uvOffset));
                        uv[uvIterator + 1] = (new Vector2(uvX - uvOffset, uvY + SceneParameters.TextureMul - uvOffset));
                        uv[uvIterator + 2] = (new Vector2(uvX - SceneParameters.TextureMul + uvOffset, uvY + SceneParameters.TextureMul - uvOffset));
                        uv[uvIterator + 3] = (new Vector2(uvX - SceneParameters.TextureMul + uvOffset, uvY + uvOffset));
                        uvIterator += 4;

                        //gridVoxel
                        gridUV[gridUVIterator + 0] = (new Vector2(0, 0));
                        gridUV[gridUVIterator + 1] = (new Vector2(1, 0));
                        gridUV[gridUVIterator + 2] = (new Vector2(1, 1));
                        gridUV[gridUVIterator + 3] = (new Vector2(0, 1));
                        gridUVIterator += 4;

                        //gridSelectedVoxel
                        if (voxel.IsSelected)
                        {
                            for (int j = 0; j < 4; j++, gridSelectedVerticesIterator++)
                                selectedVertices[gridSelectedVerticesIterator] = (VertexChunkManager.GetUnit(VertexPosition[i * 4 + j] + voxel.Position).OffsetPosition);

                            for (int j = 0; j < 6; j++, gridSelectedTrianglesIterator++)
                                selectedTriangles[gridSelectedTrianglesIterator] = (VoxelMesh.VoxelFaceTriangles[j] + iSelectedTriang);
                            iSelectedTriang += 4;

                            selectedUV[gridSelectedUVIterator + 0] = (new Vector2(0, 0));
                            selectedUV[gridSelectedUVIterator + 1] = (new Vector2(1, 0));
                            selectedUV[gridSelectedUVIterator + 2] = (new Vector2(1, 1));
                            selectedUV[gridSelectedUVIterator + 3] = (new Vector2(0, 1));
                            gridSelectedUVIterator += 4;
                        }
                    }
                }
            }
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uv;
        _mesh.Optimize();
        _mesh.RecalculateNormals();

        return;

        _gridVoxelMesh.vertices = vertices;
        _gridVoxelMesh.triangles = triangles;
        _gridVoxelMesh.uv = gridUV;
        _gridVoxelMesh.Optimize();
        _gridVoxelMesh.RecalculateNormals();

        _gridSelectedVoxelMesh.vertices = selectedVertices;
        _gridSelectedVoxelMesh.triangles = selectedTriangles;
        _gridSelectedVoxelMesh.uv = selectedUV;
        _gridSelectedVoxelMesh.Optimize();
        _gridSelectedVoxelMesh.RecalculateNormals();
    }

    private void DDArray()
    {
        MonoBehaviour.print("Array");
        _mesh.Clear();


        if (FaceCount == 0) return;
        MonoBehaviour.print(FaceCount);

        Vector3[] vertices = new Vector3[FaceCount * 4];
        Vector2[] uv = new Vector2[FaceCount * 4];
        int[] triangles = new int[FaceCount * 6];

        return;
        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uv;
        _mesh.Optimize();
        _mesh.RecalculateNormals();
        return;

        float uvX, uvY;
        float uvOffset = SceneParameters.TextureMul / 2f;

        int
            verticesIterator = 0,
            trianglesIterator = 0,
            uvIterator = 0,
            gridUVIterator = 0,
            gridSelectedVerticesIterator = 0,
            gridSelectedTrianglesIterator = 0,
            gridSelectedUVIterator = 0;

        int iTriang = 0;
        foreach (VoxelUnit voxel in Units)
        {
            if (voxel != null)
            {
                for (int i = 0; i < Direction.Masks.Length; i++)
                {
                    if (voxel.CheckFace(i))
                    {
                        //voxel
                        for (int j = 0; j < 4; j++, verticesIterator++)
                            vertices[verticesIterator] = (VertexChunkManager.GetUnit(VertexPosition[i * 4 + j] + voxel.Position).OffsetPosition);

                        for (int j = 0; j < 6; j++, trianglesIterator++)
                            triangles[trianglesIterator] = (VoxelMesh.VoxelFaceTriangles[j] + iTriang);
                        iTriang += 4;

                        uvX = ((voxel.Id - 1) % SceneParameters.TextureSize + 1) / (float)SceneParameters.TextureSize;
                        uvY = 1 - ((voxel.Id - 1) / (SceneParameters.TextureSize) + 1) / (float)SceneParameters.TextureSize;
                        uv[uvIterator + 0] = (new Vector2(uvX - uvOffset, uvY + uvOffset));
                        uv[uvIterator + 1] = (new Vector2(uvX - uvOffset, uvY + SceneParameters.TextureMul - uvOffset));
                        uv[uvIterator + 2] = (new Vector2(uvX - SceneParameters.TextureMul + uvOffset, uvY + SceneParameters.TextureMul - uvOffset));
                        uv[uvIterator + 3] = (new Vector2(uvX - SceneParameters.TextureMul + uvOffset, uvY + uvOffset));
                        uvIterator += 4;
                    }
                }
            }
        }

        return;

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uv;
        _mesh.Optimize();
        _mesh.RecalculateNormals();

    }

    private void DDList()
    {
        MonoBehaviour.print("List");

        _mesh.Clear();

        if (FaceCount == 0) return;

        //работает заебись (менее отдной секунды) с учетом того, что массив пересоздается и копируется несколько раз
        List<Vector3> vertices = new List<Vector3>(FaceCount * 4);
        List<Vector2> uv = new List<Vector2>(FaceCount * 4);
        List<int> triangles = new List<int>(FaceCount * 6);

        float uvX, uvY;
        float uvOffset = SceneParameters.TextureMul / 2f;

        int c = 0;
        int iTriang = 0;
        foreach (VoxelUnit voxel in Units)
        {
            if (voxel != null)
            {
                for (int i = 0; i < Direction.Masks.Length; i++)
                {
                    if (voxel.CheckFace(i))
                    {
                        c++;
                        for (int j = 0; j < 4; j++)
                            vertices.Add(VertexChunkManager.GetUnit(VertexPosition[i * 4 + j] + voxel.Position).OffsetPosition);

                        for (int j = 0; j < 6; j++)
                            triangles.Add(VoxelMesh.VoxelFaceTriangles[j] + iTriang);

                        //vertices.Init(VertexChunkManager.GetUnit(VertexPosition[i * 4 + 0] + voxel.Position).OffsetPosition);
                        //vertices.Init(VertexChunkManager.GetUnit(VertexPosition[i * 4 + 1] + voxel.Position).OffsetPosition);
                        //vertices.Init(VertexChunkManager.GetUnit(VertexPosition[i * 4 + 2] + voxel.Position).OffsetPosition);
                        //vertices.Init(VertexChunkManager.GetUnit(VertexPosition[i * 4 + 3] + voxel.Position).OffsetPosition);


                        //triangles.Init(VoxelMesh.VoxelFaceTriangles[0] + iTriang);
                        //triangles.Init(VoxelMesh.VoxelFaceTriangles[1] + iTriang);
                        //triangles.Init(VoxelMesh.VoxelFaceTriangles[2] + iTriang);
                        //triangles.Init(VoxelMesh.VoxelFaceTriangles[3] + iTriang);
                        //triangles.Init(VoxelMesh.VoxelFaceTriangles[4] + iTriang);
                        //triangles.Init(VoxelMesh.VoxelFaceTriangles[5] + iTriang);

                        iTriang += 4;

                        uvX = ((voxel.Id - 1) % SceneParameters.TextureSize + 1) / (float)SceneParameters.TextureSize;
                        uvY = 1 - ((voxel.Id - 1) / (SceneParameters.TextureSize) + 1) / (float)SceneParameters.TextureSize;

                        uv.Add(new Vector2(uvX - uvOffset, uvY + uvOffset));
                        uv.Add(new Vector2(uvX - uvOffset, uvY + SceneParameters.TextureMul - uvOffset));
                        uv.Add(new Vector2(uvX - SceneParameters.TextureMul + uvOffset, uvY + SceneParameters.TextureMul - uvOffset));
                        uv.Add(new Vector2(uvX - SceneParameters.TextureMul + uvOffset, uvY + uvOffset));
                    }
                }
            }
        }

        _mesh.vertices = vertices.ToArray();
        _mesh.triangles = triangles.ToArray();
        _mesh.uv = uv.ToArray();
        _mesh.Optimize();
        _mesh.RecalculateNormals();
    }

    public override void UpdateMesh()
    {
        _mesh.Clear();
        _gridVoxelMesh.Clear();
        _gridSelectedVoxelMesh.Clear();

        if (FaceCount == 0) return;

        InitializedArray<Vector3> vertices = new InitializedArray<Vector3>(FaceCount * 4);
        InitializedArray<Vector2> uv = new InitializedArray<Vector2>(FaceCount * 4);
        InitializedArray<int> triangles = new InitializedArray<int>(FaceCount * 6);

        InitializedArray<Vector2> gridUV = new InitializedArray<Vector2>(FaceCount * 4);

        InitializedArray<Vector3> selectedVertices = new InitializedArray<Vector3>(SelectedFaceCount * 4);
        InitializedArray<Vector2> selectedUV = new InitializedArray<Vector2>(SelectedFaceCount * 4);
        InitializedArray<int> selectedTriangles = new InitializedArray<int>(SelectedFaceCount * 6);

        float uvX, uvY;
        float uvOffset = SceneParameters.TextureMul / 2f;

        int iTriang = 0;
        int iSelectedTriang = 0;
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
                            vertices.Init(VertexChunkManager.GetUnit(VertexPosition[i * 4 + j] + voxel.Position).OffsetPosition);

                        for (int j = 0; j < 6; j++)
                            triangles.Init(VoxelMesh.VoxelFaceTriangles[j] + iTriang);
                        iTriang += 4;

                        uvX = ((voxel.Id - 1) % SceneParameters.TextureSize + 1) / (float)SceneParameters.TextureSize;
                        uvY = 1 - ((voxel.Id - 1) / (SceneParameters.TextureSize) + 1) / (float)SceneParameters.TextureSize;

                        uv.Init(new Vector2(uvX - uvOffset, uvY + uvOffset));
                        uv.Init(new Vector2(uvX - uvOffset, uvY + SceneParameters.TextureMul - uvOffset));
                        uv.Init(new Vector2(uvX - SceneParameters.TextureMul + uvOffset, uvY + SceneParameters.TextureMul - uvOffset));
                        uv.Init(new Vector2(uvX - SceneParameters.TextureMul + uvOffset, uvY + uvOffset));

                        //gridVoxel
                        gridUV.Init(new Vector2(0, 0));
                        gridUV.Init(new Vector2(1, 0));
                        gridUV.Init(new Vector2(1, 1));
                        gridUV.Init(new Vector2(0, 1));

                        //gridSelectedVoxel
                        if (voxel.IsSelected)
                        {
                            for (int j = 0; j < 4; j++)
                                selectedVertices.Init(VertexChunkManager.GetUnit(VertexPosition[i * 4 + j] + voxel.Position).OffsetPosition);

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
        }

        _mesh.vertices = vertices.Array;
        _mesh.triangles = triangles.Array;
        _mesh.uv = uv.Array;
        //_mesh.Optimize();
        _mesh.RecalculateNormals();

        _gridVoxelMesh.vertices = vertices.Array;
        _gridVoxelMesh.triangles = triangles.Array;
        _gridVoxelMesh.uv = gridUV.Array;
        //_gridVoxelMesh.Optimize();
        _gridVoxelMesh.RecalculateNormals();

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

}

