﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Raycast
{
    public static CastResult CastByMouse(float length)
    {
        Vector3Int lastPointInt = new Vector3Int(-1, -1, -1);

        Vector2 mousePos = InputEvent.MousePosition;
        Vector3 pos = SceneData.Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        Vector3 dir = SceneData.Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1)) - SceneData.Camera.transform.position;
        dir.Normalize();

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
            curVoxel = ChunksManager.GetVoxel(pointInt);

            if (curVoxel != null)
            {
                //lastVoxel = SceneData.Chunk.GetVoxelByPos(lastPointInt);
                lastVoxel = ChunksManager.GetVoxel(lastPointInt);

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
        Vector3 startPoint = GetStartPoint(InputEvent.MousePosition); 
        Vector3 dir = GetDirection();

        Vector3 ray = Vector3.zero;
        Vector3 point = startPoint;
        Vector3Int pointInt = Vector3Int.zero;

        Vector3 startVertex = Vector3.zero;
        Vector3 endVertex = Vector3.zero;

        Vertex vertex = null;

        for (float f = 0; f < length; f += SceneData.RayStep)
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
                        vertex = ChunksManager.GetVertex(new Vector3(x, y, z));

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

            if (point.magnitude < pointInt.magnitude) point = pointInt;
        }

        return null;
    }

    public static CastResult CastVerticesByMouse0(float length)
    {
        Vector2 mousePos = InputEvent.MousePosition;
        Vector3 pos = SceneData.Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        Vector3 dir = SceneData.Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1)) - SceneData.Camera.transform.position;
        dir.Normalize();


        //Vector3 direction = Vector3.zero;
        //Vector3 point = Vector3.zero;
        //Vector3Int pointInt = Vector3Int.zero;
        //int index = 0;
        //Vector3? vertex = null;
        //bool isVoxel = false;
        //for (float f = 0; f < length; f += SceneData.RayStep * 0.1f)
        //{
        //    direction = dir * f;
        //    point = pos + direction;

        //    //Round 
        //    //1.2 -> 1.5
        //    //0.9 -> 0.5
        //    pointInt = SceneData.Vector3FloatRound(point - Vector3.one * 0.5f);
        //    point = pointInt + Vector3.one * 0.5f;


        //    index = SceneData.Chunk.GetVertexIndexByPos(point);
        //    vertex = SceneData.Chunk.GetOffsetVertexByPos(point);

        //    if (index != -1 && vertex != null)
        //    {
        //        isVoxel = true;
        //        if ((pos + direction - point).magnitude > 0.25f) continue;

        //        return new CastResult(null, null, Vector3Int.zero, pointInt, vertex);
        //    }
        //    else if (isVoxel) return null;

        //}

        return null;
    }

    //public static CastResult Cast(Vector3 pos, Vector3 dir, float length)
    //{
    //    //Voxel voxel = null;


    //    Vector3Int lastPointInt = new Vector3Int(-1, -1, -1);


    //    for (float f = 0; f < length; f += 0.1f)
    //    {
    //        Vector3 direction = dir * f;
    //        Vector3 point = pos + direction;

    //        Vector3Int pointInt = new Vector3Int((int)Mathf.Round(point.x), (int)Mathf.Round(point.y), (int)Mathf.Round(point.z));

    //        if (pointInt.y < 0 && lastPointInt.y == 0)
    //        {
    //            return new CastResult(null, null, lastPointInt, pointInt);
    //        }

    //        Voxel curVoxel = SceneData.Chunk.GetVoxelByPos(pointInt);

    //        if (curVoxel != null)
    //        {
    //            Voxel lastVoxel = SceneData.Chunk.GetVoxelByPos(lastPointInt);

    //            return new CastResult(lastVoxel, curVoxel, lastPointInt, pointInt);

    //            //voxel = curVoxel;
    //            //result = lastPointInt;
    //        }

    //        lastPointInt = pointInt;
    //    }

    //    return null;
    //    //return voxel;
    //}



    private static Vector3 GetStartPoint(Vector3 mousePos)
    {
        return SceneData.Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
    }

    private static Vector3 GetDirection()
    {
        Vector2 mousePos = InputEvent.MousePosition;
        Vector3 dir = SceneData.Camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1)) - SceneData.Camera.transform.position;
        dir.Normalize();

        return dir;
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