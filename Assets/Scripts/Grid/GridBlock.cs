using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class GridBlock : MonoBehaviour
{
    public bool Active
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }
    public Vector3Int Position
    {
        get => transform.position.ToVector3Int();
        set => transform.position = value;
    }
    public Vector3Int Size
    {
        get => _bounds.Size;
        set
        {
            _bounds = new BoundsInt(Vector3Int.zero, value.Mul(Vector3Int.one - _normal));
            GenerateMesh();
        }
    }

    private BoundsInt _bounds;

    [SerializeField] private DirectionType _directionType;
    private Vector3Int _normal;
    private Mesh _mesh;

    public void SetBounds(Vector3Int firstPoint, Vector3Int secondPoint)
    {
        _bounds = new BoundsInt(firstPoint, secondPoint);
    }

    public void GenerateMesh()
    {
        Vector3Int[] points = new Vector3Int[2];

        for (int i = 1, j = 0; i < Direction.Directions.Length; i += 2)
        {
            if (Vector3.Angle(_normal, Direction.Directions[i]) == 90)
            {
                points[j] = Direction.Directions[i].Mul(Size);
                j++;
            }
        }

        CustomMesh plane = MeshGenerator.GenerateMesh(new Vector3[]
            {
                Vector3.zero,
                points[0],
                points[0] + points[1],
                points[1]
            });

        print(points[0]);
        print(points[1]);
        _mesh.SetCustomMesh(plane);

        _mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = _mesh;

        GetComponent<MeshRenderer>()
            .material
            .SetTextureScale("_MainTex", new Vector2(Size.x, Size.z));
    }

    public bool Inside(Vector3Int position)
    {
        return _bounds.Contains(Position + position);
    }

    private void Awake()
    {
        _normal = Direction.GetDirectionByType(_directionType);
        _mesh = new Mesh();
    }
}
