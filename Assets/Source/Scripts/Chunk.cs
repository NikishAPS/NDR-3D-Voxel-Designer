using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class Chunk : MonoBehaviour
{
    private Voxel _voxel;

    private Vector3Int _size;
    private Vector3Int _vertexArraySize;
    private Vector3Int _vertexPointAraySize;

    public Voxel[] Voxels { get; private set; }
    public Voxel[] SelectedVoxels { get; private set; }
    public Vertex[] Vertices { get; private set; }
    public VertexPoint[] VertexPoints { get; private set; }

    public Builder Builder { get; private set; }
    public Selector Selector { get; private set; }
    public Editor Editor { get; private set; }

    private int _incrementOption;

    private Mesh _mesh;
    private Mesh _selectedMesh;
    private Mesh _verticesMesh;

    [SerializeField] public MeshFilter _selectedMeshFilter;
    [SerializeField] private MeshFilter _verticesMeshFilter;


    public Vector3 Center => _size.ToVector3() * 0.5f;

    public Vector3Int Size => _size;

    public int IncrementOption => _incrementOption;

    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _selectedMesh = new Mesh();
        _selectedMeshFilter.mesh = _selectedMesh;

        _verticesMesh = new Mesh();
        _verticesMeshFilter.mesh = _verticesMesh;

        Builder = new Builder(this);
        Selector = new Selector(this);
        Editor = new Editor(this);
    }

    public void SetSelectedMeshActive(bool active)
    {
        _selectedMeshFilter.gameObject.SetActive(active);
    }

    public void SetSizeX(int value)
    {
        _size.x = value;
    }

    public void SetSizeY(int value)
    {
        _size.y = value;
    }

    public void SetSizeZ(int value)
    {
        _size.z = value;
    }

    public void SetIncrementOption(int incrementOption)
    {
        _incrementOption = incrementOption;
    }

    public bool InChunk(Vector3Int point)
    {
        return point.x >= 0 && point.x < _size.x &&
            point.y >= 0 && point.y < _size.y &&
            point.z >= 0 && point.z < _size.z;
    }

    public bool InBox(Vector3Int size, Vector3Int point)
    {
        return point.x >= 0 && point.x < size.x &&
            point.y >= 0 && point.y < size.y &&
            point.z >= 0 && point.z < size.z;
    }

    public void Resize()
    {
        int size = _size.x * _size.y * _size.z;

        _vertexArraySize = _size + Vector3Int.one;
        _vertexPointAraySize = _vertexArraySize * _incrementOption;

        Voxels = new Voxel[size];
        SelectedVoxels = new Voxel[size];

        Vertices = new Vertex[_vertexArraySize.x * _vertexArraySize.y * _vertexArraySize.z];
        VertexPoints = new VertexPoint[_vertexPointAraySize.x * _vertexPointAraySize.y * _vertexPointAraySize.z];

        SceneData.Grid.Resize(_size);
    }

    public void Resize(Vector3Int size)
    {
        _size = size;
        Resize();
    }

    public int GetIndex(Vector3Int size, Vector3Int pos)
    {
        return (pos.x * size.y + pos.y) * size.z + pos.z;
    }

    public Voxel GetVoxel(int index)
    {
        return (index < 0 || index >= Voxels.Length) ? null : Voxels[index];
    }

    public int GetIndexByPos(Vector3Int pos)
    {
        if (!InChunk(pos)) return -1;
        return GetIndex(_size, pos);
        return (pos.x * _size.y + pos.y) * _size.z + pos.z;
    }

    public Vector3Int GetPosByIndex(int index)
    {
        int z = index % _size.z;
        int y = (index - z) / _size.z % _size.y;
        int x = ((index - z) / _size.z - y) / _size.y;
        return new Vector3Int(x, y, z);
    }

    public Voxel GetVoxelByPos(Vector3Int pos)
    {
        return GetVoxel(GetIndexByPos(pos));
    }

    public Voxel GetSelectedVoxel(int index)
    {
        return (index < 0 || index >= SelectedVoxels.Length) ? null : SelectedVoxels[index];
    }

    public Voxel GetSelectedVoxelByPos(Vector3Int pos)
    {
        return GetSelectedVoxel(GetIndexByPos(pos));
    }

    public Vertex GetVertex(int index)
    {
        return (index < 0 || index >= Vertices.Length) ? null : Vertices[index];
    }

    public int GetVertexIndexByPos(Vector3 pos)
    {
        Vector3Int posInt = (pos + Vector3.one * 0.5f).ToVector3Int();

        if (!InBox(_vertexArraySize, posInt)) return -1;
        return GetIndex(_vertexArraySize, posInt);
    }

    public Vertex GetVertexByPos(Vector3 pos)
    {
        return GetVertex(GetVertexIndexByPos(pos));
    }

    public VertexPoint GetVertexPoint(int index)
    {
        return index < 0 || index >= VertexPoints.Length ? null : VertexPoints[index];
    }

    public int GetVertexPointIndexByPos(Vector3 vertexPointPos)
    {
        Vector3Int posInt = ((vertexPointPos + Vector3.one * 0.5f) * _incrementOption).ToVector3Int();

        if (!InBox(_vertexPointAraySize, posInt)) return -1;
        return GetIndex(_vertexPointAraySize, posInt);
    }

    public void OffsetVertexByPos(Vector3 pos, Vector3 value)
    {
        //_offsetsVertices[GetVertexIndexByPos(pos)] += value;
    }

    public void CreateVoxel(int id, Vector3Int pos)
    {
        if (Builder.TryCreateVoxel(id, pos))
        {
            Editor.CreateVertices(pos);

            UpdateMesh();
        }
    }

    public void DeleteSelectedVoxels()
    {
        for (int i = 0; i < Selector.SelectedVoxelIndices.Count; i++)
        {
            int voxelIndex = Selector.SelectedVoxelIndices[i];
            Vector3Int voxelPos = Voxels[Selector.SelectedVoxelIndices[i]].Position;

            Builder.DeleteVoxel(voxelIndex);
            Editor.DeleteVerticesByPos(voxelPos);
        }
        Selector.Reset();
        Builder.UpdateAllVoxels();

        UpdateMesh();
        UpdateSelectedMesh();
    }

    public void SelectVoxel(Vector3Int pos)
    {
        Selector.SelectVoxel(pos);

        UpdateSelectedMesh();
    }

    public void ResetSelection()
    {
        Selector.Reset();

        UpdateSelectedMesh();
    }

    public void MoveSelectedVoxels(Vector3 startPos, Vector3 offset)
    {
        Vector3Int offsetInt = offset.ToVector3Int();
        if (Selector.TryMoveVoxels(startPos, offsetInt))
        {
            UpdateMesh();
            UpdateSelectedMesh();

            SceneData.DragSystem.OffsetPosition(offsetInt);
        }
    }

    public ChunkData GetData()
    {
        //data arrays
        //VoxelData[] voxelData = new VoxelData[_buildedIndices.Count];
        //Vector3[] offsetsVertices = new Vector3[_verticesIndices.Count];

        ////initializing arrays
        //for (int i = 0; i < _buildedIndices.Count; i++)
        //{
        //    voxelData[i] = new VoxelData(Voxels[_buildedIndices[i]]);
        //}
        //for (int i = 0; i < _verticesIndices.Count; i++)
        //{
        //    offsetsVertices[i] = (Vector3)_offsetsVertices[_verticesIndices[i]];
        //}
        //return new ChunkData(Size, _buildedIndices, _verticesIndices, offsetsVertices, _faceCount, voxelData);
        return null;
    }

    public void SetData(ChunkData chunkData)
    {
        //Resize(chunkData.Size);

        //_buildedIndices = chunkData.BuildedIndices;
        //for (int i = 0; i < _buildedIndices.Count; i ++)
        //{
        //    int index = _buildedIndices[i];
        //    Voxels[index] = new Voxel(chunkData.VoxelData[i].Id, chunkData.VoxelData[i].Position, chunkData.VoxelData[i].Faces);

        //}

        //_verticesIndices = chunkData.VerticesIndices;
        //for (int i = 0; i < _verticesIndices.Count; i++)
        //{
        //    int index = _verticesIndices[i];
        //    _offsetsVertices[index] = chunkData.OffsetsVertices[i];
        //}

        //_faceCount = chunkData.FaceCount;


        UpdateMesh();
    }



    private void OffsetVertexByPos(ref Vector3 vertex)
    {
        int index = GetVertexIndexByPos(vertex);
        //vertex += (Vector3)_offsetsVertices?[index];
    }

    public void UpdateMesh()
    {
        _mesh.Clear();

        if (Builder.FaceCount == 0) return;

        Vector3[] vertices = new Vector3[Builder.FaceCount * 4];
        Vector2[] uv = new Vector2[Builder.FaceCount * 4];
        int[] triangles = new int[Builder.FaceCount * 6];
        int i4 = 0;
        int i6 = 0;

        for (int i = 0; i < Builder.BuildedVoxelIndices.Count; i++)
        {
            Voxel curVoxel = Builder.GetVoxelByBuildedIndex(i);
            for (int j = 0; j < curVoxel.Faces.Length; j++)
            {
                if (curVoxel.Faces[j])
                {
                    int _i = j * 4;
                    Vector3 verPos = curVoxel.Position;

                    for (int v = 0; v <= 3; v++)
                    {
                        vertices[i4 + v] = SceneData.VoxelVertices[_i + v] + verPos;
                        //OffsetVertexByPos(ref vertices[i4 + v]);
                    }

                    float v1 = SceneData.TextureMul * (curVoxel.Id % SceneData.TextureSize);
                    float v2 = 1 - SceneData.TextureMul * (curVoxel.Id / (SceneData.TextureSize + 1));
                    uv[i4 + 0] = new Vector2(v1, v2 - SceneData.TextureMul);
                    uv[i4 + 1] = new Vector2(v1, v2);
                    uv[i4 + 2] = new Vector2(v1 - SceneData.TextureMul, v2);
                    uv[i4 + 3] = new Vector2(v1 - SceneData.TextureMul, v2 - SceneData.TextureMul);

                    triangles[i6 + 0] = i4 + 0;
                    triangles[i6 + 1] = i4 + 1;
                    triangles[i6 + 2] = i4 + 3;
                    triangles[i6 + 3] = i4 + 1;
                    triangles[i6 + 4] = i4 + 2;
                    triangles[i6 + 5] = i4 + 3;

                    i4 += 4;
                    i6 += 6;
                }
            }
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.uv = uv;

        _mesh.Optimize();
        _mesh.RecalculateNormals();
    }

    private void UpdateSelectedMesh()
    {
        _selectedMesh.Clear();

        if (Selector.FaceCount == 0) return;

        Vector3[] vertices = new Vector3[Selector.FaceCount * 4];
        Vector2[] uv = new Vector2[Selector.FaceCount * 4];
        int[] triangles = new int[Selector.FaceCount * 6];
        int i4 = 0;
        int i6 = 0;

        for (int i = 0; i < Selector.SelectedVoxelIndices.Count; i++)
        {
            Voxel curVoxel = Selector.GetVoxelBySelectedIndex(i);
            for (int j = 0; j < curVoxel.Faces.Length; j++)
            {
                if (curVoxel.Faces[j])
                {
                    int _i = j * 4;
                    Vector3 verPos = curVoxel.Position;

                    for (int v = 0; v <= 3; v++)
                    {
                        vertices[i4 + v] = (SceneData.VoxelVertices[_i + v] + verPos);
                        //OffsetVertexByPos(ref vertices[i4 + v]);
                    }

                    triangles[i6 + 0] = i4 + 0;
                    triangles[i6 + 1] = i4 + 1;
                    triangles[i6 + 2] = i4 + 3;
                    triangles[i6 + 3] = i4 + 1;
                    triangles[i6 + 4] = i4 + 2;
                    triangles[i6 + 5] = i4 + 3;

                    i4 += 4;
                    i6 += 6;
                }
            }
        }

        _selectedMesh.vertices = vertices;
        _selectedMesh.triangles = triangles;

        _selectedMesh.Optimize();
        _selectedMesh.RecalculateNormals();
    }

    private void UpdateVerticesMesh()
    {
        _verticesMesh.Clear();

        Vector3[] vertices = new Vector3[0];
        int[] triangles = new int[0];

        _verticesMesh.vertices = vertices;
        _verticesMesh.triangles = triangles;

        _verticesMesh.Optimize();
        _verticesMesh.RecalculateNormals();
    }


    public int FaceCount, SelectedFaceCount;
    public void Update()
    {
        FaceCount = Builder.FaceCount;
        SelectedFaceCount = Selector.FaceCount;
    }

    public void OnDrawGizmos()
    {
        if (VertexPoints != null)
        {
            for(int i = 0; i < VertexPoints.Length; i++)
            {
                if (VertexPoints[i] != null)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(VertexPoints[i].Position, 0.05f);
                }
            }
        }

        return;
        if(Vertices != null)
        {
            for (int i = 0; i < Vertices.Length; i++)
            {
                if (Vertices[i] != null)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(Vertices[i].Position, 0.1f);
                }
            }
        }
    }
}
