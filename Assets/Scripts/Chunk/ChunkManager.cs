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


    public static readonly Vector3Int ChunkSize = new Vector3Int(1, 1, 1) * 64;

    public static Vertex SelectedVertex { get; set; }
    public static Vector3Int[] SelectedVoxelPositions => _selectedVoxelPositions.ToArray();

    public static Vector3 MiddleSelectedPos { get; private set; }
    public static int SelectedVoxelCount => _selectedVoxelPositions.Count;

    private static Vector3Bool _mirror;
    private static Vector3Int _chunkSizes;
    private static List<Chunk> _nonUpdatedChunks = new List<Chunk>();
    private static LinkedList<Vector3Int> _selectedVoxelPositions = new LinkedList<Vector3Int>();
    private static Reflector _reflector;

    private static Material _chunkMaterial;
    private static Material _selectedChunkMaterial;

    static ChunkManager()
    {
        _chunkMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/Chunk");
        _selectedChunkMaterial = ResourcesLoader.Load<Material>("Materials/Chunk/SelectedChunk");
    }

    public static void Init(Vector3Int fieldSize, int incrementOption)
    {
        FieldSize = fieldSize;
        VerticesArraySize = FieldSize + Vector3Int.one;

        IncrementOption = incrementOption;

        CreateChunks();
    }

    public static void SetVoxelIdByColor(Color color)
    {
        int r = (int)(color.r * 255);
        int g = (int)(color.g * 255);
        int b = (int)(color.b * 255);

        int index = Voxelator.GetIndex(Vector3Int.one * 256, new Vector3Int(r, g, b)) + 1;

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
        return Voxelator.WithinTheArray(FieldSize, position);
    }

    public static Chunk GetChunk(Vector3 position)
    {
        return InField(position.ToVector3Int()) ? Chunks[Voxelator.GetIndex(_chunkSizes, GetChunkPos(position))] : null;
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

    public static void SetSelectedMeshActive(bool active)
    {
        for (int i = 0; i < Chunks.Length; i++)
        {
            Chunks[i].SetActiveSelectedMesh(active);
        }
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
        return new ChunkManagerData(IncrementOption, FieldSize, _chunkSizes, verticesData, chunksData);
    }

    public static void SetData(ChunkManagerData chunksManagerData)
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

    private static void CreateChunks()
    {
        VoxelId = 4096 * 4096;
        MiddleSelectedPos = Vector3.zero;

        _chunkSizes = new Vector3Int(
             FieldSize.x % ChunkSize.x != 0 ? (int)(FieldSize.x / ChunkSize.x) + 1 : FieldSize.x / ChunkSize.x,
             FieldSize.y % ChunkSize.y != 0 ? (int)(FieldSize.y / ChunkSize.y) + 1 : FieldSize.y / ChunkSize.y,
             FieldSize.z % ChunkSize.z != 0 ? (int)(FieldSize.z / ChunkSize.z) + 1 : FieldSize.z / ChunkSize.z
             );

        Chunks = new Chunk[_chunkSizes.x * _chunkSizes.y * _chunkSizes.z];
        Vertices = new Vertex[VerticesArraySize.x * VerticesArraySize.y * VerticesArraySize.z];
        _reflector = new Reflector(FieldSize);

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
                        new Chunk(chunkPos, chunkSize, _chunkMaterial, _selectedChunkMaterial);
                }
            }
        }
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
        return Voxelator.GetIndex(VerticesArraySize, (globalVertexPos + new Vector3(0.5f, 0.5f, 0.5f)).ToVector3Int());
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
