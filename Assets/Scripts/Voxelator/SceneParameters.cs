using UnityEngine;

public static class SceneParameters
{
    //parameters
    public static readonly Vector3Int ChunkSize = new Vector3Int(1, 1, 1) * 64;
    public static readonly float RayLength = 300f;
    public static readonly int TextureSize = 4096;
    public static readonly float TextureMul = 1.0f / TextureSize;

}
