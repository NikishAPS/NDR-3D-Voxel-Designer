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
    private List<int> _buildedIndeces = new List<int>();
    private List<int> _selectedIndeces = new List<int>();

    private Mesh _mesh;
    private Mesh _selectedMesh;

    private int _faceCount;

    public Vector3 Center
    {
        get { return SceneData.Vector3IntToFloat(_size) * 0.5f; }
    }

    public Vector3 Size
    {
        get { return SceneData.Vector3IntToFloat(_size); }
    }

    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;

        _selectedMesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh = _selectedMesh;
    }

    public bool InChunk(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < Size.x &&
            pos.y >= 0 && pos.y < Size.y &&
            pos.z >= 0 && pos.z < Size.z;
    }

    public void Resize()
    {
        _voxels = new Voxel[_size.x * _size.y * _size.z];
        //for (int i = 0; i < _voxels.Length; i++) _voxels[i] = new Voxel();
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
        return _voxels[GetIndexByPos(pos)];
    }

    private void CreateVoxelMesh(List<int> indeces, List<Voxel> voxels, Vector3Int pos)
    {
        int index = GetIndexByPos(pos);

        if (GetVoxelByPos(pos) != null) return;

        Voxel newVoxel = new Voxel(0, SceneData.Vector3IntToPoint3Int(pos));
        indeces.Add(index);
        _voxels[index] = newVoxel;

        //print(GetIndexByPos(posInt));

        Voxel adjacentVoxel =

        GetVoxelByPos(pos + Vector3Int.left);
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
        else { adjacentVoxel.SetBottomFace(false); _faceCount--; }

        adjacentVoxel = GetVoxelByPos(pos + new Vector3Int(0, 0, -1));
        if (adjacentVoxel == null) { newVoxel.SetRearFace(true); _faceCount++; }
        else { adjacentVoxel.SetFrontFace(false); _faceCount--; }

        adjacentVoxel = GetVoxelByPos(pos + new Vector3Int(0, 0, 1));
        if (adjacentVoxel == null) { newVoxel.SetFrontFace(true); _faceCount++; }
        else { adjacentVoxel.SetRearFace(false); _faceCount--; }

    }

    public void CreateVoxel(int id, Vector3Int pos)
    {
        int index = GetIndexByPos(pos);

        if (GetVoxelByPos(pos) != null) return;

        Voxel newVoxel = new Voxel(id, SceneData.Vector3IntToPoint3Int(pos));
        _buildedIndeces.Add(index);
        _voxels[index] = newVoxel;
        
        //print(GetIndexByPos(posInt));

        Voxel adjacentVoxel = 

        GetVoxelByPos(pos + Vector3Int.left);
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
                    Vector3 verPos = SceneData.Point3IntToVector3(curVoxel.position);
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
}
