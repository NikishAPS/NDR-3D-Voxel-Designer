using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkManager
{
    public static int VoxelId { get; set; }
    public static int IncrementOption { get; private set; }
    public static Vector3Int FieldSize { get; private set; }
    public static Vector3Int VerticesArraySize { get; private set; }
    public static Vector3 Center => FieldSize.ToVector3() * 0.5f;

    public static Vector3Bool Mirror
    {
        get => _mirror;
        set
        {
            _reflector.Mirror = value;
            UpdateReflection(value);
        }
    }

    public static Chunk[] Chunks { get; private set; }
    public static Vertex[] Vertices { get; private set; }
    public static VoxelChunk[] VoxelChunks { get; private set; }
    //public static GridSelectedChunk[] GridSelectedChunks { get; private set; }
    public static VertexChunk[] VertexChunks { get; private set; }

    public static readonly Vector3Int ChunkSize = SceneParameters.ChunkSize;

    public static Vertex SelectedVertex { get; set; }
    public static Vector3Int[] SelectedVoxelPositions => _selectedVoxelPositions.ToArray();

    public static Vector3 MiddleSelectedPos { get; private set; }
    public static int SelectedVoxelCount => _selectedVoxelPositions.Count;


    public static bool VoxelGridActivity { set { foreach (Chunk chunk in Chunks) chunk.GridObj.SetActive(value); } }
    public static bool SelectedVoxelGridActivity { set { foreach (Chunk chunk in Chunks) chunk.SelectedGridObj.SetActive(value); } }
    public static bool VertexActivity { set => _vertexMeshParent.SetActive(value); }

    private static GameObject _vertexMeshParent;
    private static GameObject[] _vertexMeshes;

    private static Vector3Bool _mirror;
    private static Vector3Int _chunksCount;
    private static List<Chunk> _nonUpdatedChunks = new List<Chunk>();
    private static LinkedList<Vector3Int> _selectedVoxelPositions = new LinkedList<Vector3Int>();
    private static Reflector _reflector;

    private static Material _chunkMaterial;
    private static Material _chunkGridMaterial;
    private static Material _chunkSelectedGridMaterial;
    private static Material _vertexMaterial;

    public static void Create()
    {
        /*
         if(CreateVoxel())
         {
            if(CreateVertex())
            {
                UpdateVertexChunk();
            }

        UpdateVoxelChunk();
        }



        UpdateChunks()
        {
            Go(UpdateChunk);
            Go(UpdateGrid);
            Go(UpdateVertices);

            
        }
         */
    }

    static ChunkManager()
    {
        _chunkMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/Chunk");
        _chunkGridMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/Grid");
        _chunkSelectedGridMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/SelectedGrid");
        _vertexMaterial = ResourcesLoader.Load<Material>("Materials/Vertex");

    }

    public static void Init(Vector3Int fieldSize, int incrementOption)
    {
        FieldSize = fieldSize;
        VerticesArraySize = FieldSize + Vector3Int.one;

        IncrementOption = incrementOption;

        CreateChunks();

        int count;
        Vector3Int[] positions;
        Vector3Int[] sizes;

        GetChunkParameters(ChunkSize, out count, out positions, out sizes);
    }

    public static void SetVoxelIdByColor(Color color)
    {
        int r = (int)(color.r * 255);
        int g = (int)(color.g * 255);
        int b = (int)(color.b * 255);

        int index = VoxelatorArray.GetIndex(Vector3Int.one * 256, new Vector3Int(r, g, b)) + 1;

        VoxelId = index;
    }

    public static void InitField(Vector3Int size)
    {
        FieldSize = size;
        VerticesArraySize = FieldSize + Vector3Int.one;
    }


    public static void Release()
    {
        for (int i = 0; i < Chunks.Length; i++)
        {
            Chunks[i].Release();// = null; //Вызов деструктора 
        }

        Chunks = null;
    }

    public static bool InField(Vector3 position)
    {
        return VoxelatorArray.WithinTheArray(FieldSize, position);
    }

    public static Chunk GetChunk(Vector3 position)
    {
        return InField(position.ToVector3Int()) ? Chunks[VoxelatorArray.GetIndex(_chunksCount, GetChunkPos(position))] : null;
    }

    public static Voxel GetVoxel(Vector3Int globalVoxelPos)
    {
        return InField(globalVoxelPos) ? GetChunk(globalVoxelPos).GetVoxel(globalVoxelPos) : null;
    }

    public static Vertex GetSelectedVertex()
    {
        return SelectedVertex;
    }

    public static Vertex GetVertex(Vector3 pivotVertexPos)
    {
        int index = GetVertexIndex(pivotVertexPos);
        return index < 0 ? null : Vertices[index];
    }

    public static void CreateVoxel(Vector3Int globalVoxelPos)
    {
        Chunk chunk = GetChunk(globalVoxelPos);

        if (chunk != null)
        {
            if (chunk.TryCreateVoxel(VoxelId, globalVoxelPos))
            {
                UpdateChunksAround(globalVoxelPos);
            }
        }

    }

    public static void DeleteSelectedVoxels()
    {
        if (_selectedVoxelPositions.Count == 0) return;

        foreach (Vector3Int selectedVoxelPosition in _selectedVoxelPositions)
        {
            //DeleteVoxel(selectedVoxelPosition);
            TryDeleteVoxel(selectedVoxelPosition);
        }
        _selectedVoxelPositions.Clear();
        MiddleSelectedPos = Vector3.zero;

        UpdateAllChunkMeshes();
    }

    public static void SelectVoxel(Vector3Int globalVoxelPos)
    {
        Chunk chunk = GetChunk(globalVoxelPos);
        if (chunk == null) return;

        if (chunk.TryToSelectVoxel(globalVoxelPos))
        {
            UpdateChunk(chunk);
            UpdateChunkSelectedMeshes();
            MiddleSelectedPos = (MiddleSelectedPos * _selectedVoxelPositions.Count);
            _selectedVoxelPositions.AddLast(globalVoxelPos);
            MiddleSelectedPos = (MiddleSelectedPos + globalVoxelPos) / _selectedVoxelPositions.Count;
        }
    }

    public static void ResetVoxelSelection()
    {
        foreach (Vector3Int _selectedVoxelPosition in _selectedVoxelPositions)
        {
            Chunk chunk = GetChunk(_selectedVoxelPosition);
            if (chunk != null)
            {
                chunk.ResetVoxelSelection(_selectedVoxelPosition);
                UpdateChunksAround(_selectedVoxelPosition);
            }
        }

        MiddleSelectedPos = Vector3.zero;
        _selectedVoxelPositions.Clear();
        UpdateChunkSelectedMeshes();
    }

    public static bool MoveSelectedVoxels(DragTransform dragValue)
    {
        if (_selectedVoxelPositions.Count == 0) return false;

        Vector3Int roundedOffset = dragValue.Position.RoundToInt();
        if (roundedOffset == Vector3Int.zero) return false;

        //checking limits
        foreach (Vector3Int selectedVoxelPosition in _selectedVoxelPositions)
        {
            if (!InField(selectedVoxelPosition + roundedOffset) ||
                GetVoxel(selectedVoxelPosition + roundedOffset) != null &&
                //GetSelectedVoxel(selectedVoxelPosition + roundedOffset) == null) return false;
                //!GetSelectedVoxelStatus(selectedVoxelPosition + roundedOffset)) return false;
                !GetVoxel(selectedVoxelPosition + roundedOffset).Selected) return false;
        }

        //copying voxels
        Voxel[] voxels = new Voxel[_selectedVoxelPositions.Count];
        Vertex[] vertices = new Vertex[voxels.Length * 8];
        {
            int i = 0;
            foreach (Vector3Int selectedVoxelPosition in _selectedVoxelPositions)
            {
                //voxels[i] = new Voxel(GetVoxel(selectedVoxelPosition));
                voxels[i] = GetVoxel(selectedVoxelPosition);
                Vector3[] vertexPositions = GetVertexPositions(selectedVoxelPosition);

                int j = 0;
                foreach (Vector3 vertexPosition in vertexPositions)
                {
                    vertices[i * 8 + j] = new Vertex(GetVertex(vertexPosition));
                    j++;
                }

                i++;
            }
        }

        //deleting voxels
        foreach (Vector3Int selectedVoxelPosition in _selectedVoxelPositions)
        {
            //DeleteVoxel(selectedVoxelPosition);
            TryDeleteVoxel(selectedVoxelPosition);
        }
        _selectedVoxelPositions.Clear();

        //creating voxels
        for (int i = 0; i < voxels.Length; i++)
        {
            Chunk chunk = GetChunk(voxels[i].Position);
            //if (chunk.TryCreateVoxel(voxels[i].Id, voxels[i].Position + roundedOffset))
            if (TryCreateVoxel(voxels[i].Id, voxels[i].Position + roundedOffset))
            {
                if (chunk.TryToSelectVoxel(voxels[i].Position + roundedOffset))
                {
                    _selectedVoxelPositions.AddLast(voxels[i].Position + roundedOffset);
                    UpdateChunksAround(voxels[i].Position);

                    int j = 0;
                    foreach (Vector3 vertexPosition in GetVertexPositions(voxels[i].Position + roundedOffset))
                    {
                        int index = GetVertexIndex(vertexPosition);

                        int adjacentVoxelsCount = Vertices[index] != null ? Vertices[index].AdjacentVoxelsCount + 1 : 1;

                        Vertices[index] = new Vertex(vertices[i * 8 + j].PivotPosition + roundedOffset, vertices[i * 8 + j].Offset);
                        Vertices[index].AdjacentVoxelsCount = adjacentVoxelsCount;

                        Vector3[] reflectedVertexPivotPositions = _reflector.GetReflectedPositions(Vertices[index].PivotPosition);
                        Vector3[] reflectedVertexPositions = _reflector.GetReflectedPositions(Vertices[index].Position);
                        for(int k = 0; k < reflectedVertexPivotPositions.Length; k++)
                            GetVertex(reflectedVertexPivotPositions[k]).Position = reflectedVertexPositions[k];

                        j++;
                    }

                }
            }

        }

        //SceneData.DragSystem.OffsetPosition(roundedOffset);

        MiddleSelectedPos += roundedOffset;
        UpdateAllChunkMeshes();

        dragValue.Position = roundedOffset;
        return true;
    }

    public static bool TryMoveVertex(DragTransform dragValue)
    {
        Vector3 offset = RoundVertexPointPos(dragValue.Position);

        if (SelectedVertex == null || offset == Vector3.zero) return false;

        Vector3 vertexOffset = SelectedVertex.Offset;
        Vector3 newVertexPos = offset + vertexOffset;

        if (newVertexPos.x < -1.5) offset.x = -vertexOffset.x - 1.5f; else if (newVertexPos.x > 1.5f) offset.x = 1.5f - vertexOffset.x;
        if (newVertexPos.y < -1.5) offset.y = -vertexOffset.y - 1.5f; else if (newVertexPos.y > 1.5f) offset.y = 1.5f - vertexOffset.y;
        if (newVertexPos.z < -1.5) offset.z = -vertexOffset.z - 1.5f; else if (newVertexPos.z > 1.5f) offset.z = 1.5f - vertexOffset.z;

        SelectedVertex.Shift(offset);
        UpdateChunksAround(SelectedVertex.PivotPosition.ToVector3Int());

        Vector3[] reflectedPivotPositions = _reflector.GetReflectedPositions(SelectedVertex.PivotPosition);
        Vector3[] reflectedPositions = _reflector.GetReflectedPositions(SelectedVertex.Position);
        for(int i = 0; i < reflectedPivotPositions.Length; i++)
        {
            GetVertex(reflectedPivotPositions[i]).Position = reflectedPositions[i];
            UpdateChunksAround(reflectedPositions[i].ToVector3Int());
        }

        Presenter.EditVertex();

        UpdateAllChunkMeshes();

        dragValue.Position = offset;
        return true;
    }

    public static void SetVertexPosition(Vector3 pivotVertexPosition, Vector3 position)
    {
        Vertex vertex = GetVertex(pivotVertexPosition);
        if (vertex == null) return;

        vertex.Position = position;
        ClampVertex(vertex);
    }

    public static void SetVertexPositionWithUpdateChunks(Vector3 pivotVertexPosition, Vector3 position)
    {
        SetVertexPosition(pivotVertexPosition, position);

        UpdateChunksAround(pivotVertexPosition.ToVector3Int());
        UpdateAllChunkMeshes();
    }

    public static Vector3 RoundVertexPointPos(Vector3 pos)
    {
        return ((pos + Vector3.one * 0.5f) * IncrementOption).RoundToFloat() / IncrementOption - Vector3.one * 0.5f;
    }

    public static ChunkManagerData GetData()
    {
        ChunkData[] chunksData = new ChunkData[Chunks.Length];
        for (int i = 0; i < chunksData.Length; i++)
        {
            chunksData[i] = Chunks[i].GetData();
        }
        VertexData[] verticesData = new VertexData[Vertices.Length];
        for (int i = 0; i < verticesData.Length; i++)
        {
            if (Vertices[i] != null)
                verticesData[i] = Vertices[i].GetData();
            else
                verticesData[i] = new Vertex(Vector3.one * -1).GetData();
        }
        return new ChunkManagerData(IncrementOption, FieldSize, _chunksCount, verticesData, chunksData);
    }

    public static void SetData(ChunkManagerData chunksManagerData)
    {
        if (Chunks != null)
            Release();

        IncrementOption = chunksManagerData.IncrementOption;
        InitField(chunksManagerData.FieldSize);
        _chunksCount = chunksManagerData.ChunkSizes;

        Chunks = new Chunk[chunksManagerData.ChunksData.Length];
        for (int i = 0; i < Chunks.Length; i++)
        {
            Chunks[i] = new Chunk(chunksManagerData.ChunksData[i]);
        }
        Vertices = new Vertex[chunksManagerData.VerticesData.Length];
        for (int i = 0; i < Vertices.Length; i++)
        {
            if (chunksManagerData.VerticesData[i].PivotPosition != Vector3.one * -1)
                Vertices[i] = new Vertex(chunksManagerData.VerticesData[i]);
            else
                Vertices[i] = null;
        }

        _nonUpdatedChunks = Chunks.ToList();
        UpdateChunkMeshes();

        //GridManager._grid[Direction.Down].Size = new Vector3Int(ChunkManager.FieldSize.x, 1, ChunkManager.FieldSize.z);
        //GridManager._grid[Direction.Down].Active = true;
    }

    public static bool TryCreateVoxel(Vector3Int globalVoxelPos)
    {
        return TryCreateVoxel(VoxelId, globalVoxelPos);
    }

    public static bool TryCreateVoxel(int voxelId, Vector3Int globalVoxelPosition)
    {
        if (TryCreateVoxelWithoutReflection(voxelId, globalVoxelPosition))
        {
            Voxel tailVoxel = Voxel.Head;
            if (_mirror.X) for (Voxel voxel = tailVoxel; voxel != null; voxel = voxel.Next) TryReflectVoxelByX(voxel);
            if (_mirror.Y) for (Voxel voxel = tailVoxel; voxel != null; voxel = voxel.Next) TryReflectVoxelByY(voxel);
            if (_mirror.Z) for (Voxel voxel = tailVoxel; voxel != null; voxel = voxel.Next) TryReflectVoxelByZ(voxel);

            return true;
        }

        return false;
    }

    private static void GetChunkParameters(Vector3Int chunkSize, out int count, out Vector3Int[] positions, out Vector3Int[] sizes)
    {
        _chunksCount = new Vector3Int(
             FieldSize.x % chunkSize.x != 0 ? (int)(FieldSize.x / chunkSize.x) + 1 : FieldSize.x / chunkSize.x,
             FieldSize.y % chunkSize.y != 0 ? (int)(FieldSize.y / chunkSize.y) + 1 : FieldSize.y / chunkSize.y,
             FieldSize.z % chunkSize.z != 0 ? (int)(FieldSize.z / chunkSize.z) + 1 : FieldSize.z / chunkSize.z
             );

        count = _chunksCount.x * _chunksCount.y * _chunksCount.z;
        positions = new Vector3Int[count];
        sizes = new Vector3Int[count];

        for (int x = 0; x < _chunksCount.x; x++)
        {
            for (int y = 0; y < _chunksCount.y; y++)
            {
                for (int z = 0; z < _chunksCount.z; z++)
                {
                    Vector3Int currentChunkPosition = new Vector3Int(x, y, z);

                    Vector3Int currentchunkSize = new Vector3Int(
                        x + 1 == _chunksCount.x ? FieldSize.x - x * chunkSize.x : chunkSize.x,
                        y + 1 == _chunksCount.y ? FieldSize.y - y * chunkSize.y : chunkSize.y,
                        z + 1 == _chunksCount.z ? FieldSize.z - z * chunkSize.z : chunkSize.z
                        );

                        new Chunk(currentChunkPosition, currentchunkSize, _chunkMaterial, _chunkGridMaterial, _chunkSelectedGridMaterial);

                    int index = (x * _chunksCount.y + y) * _chunksCount.z + z;

                    positions[index] = currentChunkPosition;
                    sizes[index] = currentchunkSize;

                }
            }
        }
    }

    private static void CreateChunksTest()
    {
        return;

        Vector3Int chunkCount, chunkSize;
        int arraySize;

        //voxelChunk
        chunkSize = SceneParameters.VoxelChunkSize;
        chunkCount = GetChunkCount(chunkSize);
        arraySize = VoxelatorArray.GetFlatArraySize(chunkCount);
        VoxelChunks = new VoxelChunk[arraySize];
        for (int i = 0; i < arraySize; i++)
            VoxelChunks[i] = new VoxelChunk(chunkSize * i, chunkSize, _chunkMaterial, "", null, null);

        ////GridSelectedChunk
        //chunkSize = SceneParameters.GridSelectedChunkSize;
        //chunkCount = GetChunkCount(chunkSize);
        //arraySize = Voxelator.GetFlatArraySize(chunkCount);
        //GridSelectedChunks = new GridSelectedChunk[arraySize];
        //for (int i = 0; i < arraySize; i++)
        //    GridSelectedChunks[i] = new GridSelectedChunk(chunkSize * i, chunkSize, _chunkSelectedGridMaterial);

        //VertexChunk
        chunkSize = SceneParameters.VertexChunkSize;
        chunkCount = GetChunkCount(chunkSize);
        arraySize = VoxelatorArray.GetFlatArraySize(chunkCount);
        VertexChunks = new VertexChunk[arraySize];
        for (int i = 0; i < arraySize; i++)
            VertexChunks[i] = new VertexChunk(chunkSize * i, chunkSize, _vertexMaterial, "");
    }

    private static void CreateChunks()
    {
        VoxelId = SceneParameters.TextureSize * SceneParameters.TextureSize;
        MiddleSelectedPos = Vector3.zero;

        _chunksCount = new Vector3Int(
             FieldSize.x % ChunkSize.x != 0 ? (int)(FieldSize.x / ChunkSize.x) + 1 : FieldSize.x / ChunkSize.x,
             FieldSize.y % ChunkSize.y != 0 ? (int)(FieldSize.y / ChunkSize.y) + 1 : FieldSize.y / ChunkSize.y,
             FieldSize.z % ChunkSize.z != 0 ? (int)(FieldSize.z / ChunkSize.z) + 1 : FieldSize.z / ChunkSize.z
             );

        Chunks = new Chunk[_chunksCount.x * _chunksCount.y * _chunksCount.z];
        Vertices = new Vertex[VerticesArraySize.x * VerticesArraySize.y * VerticesArraySize.z];
        _reflector = new Reflector(FieldSize);

        for (int x = 0; x < _chunksCount.x; x++)
        {
            for (int y = 0; y < _chunksCount.y; y++)
            {
                for (int z = 0; z < _chunksCount.z; z++)
                {
                    Vector3Int chunkPos = new Vector3Int(x, y, z);

                    Vector3Int chunkSize = new Vector3Int(
                        x + 1 == _chunksCount.x ? FieldSize.x - x * ChunkSize.x : ChunkSize.x,
                        y + 1 == _chunksCount.y ? FieldSize.y - y * ChunkSize.y : ChunkSize.y,
                        z + 1 == _chunksCount.z ? FieldSize.z - z * ChunkSize.z : ChunkSize.z
                        );

                    Chunks[(x * _chunksCount.y + y) * _chunksCount.z + z] =
                        new Chunk(chunkPos, chunkSize, _chunkMaterial, _chunkGridMaterial, _chunkSelectedGridMaterial);
                }
            }
        }

        

        //////////////_vertexMeshParent = new GameObject().Create("Vertex", null, Vector3.zero, Quaternion.identity);

    }

    private static Vector3Int GetChunkCount(Vector3Int chunkSize)
    {
        return new Vector3Int(
            FieldSize.x % chunkSize.x == 0 ? FieldSize.x / chunkSize.x : FieldSize.x / chunkSize.x + 1,
            FieldSize.y % chunkSize.y == 0 ? FieldSize.y / chunkSize.y : FieldSize.y / chunkSize.y + 1,
            FieldSize.z % chunkSize.z == 0 ? FieldSize.z / chunkSize.z : FieldSize.z / chunkSize.z + 1
            );
    }

    private static Vector3Int GetLastChunkSize(Vector3Int chunkCount, Vector3Int chunkSize)
    {
        return FieldSize - chunkSize * (chunkCount - Vector3Int.one);
    }

    //auxiliary method
    private static bool TryCreateVoxelWithoutReflection(Vector3Int globalVoxelPos)
    {
        return TryCreateVoxelWithoutReflection(VoxelId, globalVoxelPos);
    }

    private static bool TryCreateVoxelWithoutReflection(int voxelId, Vector3Int globalVoxelPos)
    {
        Chunk chunk = GetChunk(globalVoxelPos);
        if (chunk == null) return false;

        if (chunk.TryCreateVoxel(voxelId, globalVoxelPos))
        {
            CreateVertices(globalVoxelPos);
            UpdateChunksAround(globalVoxelPos);

            return true;
        }

        return false;
    }

    private static int GetVertexIndex(Vector3 globalVertexPos)
    {
        return VoxelatorArray.GetIndex(VerticesArraySize, (globalVertexPos + new Vector3(0.5f, 0.5f, 0.5f)).ToVector3Int());
    }

    private static void CreateVertex(Vector3 globalVertexPos)
    {
        int index = GetVertexIndex(globalVertexPos);

        if (Vertices[index] == null)
        {
            Vertices[index] = new Vertex(globalVertexPos);
        }

        Vertices[index].AdjacentVoxelsCount++;
    }

    private static void CreateVertices(Vector3Int globalVoxelPos)
    {
        foreach (Vector3 vertexPosition in GetVertexPositions(globalVoxelPos))
        {
            CreateVertex(vertexPosition);
        }
    }

    private static void ClampVertex(Vertex vertex)
    {
        Vector3 offset = vertex.Offset;
        if (offset.x < -1.5f) vertex.Offset = new Vector3(-1.5f, offset.y, offset.z); else
        if (offset.x > +1.5f) vertex.Offset = new Vector3(+1.5f, offset.y, offset.z);

        if (offset.y < -1.5f) vertex.Offset = new Vector3(offset.x, -1.5f, offset.z); else
        if (offset.y > +1.5f) vertex.Offset = new Vector3(offset.x, +1.5f, offset.z);

        if (offset.z < -1.5f) vertex.Offset = new Vector3(offset.x, offset.y, -1.5f); else
        if (offset.z > +1.5f) vertex.Offset = new Vector3(offset.x, offset.y, +1.5f);
    }

    private static Vector3[] GetVertexPositions(Vector3Int globalVoxelPos)
    {
        return new Vector3[]
        {
            globalVoxelPos + new Vector3(-0.5f, -0.5f, -0.5f),
            globalVoxelPos + new Vector3(+0.5f, -0.5f, -0.5f),
            globalVoxelPos + new Vector3(+0.5f, +0.5f, -0.5f),
            globalVoxelPos + new Vector3(-0.5f, +0.5f, -0.5f),

            globalVoxelPos + new Vector3(-0.5f, -0.5f, +0.5f),
            globalVoxelPos + new Vector3(+0.5f, -0.5f, +0.5f),
            globalVoxelPos + new Vector3(+0.5f, +0.5f, +0.5f),
            globalVoxelPos + new Vector3(-0.5f, +0.5f, +0.5f)
        };
    }

    private static Vector3Int GetChunkPos(Vector3 point)
    {
        return point.Div(ChunkSize).ToVector3Int();
    }

    private static void DeleteVoxel(Vector3Int globalVoxelPos)
    {
        Chunk chunk = GetChunk(globalVoxelPos);
        if (chunk == null) return;

        if (chunk.TryToDeleteVoxel(globalVoxelPos))
        {
            //deleting vertices
            foreach (Vector3 vertexPosition in GetVertexPositions(globalVoxelPos))
            {
                int index = GetVertexIndex(vertexPosition);
                if (Vertices[index].AdjacentVoxelsCount > 1) Vertices[index].AdjacentVoxelsCount--;
                else Vertices[index] = null;
            }

            UpdateChunksAround(globalVoxelPos);
        }
    }

    private static bool TryDeleteVoxel(Vector3Int globalVoxelPosition)
    {
        if(TryDeleteVoxelWithoutReflection(globalVoxelPosition))
        {
            foreach(Vector3 reflectedPosition in _reflector.GetReflectedPositions(globalVoxelPosition))
            {
                TryDeleteVoxelWithoutReflection(reflectedPosition.ToVector3Int());
            }

            //if (_mirror.X) TryDeleteVoxelWithoutReflection(_reflector.GetReflectedPositionByX(globalVoxelPosition).ToVector3Int());
            return true;
        }

        return false;
    }

    private static bool TryDeleteVoxelWithoutReflection(Vector3Int globalVoxelPosition)
    {
        Chunk chunk = GetChunk(globalVoxelPosition);
        if (chunk == null) return false;

        if (chunk.TryToDeleteVoxel(globalVoxelPosition))
        {
            //deleting vertices
            foreach (Vector3 vertexPosition in GetVertexPositions(globalVoxelPosition))
            {
                int index = GetVertexIndex(vertexPosition);
                if (Vertices[index].AdjacentVoxelsCount > 1) Vertices[index].AdjacentVoxelsCount--;
                else Vertices[index] = null;
            }

            UpdateChunksAround(globalVoxelPosition);

            return true;
        }

        return false;
    }

    private static void TryToAddNonUpdatedChunk(Chunk chunk)
    {
        if (chunk == null) return;

        for (int i = 0; i < _nonUpdatedChunks.Count; i++)
        {
            if (_nonUpdatedChunks[i] == chunk)
                return;
        }

        _nonUpdatedChunks.Add(chunk);
    }

    private static void UpdateChunk(Chunk chunk)
    {
        TryToAddNonUpdatedChunk(chunk);
    }

    public static void UpdateChunksAround(Vector3Int globalVoxelPos)
    {
        UpdateChunk(GetChunk(globalVoxelPos + Vector3Int.left));
        UpdateChunk(GetChunk(globalVoxelPos + Vector3Int.right));
        UpdateChunk(GetChunk(globalVoxelPos + Vector3Int.down));
        UpdateChunk(GetChunk(globalVoxelPos + Vector3Int.up));
        UpdateChunk(GetChunk(globalVoxelPos + new Vector3Int().Back()));
        UpdateChunk(GetChunk(globalVoxelPos + new Vector3Int().Forward()));
    }

    public static void UpdateChunkMeshes()
    {
        for (int i = 0; i < _nonUpdatedChunks.Count; i++)
        {
            _nonUpdatedChunks[i].UpdateMesh();
        }

        _nonUpdatedChunks.Clear();
    }

    private static void UpdateChunkSelectedMeshes()
    {
        for (int i = 0; i < _nonUpdatedChunks.Count; i++)
        {
            _nonUpdatedChunks[i].UpdateSelectedMesh();
        }

        _nonUpdatedChunks.Clear();
    }

    private static void UpdateAllChunkMeshes()
    {
        for (int i = 0; i < _nonUpdatedChunks.Count; i++)
        {
            _nonUpdatedChunks[i].UpdateMesh();
            _nonUpdatedChunks[i].UpdateSelectedMesh();
        }

        _nonUpdatedChunks.Clear();
    }

    private static bool TryReflectVoxelByX(Voxel voxel)
    {
        if (voxel == null) return false;
        Vector3Int reflectedPosition = _reflector.GetReflectedPositionByX(voxel.Position).ToVector3Int();
        Chunk chunk = GetChunk(reflectedPosition);

        if (chunk.TryCreateVoxel(voxel.Id, reflectedPosition))
        {
            foreach (Vector3 vertexPosition in GetVertexPositions(voxel.Position))
            {
                Vertex vertex = GetVertex(vertexPosition);

                Vector3 reflectedVertexPosition = _reflector.GetReflectedPositionByX(vertexPosition);
                CreateVertex(reflectedVertexPosition);
                Vertex reflectedVertex = GetVertex(reflectedVertexPosition);

                reflectedVertex.Offset = new Vector3(-vertex.Offset.x, vertex.Offset.y, vertex.Offset.z);
            }

            UpdateChunksAround(reflectedPosition);

            return true;
        }

        return false;
    }

    private static bool TryReflectVoxelByY(Voxel voxel)
    {
        if (voxel == null) return false;
        Vector3Int reflectedPosition = _reflector.GetReflectedPositionByY(voxel.Position).ToVector3Int();
        Chunk chunk = GetChunk(reflectedPosition);

        if (chunk.TryCreateVoxel(voxel.Id, reflectedPosition))
        {
            foreach (Vector3 vertexPosition in GetVertexPositions(voxel.Position))
            {
                Vertex vertex = GetVertex(vertexPosition);

                Vector3 reflectedVertexPosition = _reflector.GetReflectedPositionByY(vertexPosition);
                CreateVertex(reflectedVertexPosition);
                Vertex reflectedVertex = GetVertex(reflectedVertexPosition);

                reflectedVertex.Offset = new Vector3(vertex.Offset.x, -vertex.Offset.y, vertex.Offset.z);
            }

            UpdateChunksAround(reflectedPosition);

            return true;
        }

        return false;
    }

    private static bool TryReflectVoxelByZ(Voxel voxel)
    {
        if (voxel == null) return false;
        Vector3Int reflectedPosition = _reflector.GetReflectedPositionByZ(voxel.Position).ToVector3Int();
        Chunk chunk = GetChunk(reflectedPosition);

        if (chunk.TryCreateVoxel(voxel.Id, reflectedPosition))
        {
            foreach (Vector3 vertexPosition in GetVertexPositions(voxel.Position))
            {
                Vertex vertex = GetVertex(vertexPosition);

                Vector3 reflectedVertexPosition = _reflector.GetReflectedPositionByZ(vertexPosition);
                CreateVertex(reflectedVertexPosition);
                Vertex reflectedVertex = GetVertex(reflectedVertexPosition);

                reflectedVertex.Offset = new Vector3(vertex.Offset.x, vertex.Offset.y, -vertex.Offset.z);
            }

            UpdateChunksAround(reflectedPosition);

            return true;
        }

        return false;
    }

    private static void UpdateReflection(Vector3Bool mirror)
    {
        if (Voxel.Head != null)
        {
            if (mirror.X != _mirror.X)
            {
                if (mirror.X)
                    ReflectVoxelsByX();
                else
                    DeleteReflectionByX();
            }
            if (mirror.Y != _mirror.Y)
            {
                if (mirror.Y)
                    ReflectVoxelsByY();
                else
                    DeleteReflectionByY();
            }
            if (mirror.Z != _mirror.Z)
            {
                if (mirror.Z)
                    ReflectVoxelsByZ();
                else
                    DeleteReflectionByZ();
            }

            UpdateChunkMeshes();
        }

        _mirror = mirror;
    }

    private static void ReflectVoxelsByX()
    {
        for (Voxel voxel = Voxel.Head; voxel != null; voxel = voxel.Prev)
            TryReflectVoxelByX(voxel);
    }

    private static void ReflectVoxelsByY()
    {
        for (Voxel voxel = Voxel.Head; voxel != null; voxel = voxel.Prev)
            TryReflectVoxelByY(voxel);
    }

    private static void ReflectVoxelsByZ()
    {
        for (Voxel voxel = Voxel.Head; voxel != null; voxel = voxel.Prev)
            TryReflectVoxelByZ(voxel);
    }

    private static void DeleteReflectionByX()
    {
        Voxel voxel = Voxel.Head;
        while (voxel != null)
        {
            Vector3Int voxelPosition = voxel.Position;
            voxel = voxel.Prev;
            if (voxelPosition.x > FieldSize.x / 2 - 1)
                DeleteVoxel(voxelPosition);
        }
    }

    private static void DeleteReflectionByY()
    {
        Voxel voxel = Voxel.Head;
        while (voxel != null)
        {
            Vector3Int voxelPosition = voxel.Position;
            voxel = voxel.Prev;
            if (voxelPosition.y > FieldSize.y / 2 - 1)
                DeleteVoxel(voxelPosition);
        }
    }

    private static void DeleteReflectionByZ()
    {
        Voxel voxel = Voxel.Head;
        while (voxel != null)
        {
            Vector3Int voxelPosition = voxel.Position;
            voxel = voxel.Prev;
            if (voxelPosition.z > FieldSize.z / 2 - 1)
                DeleteVoxel(voxelPosition);
        }
    }
}
