using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public readonly int Id;
    public readonly Vector3Int Position;
    public readonly bool[] Faces = new bool[0];

    public Voxel()
    {
        Id = 0;
        Position = new Vector3Int(0, 0, 0);
        Faces = new bool[0];
    }

    public Voxel(int id, Vector3Int position)
    {
        this.Id = id;
        this.Position = position;
        Faces = new bool[6];
    }

    public Voxel(int id, Vector3Int position, bool[] availabilityVertices)
    {
        this.Id = id;
        this.Position = position;
        this.Faces = new bool[6];
        for (int i = 0; i < availabilityVertices.Length; i++) this.Faces[i] = availabilityVertices[i];
    }

    public Voxel(Voxel voxel)
    {
        Id = voxel.Id;
        Position = voxel.Position;
        Faces = new bool[6];
        for (int i = 0; i < Faces.Length; i++) Faces[i] = voxel.Faces[i];
    }

    public void SetLeftFace(bool flag)
    {
        Faces[0] = flag;
    }

    public void SetRightFace(bool flag)
    {
        Faces[1] = flag;
    }

    public void SetBottomFace(bool flag)
    {
        Faces[2] = flag;
    }

    public void SetTopFace(bool flag)
    {
        Faces[3] = flag;
    }

    public void SetRearFace(bool flag)
    {
        Faces[4] = flag;
    }

    public void SetFrontFace(bool flag)
    {
        Faces[5] = flag;
    }
}
