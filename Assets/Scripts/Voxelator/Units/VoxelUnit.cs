using UnityEngine;

public class VoxelUnit : Unit
{
    public readonly Vector3Byte Color;
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
    public bool Selected { get; private set; }

    public static int Count { get; private set; }
    public static VoxelUnit Head { get; private set; }
    public VoxelUnit Prev { get; private set; }
    public VoxelUnit Next { get; private set; }
    public static Vector3 MiddlePosition => (Count > 0) ? _commonPosition.ToVector3() / Count : -Vector3.zero;

    public static int SelectedCount { get; private set; }
    public static VoxelUnit SelectedHead { get; private set; }
    public VoxelUnit SelectedPrev { get; private set; }
    public VoxelUnit SelectedNext { get; private set; }
    public static Vector3 MiddleSelectedPosition => (SelectedCount > 0) ? _commonSelectedPosition.ToVector3() / SelectedCount : -Vector3.one;

    private byte _faceFlags;//first bit is a flag of the left face, second - right, etc  

    private static Vector3Int _commonPosition;
    private static Vector3Int _commonSelectedPosition;

    public VoxelUnit(Vector3Int position, Vector3Byte color) : base(position)
    {
        Color = color;
        FacesFlags = 0;
        ConstructConnection();
        _commonPosition += position;
    }

    public void Select(bool select)
    {
        if (Selected == select) return;

        Selected = select;
        if (select)
        {
            ConstructSelectedConnection();
            _commonSelectedPosition += Position;
        }
        else
        {
            DestructSelectedConnection();
            _commonSelectedPosition -= Position;
        }
    }

    public static void ResetSelection()
    {
        VoxelUnit voxelIterator = SelectedHead;
        VoxelUnit currentVoxel;
        while (voxelIterator != null)
        {
            currentVoxel = voxelIterator;
            voxelIterator = voxelIterator.SelectedPrev;

            currentVoxel.Selected = false;
            currentVoxel.SelectedPrev = null;
            currentVoxel.SelectedNext = null;
        }

        SelectedCount = 0;
        _commonSelectedPosition = Vector3Int.zero;
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

        if (Selected)
        {
            DestructSelectedConnection();
            _commonSelectedPosition -= Position;
        }

        _commonPosition -= Position;
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
        if (SelectedPrev != null) SelectedPrev.SelectedNext = SelectedNext;
        if (this == SelectedHead) SelectedHead = SelectedPrev;
        SelectedCount--;
    }

}
