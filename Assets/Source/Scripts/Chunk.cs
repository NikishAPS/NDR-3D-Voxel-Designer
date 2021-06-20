using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Voxel _voxel;

    [SerializeField]
    private Vector3Int _size;

    private Voxel[] _voxels;
    private Voxel[] _selectedVoxels;
    private Vector3?[] _offsetsVertices;


    private List<int> _buildedIndeces = new List<int>();
    private List<int> _selectedIndeces = new List<int>();
    private List<int> _verticesIndeces = new List<int>();

    private Mesh _mesh;
    private Mesh _selectedMesh;
    private Mesh _verticesMesh;

    [SerializeField]
    private int _faceCount;
    [SerializeField]
    private int _selectedFaceCount;

    [SerializeField]
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

    public int SelectedIndecesCount => _selectedIndeces.Count;

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

        SceneData.grid.Resize(_size);
    }

    public void SetSizeX(InputField inputField)
    {
        _size.x = int.Parse(inputField.text);
    }

    public void SetSizeY(InputField inputField)
    {
        _size.y = int.Parse(inputField.text);
    }

    public void SetSizeZ(InputField inputField)
    {
        _size.z = int.Parse(inputField.text);
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

        Vector3Int pos = _voxels[index].position;

        if (GetVoxelByPos(pos + Vector3Int.left) == null) { if (!voxel.availabilityVertices[0]) { _faceCount++; voxel.SetLeftFace(true); } }
        else { if (voxel.availabilityVertices[0]) { _faceCount--; voxel.SetLeftFace(false); } }

        if (GetVoxelByPos(pos + Vector3Int.right) == null) { if (!voxel.availabilityVertices[1]) { _faceCount++; voxel.SetRightFace(true); } }
        else { if (voxel.availabilityVertices[1]) { _faceCount--; voxel.SetRightFace(false); } }

        if (GetVoxelByPos(pos + Vector3Int.down) == null) { if (!voxel.availabilityVertices[2]) { _faceCount++; voxel.SetBottomFace(true); } }
        else { if (voxel.availabilityVertices[2]) { _faceCount--; voxel.SetBottomFace(false); } }

        if (GetVoxelByPos(pos + Vector3Int.up) == null) { if (!voxel.availabilityVertices[3]) { _faceCount++; voxel.SetTopFace(true); } }
        else { if (voxel.availabilityVertices[3]) { _faceCount--; voxel.SetTopFace(false); } }

        if (GetVoxelByPos(pos + new Vector3Int(0, 0, -1)) == null) { if (!voxel.availabilityVertices[4]) { _faceCount++; voxel.SetRearFace(true); } }
        else { if (voxel.availabilityVertices[4]) { _faceCount--; voxel.SetRearFace(false); } }

        if (GetVoxelByPos(pos + new Vector3Int(0, 0, 1)) == null) { if (!voxel.availabilityVertices[5]) { _faceCount++; voxel.SetFrontFace(true); } }
        else { if (voxel.availabilityVertices[5]) { _faceCount--; voxel.SetFrontFace(false); } }

    }

    public void CreateVertexOffsetByIndex(int index)
    {
        if (_offsetsVertices[index] == null)
        {
            _offsetsVertices[index] = Vector3.zero;
            _verticesIndeces.Add(index);
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
            _offsetsVertices[GetVertexIndexByPos(pos)] = null;
        }
    }

    private void CreateOffsetsVerticesByIndex(int voxelIndex)
    {
        Vector3 pos = _voxels[voxelIndex].position;

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
        _buildedIndeces.Add(index);
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

        CreateOffsetsVerticesByIndex(index);

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
        _selectedIndeces.Add(index);

        for (int i = 0; i < buildedVoxel.availabilityVertices.Length; i++)
        {
            if (buildedVoxel.availabilityVertices[i])
            {
                newSelectedVoxel.availabilityVertices[i] = buildedVoxel.availabilityVertices[i];
                _selectedFaceCount++;
            }
        }

        



        //CreateVoxelMesh(_selectedVoxels, _selectedIndeces, ref _selectedFaceCount, 0, pos);
        middleSelectedPos = (middleSelectedPos * (_selectedIndeces.Count - 1) + pos) / _selectedIndeces.Count;
        UpdateSelectedMesh();
    }

    public void DeleteVoxel()
    {
        for(int i = 0; i < _selectedIndeces.Count; i++)
        {
            for (int j = 0; j < _voxels[_selectedIndeces[i]].availabilityVertices.Length; j++)
            {
                if (_voxels[_selectedIndeces[i]].availabilityVertices[j])
                {
                    _faceCount--;
                }
            }


            for (int j = 0; j < _buildedIndeces.Count; j++)
            {
                if (_selectedIndeces[i] == _buildedIndeces[j])
                {
                    Vector3Int pos = SceneData.Vector3FloatToInt(_voxels[_buildedIndeces[j]].position);

                    _buildedIndeces.Remove(_buildedIndeces[j]);

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

            Vector3 posToDeleteOffsetsVertices = _voxels[_selectedIndeces[i]].position;
            _voxels[_selectedIndeces[i]] = null;
            DeleteOffsetsVerticesByPos(posToDeleteOffsetsVertices);

        }

        for (int i = 0; i < _selectedVoxels.Length; i++)
        {
            //DeleteOffsetsVerticesByIndex(_selectedIndeces[i]);
        }

        _selectedFaceCount = 0;
        _selectedIndeces.Clear();



        UpdateMesh();
        UpdateSelectedMesh();


    }

    public void ResetSelection()
    {
        for(int i =0; i < _selectedIndeces.Count; i++)
        {
            _selectedVoxels[_selectedIndeces[i]] = null;
        }

        _selectedIndeces.Clear();

        UpdateSelectedMesh();
    }

    public void MoveVoxels(Vector3 startPos, Vector3 offset)
    {
        Vector3Int offsetInt = SceneData.Vector3FloatRound(offset);

        if (offsetInt == Vector3Int.zero) return;

        //checking movement limits
        for (int i = 0; i < _selectedIndeces.Count; i++)
        {
            Vector3Int newPos = _voxels[_selectedIndeces[i]].position + offsetInt;
            Vector3Int newPosN = _voxels[_selectedIndeces[i]].position + SceneData.Vector3FloatToInt(((Vector3)offset).normalized);

            int index = GetIndexByPos(newPosN);
            if (GetIndexByPos(newPos) == -1 || index == -1 || _voxels[index] != null && _selectedVoxels[index] == null) return;

            //if (GetIndexByPos(pos) == -1) { print("ret1"); return; }
            //if (index == -1) { print("ret2"); return; }
            //if (_voxels[index] != null) { print(posN); return; }
        }

        Voxel[] offsetBuildedVoxels = new Voxel[_selectedIndeces.Count];
        Vector3Int[] offsetSelectedPoss = new Vector3Int[_selectedIndeces.Count];
        for (int i = 0; i < _selectedIndeces.Count; i++)
        {
            Vector3Int curPos = _voxels[_selectedIndeces[i]].position;
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

            offsetBuildedVoxels[i] = new Voxel(_voxels[curIndex].id, newPos, _voxels[curIndex].availabilityVertices);
            _voxels[curIndex] = null;
            DeleteOffsetsVerticesByPos(curPos);

            offsetSelectedPoss[i] = newPos;
            _selectedVoxels[curIndex] = null;

            //_selectedIndeces[i] = newIndex;

            for (int j = 0; j < _buildedIndeces.Count; j++)
            {
                if (_buildedIndeces[j] == curIndex)
                {
                    _buildedIndeces[j] = newIndex;
                    break;
                }
            }
        }

        _selectedIndeces.Clear();
        _selectedFaceCount = 0;


        for (int i = 0; i < offsetBuildedVoxels.Length; i++)
        {
            _voxels[GetIndexByPos(offsetSelectedPoss[i])] = offsetBuildedVoxels[i];
        }

        for(int i = 0; i < _buildedIndeces.Count; i++)
        {
            UpdateVoxelByIndex(_buildedIndeces[i]);
            CreateOffsetsVerticesByIndex(_buildedIndeces[i]);
        }

        for (int i = 0; i < offsetSelectedPoss.Length; i++)
        {
            SelectVoxel(offsetSelectedPoss[i]);
        }

        middleSelectedPos += offsetInt;

        //SceneData.dragSystem.ResetDragValue();
        //SceneData.dragSystem.SetPosition(middleSelectedPos);
        SceneData.dragSystem.OffsetPosition(offsetInt);

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
        for (int i = 0; i < _buildedIndeces.Count; i++)
        {
            Voxel curVoxel = _voxels[_buildedIndeces[i]];
            for (int j = 0; j < curVoxel.availabilityVertices.Length; j++)
            {
                if (curVoxel.availabilityVertices[j])
                {
                    int _i = j * 4;
                    Vector3 verPos = curVoxel.position;
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
                        vertices[i4 + v] = SceneData.voxelVertices[_i + v] + verPos;
                        OffsetVertexByPos(ref vertices[i4 + v]);
                    }

                    float v1 = SceneData.textureMul * (curVoxel.id % SceneData.textureSize);
                    float v2 = 1 - SceneData.textureMul * (curVoxel.id / (SceneData.textureSize + 1));
                    uv[i4 + 0] = new Vector2(v1, v2 - SceneData.textureMul);
                    uv[i4 + 1] = new Vector2(v1, v2);
                    uv[i4 + 2] = new Vector2(v1 - SceneData.textureMul, v2);
                    uv[i4 + 3] = new Vector2(v1 - SceneData.textureMul, v2 - SceneData.textureMul);

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

        for (int i = 0; i < _selectedIndeces.Count; i++)
        {
            Voxel curVoxel = _selectedVoxels[_selectedIndeces[i]];
            for (int j = 0; j < curVoxel.availabilityVertices.Length; j++)
            {
                if (curVoxel.availabilityVertices[j])
                {
                    int _i = j * 4;
                    Vector3 verPos = curVoxel.position;

                    for (int v = 0; v <= 3; v++)
                    {
                        vertices[i4 + v] = (SceneData.voxelVertices[_i + v] + verPos);
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
            for (int i = 0; i < _buildedIndeces.Count; i++)
            {
                Vector3 pos = _voxels[_buildedIndeces[i]].position;
                
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
