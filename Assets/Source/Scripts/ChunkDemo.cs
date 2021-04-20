using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkDemo : MonoBehaviour
{
    public GameObject voxel;
    public Material material1, material2;

    public Vector3Int sizeChunk;

    [SerializeField]
    private Voxel[] voxels;

    void Start()
    {
        voxels = new Voxel[sizeChunk.x * sizeChunk.y * sizeChunk.z];
        for(int x = 0; x < sizeChunk.x; x++)
        {
            for (int y = 0; y < sizeChunk.y; y++)
            {
                for (int z = 0; z < sizeChunk.z; z++)
                {
                    voxels[GetIndexByPos(new Vector3Int(x, y, z))] = Instantiate(voxel, new Vector3Int(x,y,z), Quaternion.identity).GetComponent<Voxel>();
                }
            }
        }
    }

    private void Update()
    {
        int index = GetIndexByPos(voxel.transform.position);

        for (int i = 0; i < voxels.Length; i++)
        {
            voxels[i].GetComponent<MeshRenderer>().material = material1;
        }

        if(index < voxels.Length && index > 0)
        voxels[index].GetComponent<MeshRenderer>().material = material2;
    }

    public int GetIndexByPos(Vector3 pos)
    {
        //(x * Y + y) * Z + z
        Vector3Int _pos = new Vector3Int((int)pos.x, (int)pos.y, (int)pos.z);

        return (_pos.x * sizeChunk.y + _pos.y) * sizeChunk.z + _pos.z;
    }

    public Voxel GetVoxelByPos(Vector3Int pos)
    {
        return voxels[GetIndexByPos(pos)];
    }
}
