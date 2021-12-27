using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectVoxelTest : MonoBehaviour
{
    public int ValueOffset = 1;

    void Update()
    {
        Vector3 dragResult;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Voxelator.DragSelectedVoxels(Vector3Int.left * ValueOffset, out dragResult);
            Voxelator.UpdateChunks();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Voxelator.DragSelectedVoxels(Vector3Int.right * ValueOffset, out dragResult);
            Voxelator.UpdateChunks();
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Voxelator.DragSelectedVoxels(Vector3Int.down * ValueOffset, out dragResult);
            Voxelator.UpdateChunks();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Voxelator.DragSelectedVoxels(Vector3Int.up * ValueOffset, out dragResult);
            Voxelator.UpdateChunks();
        }

    }
}
