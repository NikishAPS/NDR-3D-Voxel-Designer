using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

public class Chunk : MonoBehaviour
{
    private Voxel _voxel;

    private Vector3Int _size;
    private int voxelIncrement;

    private Voxel[] _voxels;
    private Voxel[] _selectedVoxels;

    private Vector3?[] _offsetsVertices;
    private Vector3[] _offsetsVertices2;

    private List<int> _buildedIndices = new List<int>();
    private List<int> _selectedIndices = new List<int>();
    [SerializeField]
    private List<int> _verticesIndices = new List<int>();

    private Mesh _mesh;
    private Mesh _selectedMesh;
    private Mesh _verticesMesh;

    private int _faceCount;
    private int _selectedFaceCount;

    private Vector3 middleSelectedPos;

    [SerializeField]
    private MeshFilter _selectedMeshFilter;

    [SerializeField]
    private MeshFilter _verticesMeshFilter;

    public Vector3 Center
    {
        get { return SceneData.Vector3IntToFloat(_size) * 0.5f; }
    }

    public Vector3Int Size
    {
        get { return (_size); }
    }

    public int SelectedIndicesCount => _selectedIndices.Count;

    public Vector3 MiddleSelectedPosition => middleSelectedPos;

    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _selectedMesh = new Mesh();
        _selectedMeshFilter.mesh = _selectedMesh;

        _verticesMesh = new Mesh();
        _verticesMeshFilter.mesh = _verticesMesh;
    }

    public void SetSelectedMeshActive(bool active)
    {
        _selectedMeshFilter.transform.gameObject.SetActive(active);
    }

    public ChunkData GetData()
    {
        //data arrays
        VoxelData[] voxelData = new VoxelData[_buildedIndices.Count];
        Vector3[] offsetsVertices = new Vector3[_verticesIndices.Count];

        //initializing arrays
        for (int i = 0; i < _buildedIndices.Count; i++)
        {
            voxelData[i] = new VoxelData(_voxels[_buildedIndices[i]]);
        }
        for (int i = 0; i < _verticesIndices.Count; i++)
        {
            offsetsVertices[i] = (Vector3)_offsetsVertices[_verticesIndices[i]];
        }
        return new ChunkData(Size, _buildedIndices, _verticesIndices, offsetsVertices, _faceCount, voxelData);
    }

    public void SetData(ChunkData chunkData)
    {
        Resize(chunkData.Size);

        _buildedIndices = chunkData.BuildedIndices;
        for (int i = 0; i < _buildedIndices.Count; i ++)
        {
            int index = _buildedIndices[i];
            _voxels[index] = new Voxel(chunkData.VoxelData[i].Id, chunkData.VoxelData[i].Position, chunkData.VoxelData[i].Faces);

        }

        _verticesIndices = chunkData.VerticesIndices;
        for (int i = 0; i < _verticesIndices.Count; i++)
        {
            int index = _verticesIndices[i];
            _offsetsVertices[index] = chunkData.OffsetsVertices[i];
        }

        _faceCount = chunkData.FaceCount;


        UpdateMesh();
    }

    public bool InChunk(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < _size.x &&
            pos.y >= 0 && pos.y < _size.y &&
            pos.z >= 0 && pos.z < _size.z;
    }

    public bool InChunk(Vector3Int size, Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < size.x &&
            pos.y >= 0 && pos.y < size.y &&
            pos.z >= 0 && pos.z < size.z;
    }

    public void Resize(Vector3Int size)
    {
        _size = size;
        Resize();
    }

    public void Resize()
    {
        int size = _size.x * _size.y * _size.z;

        _voxels = new Voxel[size];
        _selectedVoxels = new Voxel[size];
        _offsetsVertices = new Vector3?[(_size.x + 1) * (_size.y + 1) * (_size.z + 1)];

        SceneData.Grid.Resize(_size);
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

    public int GetIndexByPos(int x, int y, int z)
    {
        //(x * Y + y) * Z + z
        return (x * _size.y + y) * _size.z + z;
    }

    public int GetIndexByPos(Vector3Int pos)
    {
        if (!InChunk(pos)) return -1;
        return (pos.x * _size.y + pos.y) * _size.z + pos.z;
    }

    public int GetIndexByPos(Vector3Int size, Vector3Int pos)
    {
        return (pos.x * size.y + pos.y) * size.z + pos.z;
    }

    public Voxel GetVoxelByPos(Vector3Int pos)
    {
        int index = GetIndexByPos(pos);
        if (index >= _voxels.Length || index < 0) return null;
        return _voxels[index];
    }

    public Voxel GetVoxelByPos(Voxel[] voxels, Vector3Int pos)
    {
        int index = GetIndexByPos(pos);
        if (index >= _voxels.Length || index < 0) return null;
        return voxels[index];
    }

    public int GetVertexIndexByPos(Vector3 pos)
    {
        //    Vector3Int posCheck = new Vector3Int(pos.x > 0 ? pos.x - 1 : pos.x,
        //        pos.y > 0 ? pos.y - 1 : pos.y,
        //        pos.z > 0 ? pos.z - 1 : pos.z);

        Vector3Int posInt = SceneData.Vector3FloatToInt(pos + Vector3.one * 0.5f);
        Vector3Int size = _size + Vector3Int.one;
        //if ((pos + Vector3.one * 0.5f - (Vector3)posInt).magnitude > 0.0001f) { print("2"); return -1; }
        //else { print(pos + Vector3.one * 0.5f ); print(posInt); }
        if (!InChunk(new Vector3Int(_size.x, _size.y, _size.z) + Vector3Int.one, posInt)) return -1;
        return (posInt.x * size.y + posInt.y) * size.z + posInt.z;
    }

    public Vector3? GetOffsetVertexByPos(Vector3 pos)
    {
        Vector3? offsetVertex = null;
        int index = GetVertexIndexByPos(pos);
        if (index != -1) offsetVertex = _offsetsVertices[index];
        return offsetVertex;
    }

    public void OffsetVertexByPos(Vector3 pos, Vector3 value)
    {
        _offsetsVertices[GetVertexIndexByPos(pos)] += value;
    }

    private void CreateVoxelMesh(Voxel[] voxels, List<int> indeces, ref int faceCount, int id, Vector3Int pos)
    {
        int index = GetIndexByPos(pos);

        Voxel newVoxel = new Voxel(id, pos);
        indeces.Add(index);
        voxels[index] = newVoxel;


        //print(GetIndexByPos(posInt));

        Voxel adjacentVoxel =

        GetVoxelByPos(voxels, pos + Vector3Int.left);
        if (adjacentVoxel == null) { newVoxel.SetLeftFace(true); faceCount++; }
        else { adjacentVoxel.SetRightFace(false); faceCount--; }

        adjacentVoxel = GetVoxelByPos(voxels, pos + Vector3Int.right);
        if (adjacentVoxel == null) { newVoxel.SetRightFace(true); faceCount++; }
        else { adjacentVoxel.SetLeftFace(false); faceCount--; }

        adjacentVoxel = GetVoxelByPos(voxels, pos + Vector3Int.down);
        if (adjacentVoxel == null) { newVoxel.SetBottomFace(true); faceCount++; }
        else { adjacentVoxel.SetTopFace(false); faceCount--; }

        adjacentVoxel = GetVoxelByPos(voxels, pos + Vector3Int.up);
        if (adjacentVoxel == null) { newVoxel.SetTopFace(true); faceCount++; }
        else { adjacentVoxel.SetBottomFace(false); faceCount--; }

        adjacentVoxel = GetVoxelByPos(voxels, pos + new Vector3Int(0, 0, -1));
        if (adjacentVoxel == null) { newVoxel.SetRearFace(true); faceCount++; }
        else { adjacentVoxel.SetFrontFace(false); faceCount--; }

        adjacentVoxel = GetVoxelByPos(voxels, pos + new Vector3Int(0, 0, 1));
        if (adjacentVoxel == null) { newVoxel.SetFrontFace(true); faceCount++; }
        else { adjacentVoxel.SetRearFace(false); faceCount--; }
    }

    private void UpdateVoxelByIndex(int index)
    {
        Voxel voxel = _voxels[index];

        Vector3Int pos = _voxels[index].Position;

        if (GetVoxelByPos(pos + Vector3Int.left) == null) { if (!voxel.Faces[0]) { _faceCount++; voxel.SetLeftFace(true); } }
        else { if (voxel.Faces[0]) { _faceCount--; voxel.SetLeftFace(false); } }

        if (GetVoxelByPos(pos + Vector3Int.right) == null) { if (!voxel.Faces[1]) { _faceCount++; voxel.SetRightFace(true); } }
        else { if (voxel.Faces[1]) { _faceCount--; voxel.SetRightFace(false); } }

        if (GetVoxelByPos(pos + Vector3Int.down) == null) { if (!voxel.Faces[2]) { _faceCount++; voxel.SetBottomFace(true); } }
        else { if (voxel.Faces[2]) { _faceCount--; voxel.SetBottomFace(false); } }

        if (GetVoxelByPos(pos + Vector3Int.up) == null) { if (!voxel.Faces[3]) { _faceCount++; voxel.SetTopFace(true); } }
        else { if (voxel.Faces[3]) { _faceCount--; voxel.SetTopFace(false); } }

        if (GetVoxelByPos(pos + new Vector3Int(0, 0, -1)) == null) { if (!voxel.Faces[4]) { _faceCount++; voxel.SetRearFace(true); } }
        else { if (voxel.Faces[4]) { _faceCount--; voxel.SetRearFace(false); } }

        if (GetVoxelByPos(pos + new Vector3Int(0, 0, 1)) == null) { if (!voxel.Faces[5]) { _faceCount++; voxel.SetFrontFace(true); } }
        else { if (voxel.Faces[5]) { _faceCount--; voxel.SetFrontFace(false); } }

    }

    public void CreateVertexOffsetByIndex(int index)
    {
        if (_offsetsVertices[index] == null)
        {
            _offsetsVertices[index] = Vector3.zero;
            _verticesIndices.Add(index);
        }
    }

    public void DeleteVertexOffsetByPos(Vector3 pos)
    {
        if (GetVoxelByPos(SceneData.Vector3FloatToInt(pos + new Vector3(-0.5f, -0.5f, -0.5f))) == null &&
            GetVoxelByPos(SceneData.Vector3FloatToInt(pos + new Vector3(+0.5f, -0.5f, -0.5f))) == null &&
            GetVoxelByPos(SceneData.Vector3FloatToInt(pos + new Vector3(-0.5f, +0.5f, -0.5f))) == null &&
            GetVoxelByPos(SceneData.Vector3FloatToInt(pos + new Vector3(+0.5f, +0.5f, -0.5f))) == null &&

            GetVoxelByPos(SceneData.Vector3FloatToInt(pos + new Vector3(-0.5f, -0.5f, +0.5f))) == null &&
            GetVoxelByPos(SceneData.Vector3FloatToInt(pos + new Vector3(+0.5f, -0.5f, +0.5f))) == null &&
            GetVoxelByPos(SceneData.Vector3FloatToInt(pos + new Vector3(-0.5f, +0.5f, +0.5f))) == null &&
            GetVoxelByPos(SceneData.Vector3FloatToInt(pos + new Vector3(+0.5f, +0.5f, +0.5f))) == null)
        {

            _verticesIndices.Remove(GetVertexIndexByPos(pos));
            _offsetsVertices[GetVertexIndexByPos(pos)] = null;
        }
    }

    private void CreateOffsetsVerticesByVoxelIndex(int voxelIndex)
    {
        Vector3 pos = _voxels[voxelIndex].Position;

        CreateVertexOffsetByIndex(GetVertexIndexByPos(pos + new Vector3(-0.5f, -0.5f, -0.5f)));
        CreateVertexOffsetByIndex(GetVertexIndexByPos(pos + new Vector3(+0.5f, -0.5f, -0.5f)));
        CreateVertexOffsetByIndex(GetVertexIndexByPos(pos + new Vector3(+0.5f, +0.5f, -0.5f)));
        CreateVertexOffsetByIndex(GetVertexIndexByPos(pos + new Vector3(-0.5f, +0.5f, -0.5f)));

        CreateVertexOffsetByIndex(GetVertexIndexByPos(pos + new Vector3(-0.5f, -0.5f, +0.5f)));
        CreateVertexOffsetByIndex(GetVertexIndexByPos(pos + new Vector3(+0.5f, -0.5f, +0.5f)));
        CreateVertexOffsetByIndex(GetVertexIndexByPos(pos + new Vector3(+0.5f, +0.5f, +0.5f)));
        CreateVertexOffsetByIndex(GetVertexIndexByPos(pos + new Vector3(-0.5f, +0.5f, +0.5f)));
    }

    private void DeleteOffsetsVerticesByPos(Vector3 voxelPos)
    {
        DeleteVertexOffsetByPos(voxelPos + new Vector3(-0.5f, -0.5f, -0.5f));
        DeleteVertexOffsetByPos(voxelPos + new Vector3(+0.5f, -0.5f, -0.5f));
        DeleteVertexOffsetByPos(voxelPos + new Vector3(+0.5f, +0.5f, -0.5f));
        DeleteVertexOffsetByPos(voxelPos + new Vector3(-0.5f, +0.5f, -0.5f));

        DeleteVertexOffsetByPos(voxelPos + new Vector3(-0.5f, -0.5f, +0.5f));
        DeleteVertexOffsetByPos(voxelPos + new Vector3(+0.5f, -0.5f, +0.5f));
        DeleteVertexOffsetByPos(voxelPos + new Vector3(+0.5f, +0.5f, +0.5f));
        DeleteVertexOffsetByPos(voxelPos + new Vector3(-0.5f, +0.5f, +0.5f));
    }

    public void CreateVoxel(int id, Vector3Int pos)
    {
        if (GetVoxelByPos(_voxels, pos) != null) return;

        //CreateVoxelMesh(_voxels, _buildedIndeces, ref _faceCount, id, pos);
        //UpdateMesh();

        //return;

        int index = GetIndexByPos(pos);

        if (GetVoxelByPos(pos) != null) return;

        Voxel newVoxel = new Voxel(id, pos);
        _buildedIndices.Add(index);
        _voxels[index] = newVoxel;
        
        //print(GetIndexByPos(posInt));

        Voxel 


        adjacentVoxel = GetVoxelByPos(pos + Vector3Int.left);
        if (adjacentVoxel == null) { newVoxel.SetLeftFace(true); _faceCount++; }
        else { adjacentVoxel.SetRightFace(false); _faceCount--; }

        adjacentVoxel = GetVoxelByPos(pos + Vector3Int.right);
        if (adjacentVoxel == null) { newVoxel.SetRightFace(true); _faceCount++; }
        else { adjacentVoxel.SetLeftFace(false); _faceCount--; }

        adjacentVoxel = GetVoxelByPos(pos + Vector3Int.down);
        if (adjacentVoxel == null) { newVoxel.SetBottomFace(true); _faceCount++; }
        else { adjacentVoxel.SetTopFace(false); _faceCount--; }

        adjacentVoxel = GetVoxelByPos(pos + Vector3Int.up);
        if (adjacentVoxel == null) { newVoxel.SetTopFace(true); _faceCount++; }
        else {adjacentVoxel.SetBottomFace(false); _faceCount--; }

        adjacentVoxel = GetVoxelByPos(pos + new Vector3Int(0, 0, -1));
        if (adjacentVoxel == null) {newVoxel.SetRearFace(true); _faceCount++; }
        else {adjacentVoxel.SetFrontFace(false); _faceCount--;}

        adjacentVoxel = GetVoxelByPos(pos + new Vector3Int(0, 0, 1));
        if (adjacentVoxel == null) { newVoxel.SetFrontFace(true); _faceCount++; }
        else { adjacentVoxel.SetRearFace(false); _faceCount--; }

        CreateOffsetsVerticesByVoxelIndex(index);

        UpdateMesh();
        UpdateVerticesMesh();
    }

    public void SelectVoxel(Vector3Int pos)
    {
        int index = GetIndexByPos(pos);
        Voxel buildedVoxel = GetVoxelByPos(pos);

        if (buildedVoxel == null || GetVoxelByPos(_selectedVoxels, pos) != null) return;

        Voxel newSelectedVoxel = new Voxel(0, pos);
        _selectedVoxels[index] = newSelectedVoxel;
        _selectedIndices.Add(index);

        for (int i = 0; i < buildedVoxel.Faces.Length; i++)
        {
            if (buildedVoxel.Faces[i])
            {
                newSelectedVoxel.Faces[i] = buildedVoxel.Faces[i];
                _selectedFaceCount++;
            }
        }

        



        //CreateVoxelMesh(_selectedVoxels, _selectedIndeces, ref _selectedFaceCount, 0, pos);
        middleSelectedPos = (middleSelectedPos * (_selectedIndices.Count - 1) + pos) / _selectedIndices.Count;
        UpdateSelectedMesh();
    }

    public void DeleteVoxel()
    {
        for(int i = 0; i < _selectedIndices.Count; i++)
        {
            for (int j = 0; j < _voxels[_selectedIndices[i]].Faces.Length; j++)
            {
                if (_voxels[_selectedIndices[i]].Faces[j])
                {
                    _faceCount--;
                }
            }


            for (int j = 0; j < _buildedIndices.Count; j++)
            {
                if (_selectedIndices[i] == _buildedIndices[j])
                {
                    Vector3Int pos = SceneData.Vector3FloatToInt(_voxels[_buildedIndices[j]].Position);

                    _buildedIndices.Remove(_buildedIndices[j]);

                    Voxel 
                    adjacentVoxel = GetVoxelByPos(pos + Vector3Int.left);
                    if (adjacentVoxel != null) { adjacentVoxel.SetRightFace(true); _faceCount++; };

                    adjacentVoxel = GetVoxelByPos(pos + Vector3Int.right);
                    if (adjacentVoxel != null) { adjacentVoxel.SetLeftFace(true); _faceCount++; };

                    adjacentVoxel = GetVoxelByPos(pos + Vector3Int.down);
                    if (adjacentVoxel != null) { adjacentVoxel.SetTopFace(true); _faceCount++; };

                    adjacentVoxel = GetVoxelByPos(pos + Vector3Int.up);
                    if (adjacentVoxel != null) { adjacentVoxel.SetBottomFace(true); _faceCount++; };

                    adjacentVoxel = GetVoxelByPos(pos + new Vector3Int(0, 0, -1));
                    if (adjacentVoxel != null) { adjacentVoxel.SetFrontFace(true); _faceCount++; };

                    adjacentVoxel = GetVoxelByPos(pos + new Vector3Int(0, 0, 1));
                    if (adjacentVoxel != null) { adjacentVoxel.SetRearFace(true); _faceCount++; };
                }
            }

            Vector3 posToDeleteOffsetsVertices = _voxels[_selectedIndices[i]].Position;
            _voxels[_selectedIndices[i]] = null;
            DeleteOffsetsVerticesByPos(posToDeleteOffsetsVertices);

        }

        for (int i = 0; i < _selectedVoxels.Length; i++)
        {
            //DeleteOffsetsVerticesByIndex(_selectedIndeces[i]);
        }

        _selectedFaceCount = 0;
        _selectedIndices.Clear();



        UpdateMesh();
        UpdateSelectedMesh();


    }

    public void ResetSelection()
    {
        for(int i =0; i < _selectedIndices.Count; i++)
        {
            _selectedVoxels[_selectedIndices[i]] = null;
        }

        _selectedIndices.Clear();

        UpdateSelectedMesh();
    }

    public void MoveVoxels(Vector3 startPos, Vector3 offset)
    {
        Vector3Int offsetInt = SceneData.Vector3FloatRound(offset);

        if (offsetInt == Vector3Int.zero) return;

        //checking movement limits
        for (int i = 0; i < _selectedIndices.Count; i++)
        {
            Vector3Int newPos = _voxels[_selectedIndices[i]].Position + offsetInt;
            Vector3Int newPosN = _voxels[_selectedIndices[i]].Position + SceneData.Vector3FloatToInt(((Vector3)offset).normalized);

            int index = GetIndexByPos(newPosN);
            if (GetIndexByPos(newPos) == -1 || index == -1 || _voxels[index] != null && _selectedVoxels[index] == null) return;

            //if (GetIndexByPos(pos) == -1) { print("ret1"); return; }
            //if (index == -1) { print("ret2"); return; }
            //if (_voxels[index] != null) { print(posN); return; }
        }

        Voxel[] offsetBuildedVoxels = new Voxel[_selectedIndices.Count];
        Vector3Int[] offsetSelectedPoss = new Vector3Int[_selectedIndices.Count];
        for (int i = 0; i < _selectedIndices.Count; i++)
        {
            Vector3Int curPos = _voxels[_selectedIndices[i]].Position;
            Vector3Int newPos = curPos + offsetInt;

            int curIndex = GetIndexByPos(curPos);
            int newIndex = GetIndexByPos(newPos);

            //if (_selectedVoxels[newIndex] == null)//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //{
            //    _voxels[newIndex] = new Voxel(_voxels[curIndex].id, newPos, _voxels[curIndex].availabilityVertices);
            //    _voxels[curIndex] = null;

            //    _selectedVoxels[newIndex] = new Voxel(0, newPos, _selectedVoxels[curIndex].availabilityVertices);
            //    _selectedVoxels[curIndex] = null;
            //}

            offsetBuildedVoxels[i] = new Voxel(_voxels[curIndex].Id, newPos, _voxels[curIndex].Faces);
            _voxels[curIndex] = null;
            DeleteOffsetsVerticesByPos(curPos);

            offsetSelectedPoss[i] = newPos;
            _selectedVoxels[curIndex] = null;

            //_selectedIndeces[i] = newIndex;

            for (int j = 0; j < _buildedIndices.Count; j++)
            {
                if (_buildedIndices[j] == curIndex)
                {
                    _buildedIndices[j] = newIndex;
                    break;
                }
            }
        }

        _selectedIndices.Clear();
        _selectedFaceCount = 0;


        for (int i = 0; i < offsetBuildedVoxels.Length; i++)
        {
            _voxels[GetIndexByPos(offsetSelectedPoss[i])] = offsetBuildedVoxels[i];
        }

        for(int i = 0; i < _buildedIndices.Count; i++)
        {
            UpdateVoxelByIndex(_buildedIndices[i]);
            CreateOffsetsVerticesByVoxelIndex(_buildedIndices[i]);
        }

        for (int i = 0; i < offsetSelectedPoss.Length; i++)
        {
            SelectVoxel(offsetSelectedPoss[i]);
        }

        middleSelectedPos += offsetInt;

        //SceneData.dragSystem.ResetDragValue();
        //SceneData.dragSystem.SetPosition(middleSelectedPos);
        SceneData.DragSystem.OffsetPosition(offsetInt);

        UpdateMesh();
        UpdateSelectedMesh();
    }

    private void OffsetVertexByPos(ref Vector3 vertex)
    {
        //Vector3Int pos = SceneData.Vector3FloatToInt(vertex + Vector3.one * 0.5f);
        int index = GetVertexIndexByPos(vertex);
        vertex += (Vector3)_offsetsVertices?[index];
    }

    public void UpdateMesh()
    {
        _mesh.Clear();

        if (_faceCount == 0) return;

        Vector3[] vertices = new Vector3[_faceCount * 4];
        Vector2[] uv = new Vector2[_faceCount * 4];
        int[] triangles = new int[_faceCount * 6];
        int i4 = 0;
        int i6 = 0;
        //for (int i = 0; i < _voxels.Length; i++)
        for (int i = 0; i < _buildedIndices.Count; i++)
        {
            Voxel curVoxel = _voxels[_buildedIndices[i]];
            for (int j = 0; j < curVoxel.Faces.Length; j++)
            {
                if (curVoxel.Faces[j])
                {
                    int _i = j * 4;
                    Vector3 verPos = curVoxel.Position;
                    //vertices[i4 + 0] = SceneData.voxelVertices[_i + 0] + verPos;
                    //vertices[i4 + 1] = SceneData.voxelVertices[_i + 1] + verPos;
                    //vertices[i4 + 2] = SceneData.voxelVertices[_i + 2] + verPos;
                    //vertices[i4 + 3] = SceneData.voxelVertices[_i + 3] + verPos;

                    //OffsetVertexByPos(ref vertices[i4 + 0]);
                    //OffsetVertexByPos(ref vertices[i4 + 1]);
                    //OffsetVertexByPos(ref vertices[i4 + 2]);
                    //OffsetVertexByPos(ref vertices[i4 + 3]);
                    for (int v = 0; v <= 3; v++)
                    {
                        vertices[i4 + v] = SceneData.VoxelVertices[_i + v] + verPos;
                        OffsetVertexByPos(ref vertices[i4 + v]);
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

        if (_selectedFaceCount == 0) return;

        Vector3[] vertices = new Vector3[_selectedFaceCount * 4];
        Vector2[] uv = new Vector2[_selectedFaceCount * 4];
        int[] triangles = new int[_selectedFaceCount * 6];
        int i4 = 0;
        int i6 = 0;

        for (int i = 0; i < _selectedIndices.Count; i++)
        {
            Voxel curVoxel = _selectedVoxels[_selectedIndices[i]];
            for (int j = 0; j < curVoxel.Faces.Length; j++)
            {
                if (curVoxel.Faces[j])
                {
                    int _i = j * 4;
                    Vector3 verPos = curVoxel.Position;

                    for (int v = 0; v <= 3; v++)
                    {
                        vertices[i4 + v] = (SceneData.VoxelVertices[_i + v] + verPos);
                        OffsetVertexByPos(ref vertices[i4 + v]);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (_offsetsVertices != null)
        for(float x = -0.5f; x < _size.x - 0.5f; x++)
            {
                for (float y = -0.5f; y < _size.y - 0.5f; y++)
                {
                    for (float z = -0.5f; z < _size.z - 0.5f; z++)
                    {
                        if (GetOffsetVertexByPos(new Vector3(x,y,z)) != null)
                        {
                            Gizmos.DrawSphere(new Vector3(x, y, z), 0.1f);
                        }
                    }
                }
            }
        return;
            if (_offsetsVertices != null)
            for (int i = 0; i < _buildedIndices.Count; i++)
            {
                Vector3 pos = _voxels[_buildedIndices[i]].Position;
                
                if(_offsetsVertices[GetVertexIndexByPos(pos + new Vector3(-0.5f, -0.5f, -0.5f))] != null)
                    Gizmos.DrawSphere(pos + new Vector3(-0.5f, -0.5f, -0.5f), 0.1f);
                if (_offsetsVertices[GetVertexIndexByPos(pos + new Vector3(+0.5f, -0.5f, -0.5f))] != null)
                    Gizmos.DrawSphere(pos + new Vector3(+0.5f, -0.5f, -0.5f), 0.1f);
                if (_offsetsVertices[GetVertexIndexByPos(pos + new Vector3(+0.5f, +0.5f, -0.5f))] != null)
                    Gizmos.DrawSphere(pos + new Vector3(+0.5f, +0.5f, -0.5f), 0.1f);
                if (_offsetsVertices[GetVertexIndexByPos(pos + new Vector3(-0.5f, +0.5f, -0.5f))] != null)
                    Gizmos.DrawSphere(pos + new Vector3(-0.5f, +0.5f, -0.5f), 0.1f);
                if (_offsetsVertices[GetVertexIndexByPos(pos + new Vector3(-0.5f, -0.5f, +0.5f))] != null)
                    Gizmos.DrawSphere(pos + new Vector3(-0.5f, -0.5f, +0.5f), 0.1f);
                if (_offsetsVertices[GetVertexIndexByPos(pos + new Vector3(+0.5f, -0.5f, +0.5f))] != null)
                    Gizmos.DrawSphere(pos + new Vector3(+0.5f, -0.5f, +0.5f), 0.1f);
                if (_offsetsVertices[GetVertexIndexByPos(pos + new Vector3(+0.5f, +0.5f, +0.5f))] != null)
                    Gizmos.DrawSphere(pos + new Vector3(+0.5f, +0.5f, +0.5f), 0.1f);
                if (_offsetsVertices[GetVertexIndexByPos(pos + new Vector3(-0.5f, +0.5f, +0.5f))] != null)
                    Gizmos.DrawSphere(pos + new Vector3(-0.5f, +0.5f, +0.5f), 0.1f);



            }
    }
}
