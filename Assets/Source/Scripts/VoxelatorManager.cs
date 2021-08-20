using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VoxelatorManager
{
    public static readonly Project Project;
    public static readonly Coordinates Coordinates;

    static VoxelatorManager()
    {
        Project = Object.FindObjectOfType<Project>();
        Coordinates = Object.FindObjectOfType<Coordinates>();
    }

    public static int GetIndex(Vector3Int arraySize, Vector3Int position)
    {
        return 
            /*if*/      WithinTheArray(arraySize, position) ?
            /*then*/    (position.x * arraySize.y + position.y) * arraySize.z + position.z :
            /*else*/    -1;

    }

    public static bool WithinTheArray(Vector3Int arraySize, Vector3 position)
    {
        return
            position.x >= 0 && position.x < arraySize.x &&
            position.y >= 0 && position.y < arraySize.y &&
            position.z >= 0 && position.z < arraySize.z;
    }

}
