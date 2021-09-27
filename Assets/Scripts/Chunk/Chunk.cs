using UnityEngine;
using System.Collections;

public class Chunk
{
    public readonly Vector3Int Position;
    public readonly Vector3Int Size;
    public readonly Vector3Int GlobalPosition;

    public readonly Voxel[] Voxels;
    public readonly Voxel[] SelectedVoxels;

    public int FaceCount { get; private set; }
    public int SelectedFaceCount { get; private set; }

    private int _voxelCount;

    private GameObject _chunk;
    private GameObject _selectedChunk;
    private Mesh _mesh;
    private Mesh _selectedMesh;

    public Chunk(Vector3Int position, Vector3Int size)
    {
        Position = position;
        Size = size;
        GlobalPosition = Position * ChunkManager.ChunkSize;

        Voxels = new Voxel[Size.x * Size.y * Size.z];
        SelectedVoxels = new Voxel[Voxels.Length];

        CreateMeshes();
    }

    public Chunk(ChunkData chunkData)
    {
        Position = chunkData.Position;
        Size = chunkData.Size;
        GlobalPosition = Position * ChunkManager.ChunkSize;
        FaceCount = chunkData.FaceCount;

        Voxels = new Voxel[Size.x * Size.y * Size.z];
        SelectedVoxels = new Voxel[Voxels.Length];

        CreateMeshes();

        for(int i = 0; i < Voxels.Length; i++)
        {
            if (chunkData.VoxelsData[i].Id > 0)
                Voxels[i] = new Voxel(chunkData.VoxelsData[i]);
            else
                Voxels[i] = null;
        }
    }

    ~Chunk()
    {
        //MonoBehaviour.print("Xyu");
        //Object.Destroy(_chunk);
    }

    public void Release()
    {
        Object.Destroy(_chunk);
    }

    public Voxel GetVoxel(Vector3Int globalVoxelPos)
    {
        return GetVoxelByGlobalPos(Voxels, globalVoxelPos);
    }

    public Voxel GetSelectedVoxel(Vector3Int globalVoxelPos)
    {
        return GetVoxelByGlobalPos(SelectedVoxels, globalVoxelPos);
    }

    public bool TryCreateVoxel(int id, Vector3Int globalVoxelPos)
    {
        int index = VoxelatorManager.GetIndex(Size, GetLocalVoxelPos(globalVoxelPos));

        if (index < 0 || Voxels[index] != null) return false;

        Voxels[index] = new Voxel(id, globalVoxelPos);

        UpdateVoxel(Voxels[index]);
        UpdateVoxelsAround(Voxels, globalVoxelPos);

        _voxelCount++;

        return true;
    }

    public bool TryToDeleteVoxel(Vector3Int globalVoxelPos)
    {
        int index = GetVoxelIndexByGlobalPos(globalVoxelPos);

        FaceCount -= Voxels[index].FaceCount;
        SelectedFaceCount -= SelectedVoxels[index].FaceCount;

        Voxels[index] = null;
        SelectedVoxels[index] = null;

        UpdateVoxelsAround(Voxels, globalVoxelPos);

        _voxelCount--;

        return true;
    }

    public bool TryToSelectVoxel(Vector3Int globalVoxelPos)
    {
        int index = GetVoxelIndexByGlobalPos(globalVoxelPos);
        if (index < 0 || Voxels[index] == null || SelectedVoxels[index] != null) return false;

        SelectedVoxels[index] = new Voxel(Voxels[index]);
        SelectedFaceCount += SelectedVoxels[index].FaceCount;

        return true;
    }

    public void ResetVoxelSelection(Vector3Int globaVoxelPos)
    {
        int index = GetVoxelIndexByGlobalPos(globaVoxelPos);

        if (index < 0 || SelectedVoxels[index] == null) return; 

        SelectedFaceCount -= SelectedVoxels[index].FaceCount;
        SelectedVoxels[index] = null;
    }

    public void SetActiveSelectedMesh(bool active)
    {
        _chunk.transform.GetChild(0).gameObject.SetActive(active);
    }

    public void UpdateMesh()
    {
        _mesh.Clear();

        if (FaceCount == 0) return;

        Vector3[] vertices = new Vector3[FaceCount * 4];
        Vector2[] uv = new Vector2[FaceCount * 4];
        int[] triangles = new int[FaceCount * 6];
        int i4 = 0;
        int i6 = 0;

        foreach(Voxel voxel in Voxels)
        {
            if(voxel != null)
            {
                for(int i = 0; i < voxel.Faces.Length; i++)
                {
                    if (voxel.Faces[i])
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            vertices[i4 + j] = ChunkManager.GetVertex(VoxelMesh.VoxelVertices[i * 4 + j] + voxel.Position).Position - Position;
                        }

                        for(int j = 0; j < 6; j++)
                        {
                            triangles[i6 + j] = VoxelMesh.VoxelFaceTriangles[j] + i4;
                        }

                        float v1 = SceneData.TextureMul * ((voxel.Id - 1) % SceneData.TextureSize) + SceneData.TextureMul;
                        float v2 = 1 - SceneData.TextureMul * (int)(voxel.Id / (SceneData.TextureSize + 1));
                        float offset = SceneData.TextureMul / 2f;

                        uv[i4 + 0] = new Vector2(v1 - offset, v2 - SceneData.TextureMul + offset);
                        uv[i4 + 1] = new Vector2(v1 - offset, v2 - offset);
                        uv[i4 + 2] = new Vector2(v1 - SceneData.TextureMul + offset, v2 - offset);
                        uv[i4 + 3] = new Vector2(v1 - SceneData.TextureMul + offset, v2 - SceneData.TextureMul + offset);


                        float x = (voxel.Id - 1) % SceneData.TextureSize + 1;
                        float y = (voxel.Id - 1) / (SceneData.TextureSize) + 1;


                        x = x / SceneData.TextureSize;
                        y = 1 - y / SceneData.TextureSize;

                        uv[i4 + 0] = new Vector2(x - offset, y + offset);
                        uv[i4 + 2] = new Vector2(x - offset, y + SceneData.TextureMul - offset);
                        uv[i4 + 3] = new Vector2(x - SceneData.TextureMul + offset, y + SceneData.TextureMul - offset);
                        uv[i4 + 1] = new Vector2(x - SceneData.TextureMul + offset, y + offset);

                        i4 += 4;
                        i6 += 6;
                    }
                }
            }
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uv;

        _mesh.Optimize();
        _mesh.RecalculateNormals();
    }

    public void UpdateSelectedMesh()
    {
        _selectedMesh.Clear();

        if (SelectedFaceCount == 0) return;

        Vector3[] vertices = new Vector3[SelectedFaceCount * 8];
        int[] triangles = new int[SelectedFaceCount * 24];
        int j = 0;
        int k = 0;

        foreach (Voxel selectedVoxel in SelectedVoxels)
        {
            if(selectedVoxel != null)
            {
                for (int i = 0; i < selectedVoxel.Faces.Length; i++)
                {
                    if(selectedVoxel.Faces[i])
                    {
                        for(int l = 0; l < 4; l++)
                        {
                            Vector3 vertexOffset = ChunkManager.GetVertex(VoxelMesh.SelectedVoxelVertices[i * 8 + l] + selectedVoxel.Position).GetOffset();

                            vertices[j + l] = (VoxelMesh.SelectedVoxelVertices[i * 8 + l] + vertexOffset) * 1.001f + selectedVoxel.Position - Position;
                            vertices[j + l + 4] = (VoxelMesh.SelectedVoxelVertices[i * 8 + l + 4] + vertexOffset) * 1.001f + selectedVoxel.Position - Position;
                        }

                        for (int l = 0; l < 24; l++)
                        {
                            triangles[k + l] = VoxelMesh.SelectedVoxelFaceTriangles[l] + j;
                        }

                        j += 8;
                        k += 24;
                    }
                }
            }
        }

        _selectedMesh.vertices = vertices;
        _selectedMesh.triangles = triangles;

        _selectedMesh.Optimize();
        _selectedMesh.RecalculateNormals();
    }

    public ChunkData GetData()
    {
        VoxelData[] voxelsData = new VoxelData[Voxels.Length];
        for (int i = 0; i < Voxels.Length; i++)
        {
            if (Voxels[i] != null)
                voxelsData[i] = Voxels[i].GetData();
            else
                voxelsData[i] = new Voxel(-1, Vector3Int.zero).GetData();
        }
        return new ChunkData(Position, Size, FaceCount, voxelsData);
    }

    private void CreateMeshes()
    {
        _chunk = new GameObject().Create("Chunk", null, Position, Quaternion.identity);
        _selectedChunk = new GameObject().Create("Selected Chunk", _chunk.transform, Position, Quaternion.identity);

        _mesh = new Mesh();
        _chunk.AddComponent<MeshFilter>().mesh = _mesh;
        _chunk.AddComponent<MeshRenderer>().material = ChunkManager.ChunkMaterial;
        _selectedMesh = new Mesh();
        _selectedChunk.AddComponent<MeshFilter>().mesh = _selectedMesh;
        _selectedChunk.AddComponent<MeshRenderer>().material = ChunkManager.SelectedChunkMaterial;
    }

    private bool InArray(Vector3 arraySize, Vector3 point)
    {
        return
            point.x >= 0 && point.x < arraySize.x &&
            point.y >= 0 && point.y < arraySize.y &&
            point.z >= 0 && point.z < arraySize.z;
    }

    //get position
    private Vector3Int GetLocalVoxelPos(Vector3Int globalVoxelPos)
    {
        return globalVoxelPos - GlobalPosition;
    }
    private Vector3Int GetGlobalVoxelPos(Vector3Int localVoxelPos)
    {
        return localVoxelPos + GlobalPosition;
    }

    //get index
    private int GetIndex(Vector3Int arraySize, Vector3Int pos)
    {
        return (InArray(arraySize, pos)) ? (pos.x * arraySize.y + pos.y) * arraySize.z + pos.z : -1;
    }
    private int GetVoxelIndexByLocalPos(Vector3Int localVoxelPos)
    {
        return GetIndex(Size, localVoxelPos);
    }
    private int GetVoxelIndexByGlobalPos(Vector3Int globalVoxelPos)
    {
        return GetIndex(Size, GetLocalVoxelPos(globalVoxelPos));
    }

    //get voxel
    private Voxel GetVoxelByLocalPos(Voxel[] voxels, Vector3Int localVoxelPos)
    {
        int index = GetIndex(Size, localVoxelPos);
        return (index < 0) ? null : voxels[index];
    }
    private Voxel GetVoxelByGlobalPos(Voxel[] voxels, Vector3Int globalVoxelPos)
    {
        return GetVoxelByLocalPos(voxels, GetLocalVoxelPos(globalVoxelPos));
    }

    //get vertex

    //updates
    private void UpdateVoxel(Voxel voxel)
    {
        if (voxel == null) return;

        FaceCount -= voxel.FaceCount;

        for(int i = 0; i < Direction.Directions.Length; i++)
        {
            if (ChunkManager.GetVoxel(voxel.Position + Direction.Directions[i]) == null) voxel.SetFace(i, true);
            else voxel.SetFace(i, false);
        }

        //if (ChunksManager.GetVoxel(voxel.Position + Vector3Int.left) == null) voxel.SetFace(Direction.Left, true);
        //else voxel.SetFace(Direction.Left, false);

        //if (ChunksManager.GetVoxel(voxel.Position + Vector3Int.right) == null) voxel.SetFace(Direction.Right, true);
        //else voxel.SetFace(Direction.Right, false);

        //if (ChunksManager.GetVoxel(voxel.Position + Vector3Int.down) == null) voxel.SetFace(Direction.Down, true);
        //else voxel.SetFace(Direction.Down, false);

        //if (ChunksManager.GetVoxel(voxel.Position + Vector3Int.up) == null) voxel.SetFace(Direction.Up, true);
        //else voxel.SetFace(Direction.Up, false);

        //if (ChunksManager.GetVoxel(voxel.Position + new Vector3Int().Back()) == null) voxel.SetFace(Direction.Back, true);
        //else voxel.SetFace(Direction.Back, false);

        //if (ChunksManager.GetVoxel(voxel.Position + new Vector3Int().Forward()) == null) voxel.SetFace(Direction.Forward, true);
        //else voxel.SetFace(Direction.Forward, false);

        FaceCount += voxel.FaceCount;
    }
    private void UpdateVoxelsAround(Voxel[] voxels, Vector3Int globalVoxelPos)
    {
        UpdateVoxel(GetVoxelByGlobalPos(voxels, globalVoxelPos + Vector3Int.left));
        UpdateVoxel(GetVoxelByGlobalPos(voxels, globalVoxelPos + Vector3Int.right));
        UpdateVoxel(GetVoxelByGlobalPos(voxels, globalVoxelPos + Vector3Int.down));
        UpdateVoxel(GetVoxelByGlobalPos(voxels, globalVoxelPos + Vector3Int.up));
        UpdateVoxel(GetVoxelByGlobalPos(voxels, globalVoxelPos + new Vector3Int().Back()));
        UpdateVoxel(GetVoxelByGlobalPos(voxels, globalVoxelPos + new Vector3Int().Forward()));
    }
    private void UpdateSelectedVoxel(Voxel selectedVoxel)
    {
        if (selectedVoxel == null) return;

        Voxel voxel = GetVoxelByGlobalPos(Voxels, selectedVoxel.Position);

        SelectedFaceCount -= selectedVoxel.FaceCount;
        selectedVoxel = new Voxel(GetVoxelByGlobalPos(Voxels, selectedVoxel.Position));
        SelectedFaceCount += selectedVoxel.FaceCount;
    }

    //create
    //private void CreateVertex(Vector3 globalVertexPos)
    //{
    //    int index = GetVertexIndexByGlobalPos(globalVertexPos);

    //    if (index < 0 || Vertices[index] != null) return;

    //    Vertices[index] = new Vertex(globalVertexPos);
    //}
    //private void CreateVertices(Vector3Int globalVoxelPos)
    //{
    //    CreateVertex(globalVoxelPos + new Vector3(-0.5f, -0.5f, -0.5f));
    //    CreateVertex(globalVoxelPos + new Vector3(+0.5f, -0.5f, -0.5f));
    //    CreateVertex(globalVoxelPos + new Vector3(+0.5f, +0.5f, -0.5f));
    //    CreateVertex(globalVoxelPos + new Vector3(-0.5f, +0.5f, -0.5f));

    //    CreateVertex(globalVoxelPos + new Vector3(-0.5f, -0.5f, +0.5f));
    //    CreateVertex(globalVoxelPos + new Vector3(+0.5f, -0.5f, +0.5f));
    //    CreateVertex(globalVoxelPos + new Vector3(+0.5f, +0.5f, +0.5f));
    //    CreateVertex(globalVoxelPos + new Vector3(-0.5f, +0.5f, +0.5f));
    //}

}
