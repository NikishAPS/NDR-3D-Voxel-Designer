using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField]
    private Voxel _voxel;

    [SerializeField]
    private Vector3Int _size;

    [SerializeField]
    private Voxel[] _voxels;

    public Vector3 Center
    {
        get { return SceneData.Vector3IntToFloat(_size) * 0.5f; }
    }

    public Vector3 Size
    {
        get { return SceneData.Vector3IntToFloat(_size); }
    }

    public bool InChunk(Vector3Int pos)
    {
        return pos.x >= 0 && pos.x < SceneData.chunk.Size.x &&
            pos.y >= 0 && pos.y < SceneData.chunk.Size.y &&
            pos.z >= 0 && pos.z < SceneData.chunk.Size.z;
    }


    public void Resize()
    {
        transform.localScale = _size;
        SceneData.grid.Resize(_size);

        _voxels = new Voxel[_size.x * _size.y * _size.z];
    }

    public void SetSizeX(UnityEngine.UI.InputField inputField)
    {
        _size.x = int.Parse(inputField.text);
        Resize();
    }

    public void SetSizeY(UnityEngine.UI.InputField inputField)
    {
        _size.y = int.Parse(inputField.text);
        Resize();
    }

    public void SetSizeZ(UnityEngine.UI.InputField inputField)
    {
        _size.z = int.Parse(inputField.text);
        Resize();
    }

    public int GetIndexByPos(int x, int y, int z)
    {
        //(x * Y + y) * Z + z
        return (x * _size.y + y) * _size.z + z;
    }

    public int GetIndexByPos(Vector3 pos)
    {
        return ((int)pos.x * _size.y + (int)pos.y) * _size.z + (int)pos.z;
    }

    public Voxel GetVoxelByPos(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= Size.x ||
            pos.y < 0 || pos.y >= Size.y ||
            pos.z < 0 || pos.z >= Size.z) return null;

        int index = GetIndexByPos(pos);
        if (index >= _voxels.Length || index < 0) return null;
        return _voxels[GetIndexByPos(pos)];
    }

    public void CreateVoxel(Vector3 pos)
    {
        Vector3Int posInt = SceneData.Vector3FloatToInt(pos);

        Voxel newVoxel = Instantiate(_voxel, posInt, Quaternion.identity);
        newVoxel.Init();

        Voxel adjacentVoxel = new Voxel();

       
        adjacentVoxel = GetVoxelByPos(posInt + Vector3Int.left);
        if (!adjacentVoxel) newVoxel.AddLeftFace();
        else
        {
            adjacentVoxel.DeleteRightFace();
            adjacentVoxel.UpdateMesh();
            //print("Left");
        }

        adjacentVoxel = GetVoxelByPos(posInt + Vector3Int.right);
        if (!adjacentVoxel) newVoxel.AddRightFace();
        else
        {
            adjacentVoxel.DeleteLeftFace();
            adjacentVoxel.UpdateMesh();
            //print("Right");
        }

        adjacentVoxel = GetVoxelByPos(posInt + Vector3Int.down);
        if (!adjacentVoxel) newVoxel.AddBottomFace();
        else
        {
            adjacentVoxel.DeleteTopFace();
            adjacentVoxel.UpdateMesh();
            //print("Bottom");
        }


        adjacentVoxel = GetVoxelByPos(posInt + Vector3Int.up);
        if (!adjacentVoxel) newVoxel.AddTopFace();
        else
        {
            adjacentVoxel.DeleteBottomFace();
            adjacentVoxel.UpdateMesh();
            //print("Top");
        }


        adjacentVoxel = GetVoxelByPos(posInt + new Vector3Int(0, 0, -1));
        if (!adjacentVoxel) newVoxel.AddRearFace();
        else
        {
            adjacentVoxel.DeleteFrontFace();
            adjacentVoxel.UpdateMesh();
            //print("Rear");
        }

        adjacentVoxel = GetVoxelByPos(posInt + new Vector3Int(0, 0, 1));
        if (!adjacentVoxel) newVoxel.AddFrontFace();
        else
        {
            adjacentVoxel.DeleteRearFace();
            adjacentVoxel.UpdateMesh();
            //print("Front");
        }

        newVoxel.UpdateMesh();

        _voxels[GetIndexByPos(posInt)] = newVoxel;

    }
}
