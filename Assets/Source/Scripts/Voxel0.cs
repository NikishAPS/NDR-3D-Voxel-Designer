using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel0 : MonoBehaviour
{

    [SerializeField]
    private Vector3[] _vertices = new Vector3[0];
    [SerializeField]
    private int[] _triangles = new int[9];
    [SerializeField]
    private int _triangleCount = 0;

    private Mesh _mesh;

    public void Init()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        //AddFace();
    }

    private void AddFace()
    {
        Vector3[] newVertices = new Vector3[_vertices.Length + 4];
        for(int i = 0; i < _vertices.Length; i++)
        {
            newVertices[i] = _vertices[i];
        }
        _vertices = newVertices;

        int[] newTriangles = new int[_triangles.Length + 6];
        for(int i = 0; i < _triangles.Length; i++)
        {
            newTriangles[i] = _triangles[i];
        }
        _triangles = newTriangles;

        int l = _triangles.Length;
        _triangles[l - 6] = _triangleCount + 0;
        _triangles[l - 5] = _triangleCount + 1;
        _triangles[l - 4] = _triangleCount + 3;
        _triangles[l - 3] = _triangleCount + 1;
        _triangles[l - 2] = _triangleCount + 2;
        _triangles[l - 1] = _triangleCount + 3;
        _triangleCount += 4;
    }

    public void AddLeftFace()
    {
        AddFace();
        _vertices[_vertices.Length - 4] = new Vector3(-0.5f, -0.5f, 0.5f);
        _vertices[_vertices.Length - 3] = new Vector3(-0.5f, 0.5f, 0.5f);
        _vertices[_vertices.Length - 2] = new Vector3(-0.5f, 0.5f, -0.5f);
        _vertices[_vertices.Length - 1] = new Vector3(-0.5f, -0.5f, -0.5f);
    }

    public void AddRightFace()
    {
        AddFace();
        _vertices[_vertices.Length - 4] = new Vector3(0.5f, -0.5f, -0.5f);
        _vertices[_vertices.Length - 3] = new Vector3(0.5f, 0.5f, -0.5f);
        _vertices[_vertices.Length - 2] = new Vector3(0.5f, 0.5f, 0.5f);
        _vertices[_vertices.Length - 1] = new Vector3(0.5f, -0.5f, 0.5f);
    }

    public void AddBottomFace()
    {
        AddFace();
        _vertices[_vertices.Length - 4] = new Vector3(-0.5f, -0.5f, 0.5f);
        _vertices[_vertices.Length - 3] = new Vector3(-0.5f, -0.5f, -0.5f);
        _vertices[_vertices.Length - 2] = new Vector3(0.5f, -0.5f, -0.5f);
        _vertices[_vertices.Length - 1] = new Vector3(0.5f, -0.5f, 0.5f);

    }

    public void AddTopFace()
    {
        AddFace();
        _vertices[_vertices.Length - 4] = new Vector3(-0.5f, 0.5f, -0.5f);
        _vertices[_vertices.Length - 3] = new Vector3(-0.5f, 0.5f, 0.5f);
        _vertices[_vertices.Length - 2] = new Vector3(+0.5f, 0.5f, 0.5f);
        _vertices[_vertices.Length - 1] = new Vector3(0.5f, 0.5f, -0.5f);
    }

    public void AddRearFace()
    {
        AddFace();
        _vertices[_vertices.Length - 4] = new Vector3(-0.5f, -0.5f, -0.5f);
        _vertices[_vertices.Length - 3] = new Vector3(-0.5f, 0.5f, -0.5f);
        _vertices[_vertices.Length - 2] = new Vector3(0.5f, 0.5f, -0.5f);
        _vertices[_vertices.Length - 1] = new Vector3(0.5f, -0.5f, -0.5f);
    }

    public void AddFrontFace()
    {
        AddFace();
        _vertices[_vertices.Length - 4] = new Vector3(0.5f, -0.5f, 0.5f);
        _vertices[_vertices.Length - 3] = new Vector3(0.5f, 0.5f, 0.5f);
        _vertices[_vertices.Length - 2] = new Vector3(-0.5f, 0.5f, 0.5f);
        _vertices[_vertices.Length - 1] = new Vector3(-0.5f, -0.5f, 0.5f);
    }

    //0 <= index < 6
    private void DeleteFace(int index)
    {
        int[] m = new int[5]
        {
            1,2,3,4,5
        };

        int a = (int) m.GetValue(1);


        Vector3[] newVertices = new Vector3[_vertices.Length - 4];
        for(int i = 0, j = 0; i < _vertices.Length - 3; i+=4)
        {
            if(i != index)
            {
                newVertices[j + 0] = _vertices[i + 0];
                newVertices[j + 1] = _vertices[i + 1];
                newVertices[j + 2] = _vertices[i + 2];
                newVertices[j + 3] = _vertices[i + 3];
                j+=4;
            }
        }
        _vertices = newVertices;

        index = (int)(index * 1.5f); //index / 4 * 6;


        int[] newTriangles = new int[_triangles.Length - 6];
        for(int i = 0; i < newTriangles.Length; i++)
        {
            newTriangles[i] = _triangles[i];
        }
        _triangles = newTriangles;
    }

    public void DeleteLeftFace()
    {
        for (int i = 0; i < _vertices.Length - 3; i+=4)
        {
            if (_vertices[i + 0].x < 0 &&
                _vertices[i + 1].x < 0 &&
                _vertices[i + 2].x < 0 &&
                _vertices[i + 3].x < 0)
            {
                DeleteFace(i);
                break;
            }
        }
    }

    public void DeleteRightFace()
    {
        for (int i = 0; i < _vertices.Length - 3; i += 4)
        {
            if (_vertices[i + 0].x > 0 &&
                _vertices[i + 1].x > 0 &&
                _vertices[i + 2].x > 0 &&
                _vertices[i + 3].x > 0)
            {
                DeleteFace(i);
                break;
            }
        }
    }

    public void DeleteBottomFace()
    {
        for (int i = 0; i < _vertices.Length - 3; i += 4)
        {
            if (_vertices[i + 0].y < 0 &&
                _vertices[i + 1].y < 0 &&
                _vertices[i + 2].y < 0 &&
                _vertices[i + 3].y < 0)
            {
                DeleteFace(i);
                break;
            }
        }
    }

    public void DeleteTopFace()
    {
        for (int i = 0; i < _vertices.Length - 3; i += 4)
        {
            if (_vertices[i + 0].y > 0 &&
                _vertices[i + 1].y > 0 &&
                _vertices[i + 2].y > 0 &&
                _vertices[i + 3].y > 0)
            {
                DeleteFace(i);
                break;
            }
        }
    }

    public void DeleteRearFace()
    {
        for (int i = 0; i < _vertices.Length - 3; i += 4)
        {
            if (_vertices[i + 0].z < 0 &&
                _vertices[i + 1].z < 0 &&
                _vertices[i + 2].z < 0 &&
                _vertices[i + 3].z < 0)
            {
                DeleteFace(i);
                break;
            }
        }
    }

    public void DeleteFrontFace()
    {
        for (int i = 0; i < _vertices.Length - 3; i += 4)
        {
            if (_vertices[i + 0].z > 0 &&
                _vertices[i + 1].z > 0 &&
                _vertices[i + 2].z > 0 &&
                _vertices[i + 3].z > 0)
            {
                DeleteFace(i);
                break;
            }
        }
    }

    public void UpdateMesh()
    {

        _mesh.Clear();

        if (_vertices.Length == 0 || _triangles.Length == 0) return;

        _mesh.vertices = _vertices;
        _mesh.triangles = _triangles;

        _mesh.Optimize();
        _mesh.RecalculateNormals();
    }
}
