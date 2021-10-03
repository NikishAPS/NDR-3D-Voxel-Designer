using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManagerModel : MonoBehaviour
{
    public int IncrementOption { get; set; }
    public int VoxelId
    {
        get => _voxelId;
        set
        {
        }
    }
    public static int UVX { get; private set; }
    public static int UVY { get; private set; }

    private int _voxelId;
}
