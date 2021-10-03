using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Raycast
{
    public static CastResult CastByMouse(float length)
    {
        Vector3Int lastPointInt = new Vector3Int(-1, -1, -1);

        Vector2 mousePos = InputEvent.MousePosition;
        Vector3 pos = Camera.main.ScreenToWorldPoint(InputEvent.MousePosition);
        Vector3 dir = Camera.main.transform.forward;

        pos += dir * Camera.main.nearClipPlane;

        Vector3 direction = Vector3.zero;
        Vector3 point = Vector3.zero;
        Vector3Int pointInt = Vector3Int.zero;

        Voxel curVoxel = null;
        Voxel lastVoxel = null;

        for (float f = 0; f < length; f += SceneData.RayStep)
        {
            direction = dir * f;
            point = pos + direction;

            //pointInt = new Vector3Int((int)Mathf.Round(point.x), (int)Mathf.Round(point.y), (int)Mathf.Round(point.z));
            pointInt = point.RoundToInt();

            //if (pointInt.y < 0 && lastPointInt.y == 0)
            
            //if is gird
            if(!GridManager.IsGrid(pointInt) && GridManager.IsGrid(lastPointInt))
            {
                return new CastResult(null, null, lastPointInt, pointInt);
            }

            //lastPointInt = pointInt;
            //continue;

            //curVoxel = SceneData.Chunk.GetVoxelByPos(pointInt);
            curVoxel = ChunkManager.GetVoxel(pointInt);

            if (curVoxel != null)
            {
                //lastVoxel = SceneData.Chunk.GetVoxelByPos(lastPointInt);
                lastVoxel = ChunkManager.GetVoxel(lastPointInt);

                return new CastResult(lastVoxel, curVoxel, lastPointInt, pointInt);

                //voxel = curVoxel;
                //result = lastPointInt;
            }


            //if (SceneData.GridManager.IsGrid(pointInt))
            //{
            //    return new CastResult(null, null, lastPointInt, pointInt);
            //}

            lastPointInt = pointInt;
        }

        return null;
        //return voxel;
    }

    public static CastVertexResult CastVertexByMouse(float length)
    {
        Vector2 mousePos = InputEvent.MousePosition;
        Vector3 startPoint = Camera.main.ScreenToWorldPoint(InputEvent.MousePosition); 
        Vector3 dir = Camera.main.transform.forward;

        startPoint += dir * Camera.main.nearClipPlane; 

        Vector3 ray = Vector3.zero;
        Vector3 point = startPoint;
        Vector3Int pointInt = Vector3Int.zero;

        Vector3 startVertex = Vector3.zero;
        Vector3 endVertex = Vector3.zero;

        Vertex vertex = null;

        for (float f = 0; f < length; f += SceneData.RayStep )
        {
            ray = dir * f;
            point = startPoint + ray;
            pointInt = point.RoundToInt();

            startVertex = pointInt - new Vector3(2.5f, 2.5f, 2.5f);
            endVertex = pointInt + new Vector3(2.5f, 2.5f, 2.5f);

            for (float x = startVertex.x; x <= endVertex.x; x++)
            {
                for (float y = startVertex.y; y <= endVertex.y; y++)
                {
                    for (float z = startVertex.z; z <= endVertex.z; z++)
                    {
                        vertex = ChunkManager.GetVertex(new Vector3(x, y, z));

                        if (vertex != null)
                        {
                            if (Vector3.Distance(vertex.Position, point) < 0.1f)
                            {
                                return new CastVertexResult(-1, vertex);
                            }
                        }
                    }
                }
            }

            //if (point.magnitude < pointInt.magnitude) point = pointInt;
        }

        return null;
    }

}



public class CastVertexResult
{
    public readonly int Index;
    public readonly Vertex Vertex;

    public CastVertexResult(int index, Vertex vertex)
    {
        Index = index;
        Vertex = vertex;
    }
}

public class CastResult
{
    public readonly Voxel lastVoxel;
    public readonly Voxel voxel;

    public readonly Vector3Int lastPoint;
    public readonly Vector3Int point;

    public readonly Vector3? vertex;

    public CastResult()
    {
        this.lastVoxel = null;
        this.voxel = null;
        this.lastPoint = Vector3Int.zero;
        this.point = Vector3Int.zero;
    }

    public CastResult(Voxel lastVoxel, Voxel voxel, Vector3Int lastPoint, Vector3Int point, Vector3? vertex = null)
    {
        this.lastVoxel = lastVoxel;
        this.voxel = voxel;
        this.lastPoint = lastPoint;
        this.point = point;
        this.vertex = vertex;
    }

}



/*
 *   posRay = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            dirRay = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) - Camera.main.transform.position;
            dirRay.Normalize();
 * */
