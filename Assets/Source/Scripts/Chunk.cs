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
    private Vector3[] _offsetsVertices;

    [SerializeField]
    private List<int> _buildedIndeces = new List<int>();
    [SerializeField]
    private List<int> _selectedIndeces = new List<int>();

    private Mesh _mesh;
    private Mesh _selectedMesh;

    [SerializeField]
    private int _faceCount;
    [SerializeField]
    private int _selectedFaceCount;

    [SerializeField]
    private Vector3 middleSelectedPos;

    public Vector3 Center
    {
        get { return SceneData.Vector3IntToFloat(_size) * 0.5f; }
    }

    public Vector3 Size
    {
        get { return SceneData.Vector3IntToFloat(_size); }
    }

    public int SelectedIndecesCount => _selectedIndeces.Count;

    public Vector3 MiddleSelectedPosition => middleSelectedPos;

    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _selectedMesh = new Mesh();
        transform.GetChild(0).GetComponent<MeshFilter>().mesh = _selectedMesh;
    }

    public bool InChunk(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < Size.x &&
            pos.y >= 0 && pos.y < Size.y &&
            pos.z >= 0 && pos.z < Size.z;
    }

    public void Resize()
    {
        int size = _size.x * _size.y * _size.z;

        _voxels = new Voxel[size];
        _selectedVoxels = new Voxel[size];
        _offsetsVertices = new Vector3[size];

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

        UpdateMesh();
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
                _voxels[_selectedIndeces[i]] = null;
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
        }

        for(int i = 0; i < offsetSelectedPoss.Length; i++)
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

    private void UpdateMesh()
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
                    vertices[i4 + 0] = SceneData.voxelVertices[_i + 0] + verPos;
                    vertices[i4 + 1] = SceneData.voxelVertices[_i + 1] + verPos;
                    vertices[i4 + 2] = SceneData.voxelVertices[_i + 2] + verPos;
                    vertices[i4 + 3] = SceneData.voxelVertices[_i + 3] + verPos;

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
                    float mul = 1.0f;
                    vertices[i4 + 0] = (SceneData.voxelVertices[_i + 0] + verPos) * mul;
                    vertices[i4 + 1] = (SceneData.voxelVertices[_i + 1] + verPos) * mul;
                    vertices[i4 + 2] = (SceneData.voxelVertices[_i + 2] + verPos) * mul;
                    vertices[i4 + 3] = (SceneData.voxelVertices[_i + 3] + verPos) * mul;

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
}
