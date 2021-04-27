using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVoxelDelete : MonoBehaviour
{

    public Chunk chunk;
    void Start()
    {
        chunk = GetComponent<Chunk>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        for(int x = 0; x < chunk.Size.x; x++)
        {
            for (int y = 0; y < chunk.Size.y; y++)
            {
                for (int z = 0; z < chunk.Size.z; z++)
                {
                    chunk.CreateVoxel(1, new Vector3Int(x, y, z));
                }
            }
        }

    }
}
