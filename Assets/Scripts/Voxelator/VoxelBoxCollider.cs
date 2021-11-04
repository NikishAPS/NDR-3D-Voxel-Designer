using System;
using System.Collections.Generic;
using UnityEngine;

public class VoxelBoxCollider : IDisposable
{
    public bool Active { get; set; }
    public BoundsInt Bounds { get; set; }

    public static LinkedList<VoxelBoxCollider> Colliders { get; set; }
    private LinkedListNode<VoxelBoxCollider> _node;

    static VoxelBoxCollider()
    {
        Colliders = new LinkedList<VoxelBoxCollider>();
    }

    public VoxelBoxCollider()
    {
        Active = false;
        CreateNode();
        Bounds = new BoundsInt();
    }

    public VoxelBoxCollider(Vector3Int firstPoint, Vector3Int secondPoint)
    {
        Active = true;
        CreateNode();
        Bounds = new BoundsInt(firstPoint, secondPoint);
    }

    public bool Inside(Vector3 position)
    {
        return Active && Bounds.Contains(position);
    }

    public static bool IsCollision(Vector3 position)
    {
        foreach(VoxelBoxCollider collider in Colliders)
        {
            if (collider.Inside(position)) return true;
        }

        return false;
    }

    public void Dispose()
    {
        Colliders.Remove(_node);
    }

    private void CreateNode()
    {
        _node = new LinkedListNode<VoxelBoxCollider>(this);
        Colliders.AddLast(_node);
    }
}

