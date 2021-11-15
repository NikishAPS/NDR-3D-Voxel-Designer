using UnityEngine;

public class CastResult
{
    public readonly Vector3 Point;
    public readonly Vector3Int PreviousVoxelPosition;
    public readonly Vector3Int CurrentVoxelPosition;

    public CastResult()
    {
        Point = Vector3.zero;
        PreviousVoxelPosition = Vector3Int.zero;
        CurrentVoxelPosition = Vector3Int.zero;
    }

    public CastResult(Vector3 point, Vector3Int previousVoxelPosition, Vector3Int currentVoxelPosition)
    {
        Point = point;
        PreviousVoxelPosition = previousVoxelPosition;
        CurrentVoxelPosition = currentVoxelPosition;
    }
}
