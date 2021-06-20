using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public MeshCollider meshCollider { get; private set; }

    public Vector3 Size => transform.localScale;
    
    public void Resize(Vector3 size)
    {
        transform.localScale = new Vector3(size.x, 1, size.z);
    }
}
