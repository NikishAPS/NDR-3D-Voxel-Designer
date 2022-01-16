using UnityEngine;
using UnityEditor;

public class VoxelatorArray
{
    public static int GetFlatArraySize(Vector3Int volumeArraySize)
    {
        return volumeArraySize.x * volumeArraySize.y * volumeArraySize.z;
    }

    public static int GetIndex(Vector3Int volumeArraySize, Vector3Int position)
    {
        return
            /*if*/      WithinTheArray(volumeArraySize, position) ?
            /*then*/    (position.x * volumeArraySize.y + position.y) * volumeArraySize.z + position.z :
            /*else*/    -1;

    }

    public static Vector3Int GetPosition(Vector3Int volumeArraySize, int zIndex)
    {
        //        nz = i % vz
        //ny = ((i - nz) / vz) % vy
        //nx = ((i - nz) / vz - ny) / vy


        int z = zIndex % volumeArraySize.z;
        int y = (zIndex - z) / volumeArraySize.z % volumeArraySize.y;
        int x = ((zIndex - z) / volumeArraySize.z - y) / volumeArraySize.y;

        return new Vector3Int(x, y, z);
    }

    public static bool WithinTheArray(Vector3Int volumeArraySize, Vector3 position)
    {
        return
            position.x >= 0 && position.x < volumeArraySize.x &&
            position.y >= 0 && position.y < volumeArraySize.y &&
            position.z >= 0 && position.z < volumeArraySize.z;
    }

}