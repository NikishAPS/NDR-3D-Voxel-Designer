using UnityEngine;
using System.Collections;

public class VoxelUnit : Unit
{
    public readonly int Id;
    public int FaceCount { get; private set; }
    public byte FacesFlags
    {
        get => _faceFlags;
        set
        {
            _faceFlags = value;
            FaceCount = _faceFlags.PopCount();
        }
    }
    public bool IsSelected { get; private set; }

    public static int Count { get; private set; }
    public static VoxelUnit Head { get; private set; }
    public VoxelUnit Prev { get; private set; }
    public VoxelUnit Next { get; private set; }

    public static int SelectedCount { get; private set; }
    public static VoxelUnit SelectedHead { get; private set; }
    public VoxelUnit SelectedPrev { get; private set; }
    public VoxelUnit SelectedNext { get; private set; }

    private byte _faceFlags;//first bit is a flag of the left face, second - right, etc  

    public VoxelUnit(Vector3Int position, int id) : base(position)
    {
        Id = id;
        FacesFlags = 0;
        ConstructConnection();
    }

    public void Select(bool select)
    {
        if (IsSelected == select) return;

        IsSelected = select;
        if (select) ConstructSelectedConnection();
        else DestructSelectedConnection();
    }

    public bool CheckFace2(int index)
    {
        return (byte)(_faceFlags & Direction.Masks[index]) != 0;
    }

    public bool CheckFace(int index)
    {
        return CheckFaceByMask(Direction.Masks[index]);
    }

    private bool CheckFaceByMask(byte mask)
    {
        return GetFaceByMask(mask) != 0;
    }

    private byte GetFaceByMask(byte mask)
    {
        return (byte)(_faceFlags & mask);
    }

    public void SetFace(byte mask)
    {
        byte face = GetFaceByMask(mask);
        if (face == 0)
        {
            if (mask != 0)
            {
                FaceCount++;
                _faceFlags |= mask;
            }
        }
        else if (mask == 0)
        {
            _faceFlags &= mask;
        }
    }

    public override void Release()
    {
        DestructConnection();

        if (IsSelected)
            DestructSelectedConnection();
    }

    private void ConstructConnection()
    {
        if (Head != null)
        {
            Head.Next = this;
            Prev = Head;
        }

        Head = this;

        Count++;
    }

    private void DestructConnection()
    {
        if (Prev != null) Prev.Next = Next;
        if (Next != null) Next.Prev = Prev;
        if (this == Head) Head = Prev;

        Count--;
    }

    private void ConstructSelectedConnection()
    {
        if (SelectedHead != null)
        {
            SelectedHead.SelectedNext = this;
            SelectedPrev = SelectedHead;
        }

        SelectedHead = this;

        SelectedCount++;
    }

    private void DestructSelectedConnection()
    {
        if (SelectedNext != null) SelectedNext.SelectedPrev = SelectedPrev;
        if (SelectedPrev != null) SelectedPrev.SelectedNext = Next;
        if (this == SelectedHead) SelectedHead = SelectedPrev;
        SelectedCount--;
    }

}
