using UnityEngine;

public static class VoxelRaycast
{
    public static CastResult Cast(Vector3 origin, Vector3 direction, float distance)
    {
        direction.Normalize();
        Vector3Int previousVoxelPosition = origin.RoundToInt();
        Vector3Int currentVoxelPosition = previousVoxelPosition;
        float currentRayLength = 0;

        Vector3 stepsByDirection = new Vector3(
            GetHypotenuse(direction, Vector3.right),
            GetHypotenuse(direction, Vector3.up),
            GetHypotenuse(direction, Vector3.forward)
            );

        Vector3Int stepsByVoxels = direction.Sign().ToVector3Int();

        /*Since the beam can have its origin at any point inside the voxel,
        the first step is likely to be smaller than the rest of the steps.
        delta is a multiplier for the first step (0 < delta <= 1)*/
        Vector3 delta = (currentVoxelPosition + Vector3.one.Mul(stepsByVoxels) * 0.5f - origin).Abs();
        Vector3 lengths = stepsByDirection.Mul(delta);

        while (currentRayLength < distance)
        {
            previousVoxelPosition = currentVoxelPosition;

            if (lengths.x < lengths.y && lengths.x < lengths.z)
            {
                currentRayLength = lengths.x;
                lengths.x += stepsByDirection.x;
                currentVoxelPosition.x += stepsByVoxels.x;
            }
            else if (lengths.y < lengths.x && lengths.y < lengths.z)
            {
                currentRayLength = lengths.y;
                lengths.y += stepsByDirection.y;
                currentVoxelPosition.y += stepsByVoxels.y;
            }
            else
            {
                currentRayLength = lengths.z;
                lengths.z += stepsByDirection.z;
                currentVoxelPosition.z += stepsByVoxels.z;
            }

            //if a collision with a voxel or a voxel collider (ex. grid)
            if (ChunkManager.GetVoxel(currentVoxelPosition) != null || VoxelBoxCollider.IsCollision(currentVoxelPosition))
            {
                if (ChunkManager.InField(previousVoxelPosition) && ChunkManager.GetVoxel(previousVoxelPosition) == null)
                {
                    return new CastResult(origin + direction * currentRayLength, previousVoxelPosition, currentVoxelPosition);
                }
            }
        }

        return null;
    }

    public static CastResult CastByMouse(float length)
    {
        return Cast(CameraController.WorldMouse, CameraController.ViewDirection, length);
    }

    public static VertexCastResult VertexCast(Vector3 origin, Vector3 direction, float distance)
    {
        direction.Normalize();
        Vector3Int previousVoxelPosition = origin.RoundToInt();
        Vector3Int currentVoxelPosition = previousVoxelPosition;
        float currentRayLength = 0;

        Vector3 stepsByDirection = new Vector3(
            GetHypotenuse(direction, Vector3.right),
            GetHypotenuse(direction, Vector3.up),
            GetHypotenuse(direction, Vector3.forward)
            );

        Vector3Int stepsByVoxels = direction.Sign().ToVector3Int();

        Vector3 delta = (currentVoxelPosition + Vector3.one.Mul(stepsByVoxels) * 0.5f - origin).Abs();
        Vector3 lengths = stepsByDirection.Mul(delta);

        Vertex vertex = null;

        while (currentRayLength < distance)
        {
            previousVoxelPosition = currentVoxelPosition;

            if (lengths.x < lengths.y && lengths.x < lengths.z)
            {
                currentRayLength = lengths.x;
                lengths.x += stepsByDirection.x;
                currentVoxelPosition.x += stepsByVoxels.x;
            }
            else if (lengths.y < lengths.x && lengths.y < lengths.z)
            {
                currentRayLength = lengths.y;
                lengths.y += stepsByDirection.y;
                currentVoxelPosition.y += stepsByVoxels.y;
            }
            else
            {
                currentRayLength = lengths.z;
                lengths.z += stepsByDirection.z;
                currentVoxelPosition.z += stepsByVoxels.z;
            }

            vertex = FindNearestVertex(origin + direction * currentRayLength);
            if (vertex != null)
                return new VertexCastResult(vertex);
        }

        return null;
    }

    public static VertexCastResult VertexCastByMouse(float length)
    {
        return VertexCast(CameraController.WorldMouse, CameraController.ViewDirection, length);
    }

    private static float GetHypotenuse(Vector3 rayDirection, Vector3 axisDirection)
    {
        float angle = Vector3.Angle(rayDirection, axisDirection) * Mathf.Deg2Rad;
        float cos = Mathf.Abs(Mathf.Cos(angle));

        return cos == 0 ? Mathf.Infinity : axisDirection.magnitude / cos;
    }

    private static Vertex FindNearestVertex(Vector3 position)
    {
        Vector3Int voxelPosition = position.RoundToInt();
        Vector3 startPoint = voxelPosition - Vector3.one * 0.5f;
        Vector3 endPoint = startPoint + Vector3.one;

        float minDistance = 0.2f;
        float currentDistance = 0;
        Vertex vertex = null;
        Vertex currentVertex = null;

        for (float x = startPoint.x; x <= endPoint.x; x++)
        {
            for (float y = startPoint.y; y <= endPoint.y; y++)
            {
                for (float z = startPoint.z; z <= endPoint.z; z++)
                {
                    Vector3 currentVertexPosition = new Vector3(x, y, z);
                    currentVertex = ChunkManager.GetVertex(currentVertexPosition);
                    if (currentVertex != null)
                    {
                        currentDistance = Vector3.Distance(currentVertex.Position, position);
                        if (currentDistance < minDistance)
                        {
                            vertex = currentVertex;
                            minDistance = currentDistance;
                        }
                    }
                }
            }
        }
        return vertex;

    }

}
