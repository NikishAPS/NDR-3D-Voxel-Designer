using UnityEngine;

public static class SceneParameters
{
    //parameters
    public static readonly Vector3Int VoxelChunkSize = Vector3Int.one * 64;
    public static readonly Vector3Int VertexChunkSize = Vector3Int.one * 64;
    public static readonly float VertexConvexity = 0.005f;
    public static readonly float RayLength = 300f;
    public static readonly int TextureSize = 4096;
    public static readonly float TextureMul = 1.0f / TextureSize;
    public static readonly float MinVertexDetectionDistance = 0.2f;

}
