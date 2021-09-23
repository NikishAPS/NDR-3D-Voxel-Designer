using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ChunkManagerPresenter
{
    public static int VoxelId
    {
        get
        {
            return ChunksManager.VoxelId;
        }
        set
        {
            ChunksManager.VoxelId = value;
        }
    }
}
