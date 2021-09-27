using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkManagerPresenter
{
    public static int VoxelId
    {
        get
        {
            return ChunkManager.VoxelId;
        }
        set
        {
            ChunkManager.VoxelId = value;
        }
    }
}
