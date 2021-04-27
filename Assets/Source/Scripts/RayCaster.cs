using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RayCaster
{
    public static CastResult CastByMouse(float length)
    {
        Vector3Int lastPointInt = new Vector3Int(-1, -1, -1);

        Vector2 mousePos = SceneData.eventInput.MousePos;
        Vector3 pos = SceneData.camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        Vector3 dir = SceneData.camera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1)) - SceneData.camera.transform.position;
        dir.Normalize();

        for (float f = 0; f < length; f += 0.1f)
        {
            Vector3 direction = dir * f;
            Vector3 point = pos + direction;

            Vector3Int pointInt = new Vector3Int((int)Mathf.Round(point.x), (int)Mathf.Round(point.y), (int)Mathf.Round(point.z));

            if (pointInt.y < 0 && lastPointInt.y == 0)
            {
                return new CastResult(null, null, lastPointInt, pointInt);
            }

            Voxel curVoxel = SceneData.chunk.GetVoxelByPos(pointInt);

            if (curVoxel != null)
            {
                Voxel lastVoxel = SceneData.chunk.GetVoxelByPos(lastPointInt);

                return new CastResult(lastVoxel, curVoxel, lastPointInt, pointInt);

                //voxel = curVoxel;
                //result = lastPointInt;
            }

            lastPointInt = pointInt;
        }

        return null;
        //return voxel;
    }

    public static CastResult Cast(Vector3 pos, Vector3 dir, float length)
    {
        //Voxel voxel = null;


        Vector3Int lastPointInt = new Vector3Int(-1, -1, -1);


        for (float f = 0; f < length; f += 0.1f)
        {
            Vector3 direction = dir * f;
            Vector3 point = pos + direction;

            Vector3Int pointInt = new Vector3Int((int)Mathf.Round(point.x), (int)Mathf.Round(point.y), (int)Mathf.Round(point.z));

            if (pointInt.y < 0 && lastPointInt.y == 0)
            {
                return new CastResult(null, null, lastPointInt, pointInt);
            }

            Voxel curVoxel = SceneData.chunk.GetVoxelByPos(pointInt);

            if (curVoxel != null)
            {
                Voxel lastVoxel = SceneData.chunk.GetVoxelByPos(lastPointInt);

                return new CastResult(lastVoxel, curVoxel, lastPointInt, pointInt);

                //voxel = curVoxel;
                //result = lastPointInt;
            }

            lastPointInt = pointInt;
        }

        return null;
        //return voxel;
    }
}

/*
 *   posRay = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            dirRay = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)) - Camera.main.transform.position;
            dirRay.Normalize();
 * */

public class CastResult
{
    public readonly Voxel lastVoxel;
    public readonly Voxel voxel;

    public readonly Vector3Int lastPoint;
    public readonly Vector3Int point;

    public CastResult()
    {
        this.lastVoxel = null;
        this.voxel = null;
        this.lastPoint = Vector3Int.zero;
        this.point = Vector3Int.zero;
    }

    public CastResult(Voxel lastVoxel, Voxel voxel, Vector3Int lastPoint, Vector3Int point)
    {
        this.lastVoxel = lastVoxel;
        this.voxel = voxel;
        this.lastPoint = lastPoint;
        this.point = point;
    }

}

