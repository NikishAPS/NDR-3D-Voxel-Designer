using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class ChunksManager
{
    public static int VoxelId { get; set; }
    public static int IncrementOption { get; private set; }
    public static Vector3Int FieldSize { get; private set; }
    public static Vector3Int VerticesArraySize { get; private set; }
    public static Vector3 Center => FieldSize.ToVector3() * 0.5f;

    public static Material ChunkMaterial => VoxelatorManager.Project.ChunkMaterial;
    public static Material SelectedChunkMaterial => VoxelatorManager.Project.SelectedChunkMaterial;

    public static Chunk[] Chunks { get; private set; }
    public static Vertex[] Vertices { get; private set; }


    public static readonly Vector3Int ChunkSize = new Vector3Int(1, 1, 1) * 16;

    public static Vertex SelectedVertex { get; set; }

    public static Vector3 MiddleSelectedPos { get; private set; }
    public static int SelectedVoxelCount => _selectedVoxelPositions.Count;

    private static Vector3Int _chunkSizes;
    private static List<Chunk> _nonUpdatedChunks = new List<Chunk>();
    private static LinkedList<Vector3Int> _selectedVoxelPositions = new LinkedList<Vector3Int>();


    public static void InitField(Vector3Int size)
    {
        FieldSize = size;
        VerticesArraySize = FieldSize + Vector3Int.one;
    }

    public static void SetParameters(Vector3Int size, int incrementOption)
    {
        FieldSize = size;
        VerticesArraySize = FieldSize + Vector3Int.one;

        IncrementOption = incrementOption;
    }

    public static void InitChunks()
    {
        VoxelId = 1;
        MiddleSelectedPos = Vector3.zero;

        _chunkSizes = new Vector3Int(
             FieldSize.x % ChunkSize.x != 0 ? (int)(FieldSize.x / ChunkSize.x) + 1 : FieldSize.x / ChunkSize.x,
             FieldSize.y % ChunkSize.y != 0 ? (int)(FieldSize.y / ChunkSize.y) + 1 : FieldSize.y / ChunkSize.y,
             FieldSize.z % ChunkSize.z != 0 ? (int)(FieldSize.z / ChunkSize.z) + 1 : FieldSize.z / ChunkSize.z
             );

        Chunks = new Chunk[_chunkSizes.x * _chunkSizes.y * _chunkSizes.z];
        Vertices = new Vertex[VerticesArraySize.x * VerticesArraySize.y * VerticesArraySize.z];

        for (int x = 0; x < _chunkSizes.x; x++)
        {
            for (int y = 0; y < _chunkSizes.y; y++)
            {
                for (int z = 0; z < _chunkSizes.z; z++)
                {
                    Vector3Int chunkPos = new Vector3Int(x, y, z);

                    Vector3Int chunkSize = new Vector3Int(
                        x + 1 == _chunkSizes.x ? FieldSize.x - x * ChunkSize.x : ChunkSize.x,
                        y + 1 == _chunkSizes.y ? FieldSize.y - y * ChunkSize.y : ChunkSize.y,
                        z + 1 == _chunkSizes.z ? FieldSize.z - z * ChunkSize.z : ChunkSize.z
                        );

                    Chunks[(x * _chunkSizes.y + y) * _chunkSizes.z + z] =
                        new Chunk(chunkPos, chunkSize);
                }
            }
        }
    }

    public static void Release()
    {
        for(int i = 0; i < Chunks.Length; i++)
        {
            Chunks[i].Release();// = null; //Вызов деструктора 
        }

        Chunks = null;
    }

    public static bool InField(Vector3 position)
    {
        return VoxelatorManager.WithinTheArray(FieldSize, position);
    }

    public static Chunk GetChunk(Vector3 position)
    {
        return InField(position.ToVector3Int()) ? Chunks[VoxelatorManager.GetIndex(_chunkSizes, GetChunkPos(position))] : null;
    }

    public static Voxel GetVoxel(Vector3Int globalVoxelPos)
    {
        return InField(globalVoxelPos) ? GetChunk(globalVoxelPos).GetVoxel(globalVoxelPos) : null;
    }

    public static Voxel GetSelectedVoxel(Vector3Int globalVoxelPos)
    {
        return InField(globalVoxelPos) ? GetChunk(globalVoxelPos).GetSelectedVoxel(globalVoxelPos) : null;
    }

    public static Vertex GetVertex(Vector3 globalVertexPos)
    {
        int index = GetVertexIndex(globalVertexPos);
        return index < 0 ? null : Vertices[index];
    }


    public static void CreateVoxels(Vector3Int startVoxelArea, Vector3Int endVoxelArea)
    {
        Vector3Int steps = (endVoxelArea - startVoxelArea).Sign();
        endVoxelArea += steps;

        for (int x = startVoxelArea.x; x != endVoxelArea.x; x += steps.x)
        {
            for (int y = startVoxelArea.y; y != endVoxelArea.y; y += steps.y)
            {
                for (int z = startVoxelArea.z; z != endVoxelArea.z; z += steps.z)
                {
                    if (!TryCreateVoxel(new Vector3Int(x, y, z)))
                    {
                        continue;
                    }
                }
            }
        }

        UpdateChunkMeshes();
    }
    
    public static void DeleteSelectedVoxels()
    {
        if (_selectedVoxelPositions.Count == 0) return;

        foreach (Vector3Int selectedVoxelPosition in _selectedVoxelPositions)
        {
            DeleteVoxel(selectedVoxelPosition);
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
            MiddleSelectedPos = (MiddleSelectedPos  + globalVoxelPos) / _selectedVoxelPositions.Count;
        }
    }

    public static void ResetVoxelSelection()
    {
        foreach(Vector3Int _selectedVoxelPosition in _selectedVoxelPositions)
        {
            Chunk chunk = GetChunk(_selectedVoxelPosition);
            if(chunk != null)
            {
                chunk.ResetVoxelSelection(_selectedVoxelPosition);
                UpdateChunksAround(_selectedVoxelPosition);
            }
        }

        MiddleSelectedPos = Vector3.zero;
        _selectedVoxelPositions.Clear();
        UpdateChunkSelectedMeshes();
    }

    public static void MoveSelectedVoxels(Vector3 startPoint, Vector3 offset)
    {
        if (_selectedVoxelPositions.Count == 0) return;

        Vector3Int roundedOffset = offset.RoundToInt();

        //checking limits
        foreach (Vector3Int selectedVoxelPosition in _selectedVoxelPositions)
        {
            if (!InField(selectedVoxelPosition + roundedOffset) ||
                GetVoxel(selectedVoxelPosition + roundedOffset) != null &&
                GetSelectedVoxel(selectedVoxelPosition + roundedOffset) == null) return;
        }

        //copying voxels
        Voxel[] voxels = new Voxel[_selectedVoxelPositions.Count];
        Vertex[] vertices = new Vertex[voxels.Length * 8];
        Vector3[] verticesPos = new Vector3[1];
        {
            int i = 0;
            foreach (Vector3Int selectedVoxelPosition in _selectedVoxelPositions)
            {
                voxels[i] = new Voxel(GetVoxel(selectedVoxelPosition));
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
            DeleteVoxel(selectedVoxelPosition);
        }
        _selectedVoxelPositions.Clear();

        //creating voxels
        for (int i = 0; i < voxels.Length; i++)
        {
            Chunk chunk = GetChunk(voxels[i].Position);
            if (chunk.TryToCreateVoxel(voxels[i].Id, voxels[i].Position + roundedOffset))
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

                        Vertices[index] = new Vertex(vertices[i * 8 + j].PivotPosition + roundedOffset, vertices[i * 8 + j].GetOffset());
                        Vertices[index].AdjacentVoxelsCount = adjacentVoxelsCount;

                        j++;
                    }
                }

            }

        }

        SceneData.DragSystem.OffsetPosition(roundedOffset);

        UpdateAllChunkMeshes();
    }

    public static void MoveVertex(Vector3 startPoint, Vector3 offset)
    {
        offset = RoundVertexPointPos(offset);

        if (SelectedVertex == null || offset == Vector3.zero) return;

        Vector3 vertexOffset = SelectedVertex.GetOffset();
        Vector3 newVertexPos = offset + vertexOffset;

        if (newVertexPos.x < -1.5) offset.x = -vertexOffset.x - 1.5f; else if (newVertexPos.x > 1.5f) offset.x = 1.5f - vertexOffset.x;
        if (newVertexPos.y < -1.5) offset.y = -vertexOffset.y - 1.5f; else if (newVertexPos.y > 1.5f) offset.y = 1.5f - vertexOffset.y;
        if (newVertexPos.z < -1.5) offset.z = -vertexOffset.z - 1.5f; else if (newVertexPos.z > 1.5f) offset.z = 1.5f - vertexOffset.z;

        SelectedVertex.Offset(offset);
        SceneData.DragSystem.OffsetPosition(offset);
        UpdateChunksAround(SelectedVertex.PivotPosition.ToVector3Int());
        UpdateAllChunkMeshes();
    }

    public static void SetSelectedMeshActive(bool active)
    {
        for(int i = 0; i < Chunks.Length; i++)
        {
            Chunks[i].SetActiveSelectedMesh(active);
        }
    }

    public static Vector3 RoundVertexPointPos(Vector3 pos)
    {
        return ((pos + Vector3.one * 0.5f) * IncrementOption).RoundToFloat() / IncrementOption - Vector3.one * 0.5f;
    }

    public static ChunksManagerData GetData()
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
        return new ChunksManagerData(IncrementOption, FieldSize, _chunkSizes, verticesData, chunksData);
    }

    public static void SetData(ChunksManagerData chunksManagerData)
    {
        if (Chunks != null)
            Release();

        IncrementOption = chunksManagerData.IncrementOption;
        InitField(chunksManagerData.FieldSize);
        _chunkSizes = chunksManagerData.ChunkSizes;

        Chunks = new Chunk[chunksManagerData.ChunksData.Length];
        for (int i = 0; i < Chunks.Length; i++)
        {
            Chunks[i] = new Chunk(chunksManagerData.ChunksData[i]);
        }
        Vertices = new Vertex[chunksManagerData.VerticesData.Length];
        for(int i = 0; i < Vertices.Length; i++)
        {
            if (chunksManagerData.VerticesData[i].PivotPosition != Vector3.one * -1)
                Vertices[i] = new Vertex(chunksManagerData.VerticesData[i]);
            else
                Vertices[i] = null;
        }

        _nonUpdatedChunks = Chunks.ToList();
        UpdateChunkMeshes();

        GridManager.Grids[Direction.Down].Size = new Vector3Int(ChunksManager.FieldSize.x, 1, ChunksManager.FieldSize.z);
        GridManager.Grids[Direction.Down].Active = true;
    }


    private static bool TryCreateVoxel(Vector3Int globalVoxelPos)
    {
        Chunk chunk = GetChunk(globalVoxelPos);
        if (chunk == null) return false;

        if (chunk.TryToCreateVoxel(VoxelId, globalVoxelPos))
        {
            CreateVertices(globalVoxelPos);

            UpdateChunksAround(globalVoxelPos);

            return true;
        }

        return false;
    }

    private static int GetVertexIndex(Vector3 globalVertexPos)
    {
        return VoxelatorManager.GetIndex(VerticesArraySize, (globalVertexPos + new Vector3(0.5f, 0.5f, 0.5f)).ToVector3Int());
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
        foreach(Vector3 vertexPosition in GetVertexPositions(globalVoxelPos))
        {
            CreateVertex(vertexPosition);
        }
    }

    private static Vector3Int GetChunkPos(Vector3 point)
    {
        return point.Div(ChunkSize).ToVector3Int();
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

    private static void TryToAddNonUpdatedChunk(Chunk chunk)
    {
        if (chunk == null) return;

        for(int i = 0; i < _nonUpdatedChunks.Count; i++)
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

    private static void UpdateChunkMeshes()
    {
        for(int i = 0; i < _nonUpdatedChunks.Count; i++)
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

}
