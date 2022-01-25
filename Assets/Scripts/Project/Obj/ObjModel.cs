using System;
using UnityEngine;

public class ObjModel : MonoBehaviour, IDisposable
{
    public string Path { get; set; }
    public Vector3 Position
    {
        get => _position;
        set 
        {
            _position = value;
            transform.position = value;
        }
    }

    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            transform.localScale = Vector3.one * value;
        }
            
    }

    private Vector3 _position;
    private float _size;

    public void Dispose()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).tag = "OBJ";
            transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer("OBJ");
            transform.GetChild(i).gameObject.AddComponent<MeshCollider>();
        }
    }

}