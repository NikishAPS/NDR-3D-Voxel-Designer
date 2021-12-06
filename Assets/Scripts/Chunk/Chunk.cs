using UnityEngine;

public class Chunk
{
    public readonly Vector3Int Position;
    public readonly Vector3Int Size;
    public readonly Vector3Int GlobalPosition;

    public readonly Voxel[] Voxels;

    public int FaceCount { get; private set; }
    public int SelectedFaceCount { get; private set; }
    public int VoxelCount { get; private set; }

    public GameObject ChunkObj { get; private set; }
    public GameObject GridObj { get; private set; }
    public GameObject SelectedGridObj { get; private set; }

    private Mesh _voxelMesh;
    private Mesh _voxelGridMesh;
    private Mesh _selectedVoxelGridMesh;

    private Material _material;
    private Material _gridMaterial;
    private Material _selectedGridMaterial;

    public Chunk(Vector3Int position, Vector3Int size, Material material, Material gridMaterial, Material selectedMaterial)
    {
        Position = position;
        Size = size;
        GlobalPosition = Position * ChunkManager.ChunkSize;

        Voxels = new Voxel[Size.x * Size.y * Size.z];

        _material = material;
        _gridMaterial = gridMaterial;
        _selectedGridMaterial = selectedMaterial;

        CreateMeshes();
    }

    public Chunk(ChunkData chunkData)
    {
        Position = chunkData.Position;
        Size = chunkData.Size;
        GlobalPosition = Position * ChunkManager.ChunkSize;
        FaceCount = chunkData.FaceCount;

        Voxels = new Voxel[Size.x * Size.y * Size.z];
        //SelectedVoxels = new Voxel[Voxels.Length];

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
        Object.Destroy(ChunkObj);
    }

    public Voxel GetVoxel(Vector3Int globalVoxelPos)
    {
        return GetVoxelByGlobalPos(Voxels, globalVoxelPos);
    }

    public bool TryCreateVoxel(int id, Vector3Int globalVoxelPos)
    {
        int index = VoxelatorArray.GetIndex(Size, GetLocalVoxelPos(globalVoxelPos));

        if (index < 0 || Voxels[index] != null) return false;

        Voxels[index] = new Voxel(id, globalVoxelPos);

        UpdateVoxel(Voxels[index]);
        UpdateVoxelsAround(Voxels, globalVoxelPos);

        VoxelCount++;

        return true;
    }

    public bool TryToDeleteVoxel(Vector3Int globalVoxelPos)
    {
        int index = GetVoxelIndexByGlobalPos(globalVoxelPos);

        FaceCount -= Voxels[index].FaceCount;
        if(Voxels[index].Selected) SelectedFaceCount -= Voxels[index].FaceCount;

        Voxels[index].Release();
        Voxels[index] = null;

        UpdateVoxelsAround(Voxels, globalVoxelPos);

        VoxelCount--;

        return true;
    }

    public bool TryToSelectVoxel(Vector3Int globalVoxelPos)
    {
        int index = GetVoxelIndexByGlobalPos(globalVoxelPos);
        if (index < 0 || Voxels[index] == null || Voxels[index].Selected) return false;

        SelectedFaceCount += Voxels[index].FaceCount;
        GetVoxel(globalVoxelPos).Selected = true;

        return true;
    }

    public void ResetVoxelSelection(Vector3Int globaVoxelPos)
    {
        int index = GetVoxelIndexByGlobalPos(globaVoxelPos);

        if (index < 0 || !Voxels[index].Selected) return; 

        SelectedFaceCount -= Voxels[index].FaceCount;
        Voxels[index].Selected = false;
    }

    public void UpdateMesh()
    {
        _voxelMesh.Clear();
        _voxelGridMesh.Clear();

        if (FaceCount == 0) return;

        Vector3[] vertices = new Vector3[FaceCount * 4];
        Vector2[] uv = new Vector2[FaceCount * 4];
        int[] triangles = new int[FaceCount * 6];
        int i4 = 0;
        int i6 = 0;

        float uvX = 0;
        float uvY = 0;
        float offset = SceneParameters.TextureMul / 2f;

        Vector3[] gridVertices = new Vector3[FaceCount * 8];
        int[] gridTriangles = new int[FaceCount * 24];
        int gj = 0;
        int gk = 0;

        foreach (Voxel voxel in Voxels)
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

                        uvX = voxel.UV.x;
                        uvY = voxel.UV.y;

                        uv[i4 + 0] = new Vector2(uvX - offset, uvY + offset);
                        uv[i4 + 2] = new Vector2(uvX - offset, uvY + SceneParameters.TextureMul - offset);
                        uv[i4 + 3] = new Vector2(uvX - SceneParameters.TextureMul + offset, uvY + SceneParameters.TextureMul - offset);
                        uv[i4 + 1] = new Vector2(uvX - SceneParameters.TextureMul + offset, uvY + offset);

                        i4 += 4;
                        i6 += 6;

                        //update grid
                        for (int l = 0; l < 4; l++)
                        {
                            Vector3 vertexOffset = ChunkManager.GetVertex(VoxelMesh.SelectedVoxelVertices[i * 8 + l] + voxel.Position).Offset;

                            gridVertices[gj + l] = (VoxelMesh.SelectedVoxelVertices[i * 8 + l] + vertexOffset) * 1.001f + voxel.Position - Position;
                            gridVertices[gj + l + 4] = (VoxelMesh.SelectedVoxelVertices[i * 8 + l + 4] + vertexOffset) * 1.001f + voxel.Position - Position;
                        }

                        for (int l = 0; l < 24; l++)
                        {
                            gridTriangles[gk + l] = VoxelMesh.SelectedVoxelFaceTriangles[l] + gj;
                        }
                        gj += 8;
                        gk += 24;
                    }
                }
            }
        }

        _voxelMesh.vertices = vertices;
        _voxelMesh.triangles = triangles;
        _voxelMesh.uv = uv;

        _voxelMesh.Optimize();
        _voxelMesh.RecalculateNormals();

        //update grid
        _voxelGridMesh.vertices = gridVertices;
        _voxelGridMesh.triangles = gridTriangles;
        _voxelGridMesh.Optimize();
        _voxelGridMesh.RecalculateNormals();
    }

    public void UpdateSelectedMesh()
    {
        _selectedVoxelGridMesh.Clear();

        if (SelectedFaceCount == 0) return;

        Vector3[] vertices = new Vector3[SelectedFaceCount * 8];
        int[] triangles = new int[SelectedFaceCount * 24];
        int j = 0;
        int k = 0;

        foreach (Voxel voxel in Voxels)
        {
            if (voxel != null && voxel.Selected)
            {
                for (int i = 0; i < voxel.Faces.Length; i++)
                {
                    if(voxel.Faces[i])
                    {
                        for(int l = 0; l < 4; l++)
                        {
                            Vector3 vertexOffset = ChunkManager.GetVertex(VoxelMesh.SelectedVoxelVertices[i * 8 + l] + voxel.Position).Offset;

                            vertices[j + l] = (VoxelMesh.SelectedVoxelVertices[i * 8 + l] + vertexOffset) * 1.001f + voxel.Position - Position;
                            vertices[j + l + 4] = (VoxelMesh.SelectedVoxelVertices[i * 8 + l + 4] + vertexOffset) * 1.001f + voxel.Position - Position;
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

        _selectedVoxelGridMesh.vertices = vertices;
        _selectedVoxelGridMesh.triangles = triangles;

        _selectedVoxelGridMesh.Optimize();
        _selectedVoxelGridMesh.RecalculateNormals();
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
        //voxel
        ChunkObj = new GameObject().Create("Chunk", null, Position, Quaternion.identity);
        _voxelMesh = new Mesh();
        ChunkObj.AddComponent<MeshFilter>().mesh = _voxelMesh;
        ChunkObj.AddComponent<MeshRenderer>().material = _material;

        //grid
        GridObj = new GameObject().Create("Grid", ChunkObj.transform, Position, Quaternion.identity);
        _voxelGridMesh = new Mesh();
        GridObj.AddComponent<MeshFilter>().mesh = _voxelGridMesh;
        GridObj.AddComponent<MeshRenderer>().material = _gridMaterial;

        //selected grid
        SelectedGridObj = new GameObject().Create("Selected Grid", ChunkObj.transform, Position, Quaternion.identity);
        _selectedVoxelGridMesh = new Mesh();
        SelectedGridObj.AddComponent<MeshFilter>().mesh = _selectedVoxelGridMesh;
        SelectedGridObj.AddComponent<MeshRenderer>().material = _selectedGridMaterial;
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
        return (VoxelatorArray.WithinTheArray(arraySize, pos)) ? (pos.x * arraySize.y + pos.y) * arraySize.z + pos.z : -1;
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
        if(voxel.Selected) SelectedFaceCount -= voxel.FaceCount;

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
        if(voxel.Selected) SelectedFaceCount += voxel.FaceCount;
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

}
