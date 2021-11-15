using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel
{
    public readonly int Id = 0;
    public readonly Vector3Int Position = Vector3Int.zero;
    public readonly bool[] Faces = new bool[6];
    public int FaceCount { get; private set; }
    public Vector2 UV { get; private set; }
    public Voxel Prev { get; private set; }
    public Voxel Next { get; private set; }
    public static Voxel Head { get; private set; }
    public bool Selected { get; set; }

    public Voxel()
    {
        FaceCount = 0;
        ConstructConnection();
    }

    public Voxel(int id, Vector3Int position)
    {
        Id = id;
        Position = position;
        Faces = new bool[6];
        UpdateUV();
        ConstructConnection();
    }

    public Voxel(int id, Vector3Int position, bool[] faces)
    {
        Id = id;
        Position = position;
        Faces = faces;
        for (int i = 0; i < Faces.Length; i++) if (Faces[i]) FaceCount++;
        UpdateUV();
        ConstructConnection();
    }

    public Voxel(Voxel voxel)
    {
        Id = voxel.Id;
        Position = voxel.Position;
        FaceCount = voxel.FaceCount;
        for (int i = 0; i < Faces.Length; i++)
        {
            Faces[i] = voxel.Faces[i];
        }
        UpdateUV();
        ConstructConnection();
    }

    public Voxel(VoxelData voxelData)
    {
        Id = voxelData.Id;
        Position = voxelData.Position;
        Faces = voxelData.Faces;
        FaceCount = FaceCount;
        UpdateUV();
        ConstructConnection();
    }

    public void Release()
    {
        DestructConnection();
    }

    public void SetFace(int faceIndex, bool active)
    {
        if (Faces[faceIndex] != active) CheckFace(active);
        Faces[faceIndex] = active;
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

    public VoxelData GetData()
    {
        return new VoxelData(Id, Position, Faces, FaceCount);
    }

    private void CheckFace(bool active)
    {
        FaceCount += active ? 1 : -1;
    }

    private void UpdateUV()
    {
        UV = new Vector2(
            ((Id - 1) % SceneParameters.TextureSize + 1) / (float)SceneParameters.TextureSize,
            1 - ((Id - 1) / (SceneParameters.TextureSize) + 1) / (float)SceneParameters.TextureSize
            );
    }

    private void ConstructConnection()
    {
        if (Head != null)
        {
            Head.Next = this;
            Prev = Head;
        }

        Head = this;
    }

    private void DestructConnection()
    {
        if (Prev != null) Prev.Next = Next;
        if (Next != null) Next.Prev = Prev;
        if (this == Head) Head = Prev;
    }
}
