using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public readonly int Id;
    public readonly Vector3Int Position;
    public readonly bool[] Faces = new bool[0];
    public int FaceCount { get; private set; }

    public Voxel()
    {
        Id = 0;
        Position = new Vector3Int(0, 0, 0);
        Faces = new bool[0];
    }

    public Voxel(int id, Vector3Int position)
    {
        Id = id;
        Position = position;
        Faces = new bool[6];
    }

    public Voxel(int id, Vector3Int position, bool[] faces)
    {
        Id = id;
        Position = position;
        Faces = faces;
        for (int i = 0; i < Faces.Length; i++) if (Faces[i]) FaceCount++;
    }

    public Voxel(Voxel voxel)
    {
        Id = voxel.Id;
        Position = voxel.Position;
        FaceCount = voxel.FaceCount;
        Faces = voxel.Faces;
        FaceCount = voxel.FaceCount;
    }

    public void SetLeftFace(bool active)
    {
        if (Faces[0] != active) CheckFace(active);
        Faces[0] = active;
    }

    public void SetRightFace(bool active)
    {
        if (Faces[1] != active) CheckFace(active);
        Faces[1] = active;
    }

    public void SetBottomFace(bool active)
    {
        if (Faces[2] != active) CheckFace(active);
        Faces[2] = active;
    }

    public void SetTopFace(bool active)
    {
        if (Faces[3] != active) CheckFace(active);
        Faces[3] = active;
    }

    public void SetRearFace(bool active)
    {
        if (Faces[4] != active) CheckFace(active);
        Faces[4] = active;
    }

    public void SetFrontFace(bool active)
    {
        if (Faces[5] != active) CheckFace(active);
        Faces[5] = active;
    }

    public void SetFaceActive(int faceIndex, bool active)
    {
        if (faceIndex < 0 || faceIndex > Faces.Length) return;

        if (active != Faces[faceIndex])
            FaceCount += active ? +1 : -1;

        Faces[faceIndex] = active;
    }

    private void CheckFace(bool active)
    {
        FaceCount += active ? 1 : -1;
    }
}
