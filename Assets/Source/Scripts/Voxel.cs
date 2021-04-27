using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public readonly int id;
    public readonly Point3Int position;
    public readonly bool[] availabilityVertices = new bool[0];

    public Voxel()
    {
        id = 0;
        position = new Point3Int(0,0,0);
        availabilityVertices = new bool[0];
    }

    public Voxel(int id, Point3Int position)
    {
        this.id = id;
        this.position = position;
        availabilityVertices = new bool[6];
    }

    public void SetLeftFace(bool flag)
    {
        availabilityVertices[0] = flag;
    }

    public void SetRightFace(bool flag)
    {
        availabilityVertices[1] = flag;
    }

    public void SetBottomFace(bool flag)
    {
        availabilityVertices[2] = flag;
    }

    public void SetTopFace(bool flag)
    {
        availabilityVertices[3] = flag;
    }

    public void SetRearFace(bool flag)
    {
        availabilityVertices[4] = flag;
    }

    public void SetFrontFace(bool flag)
    {
        availabilityVertices[5] = flag;
    }
}
