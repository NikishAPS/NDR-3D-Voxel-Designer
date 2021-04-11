using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//using System;
//using System.Globalization;


public class ImporterOBJ : MonoBehaviour
{
    public Material material;
    //public string path;
    public string[] value;

    private string importedName = "imp";

    public List<Vector3> vertices;
    public List<Vector3> normals;
    public List<int> triangles;
    //public List<int> face;
    //public List<int> triangles;

    public MessageSystem messageSystem;



    private void Start()
    {
        //ReadFile(path);
    }

    public void ReadFile(string path)
    {
        if (File.Exists(path))
        {
            value = File.ReadAllLines(path);
        }
        else
        {
            messageSystem.AddMessage("The file does not exist");
            return;
        }

        for (int i = 0; i < value.Length; i++)
        {
            if(SequenceChar(value[i], "v "))
            {
                vertices.Add(ReadVectorFromLine(value[i]));
                continue;
            }

            if (SequenceChar(value[i], "vn "))
            {
                normals.Add(ReadVectorFromLine(value[i]));
                continue;
            }

            if(SequenceChar(value[i], "f "))
            {
                ReadFace(value[i]);
                continue;
            }
        }

        Create();
        //Clear();
    }

    public Vector3[] _vertices;
    public Vector3[] _normals;
    public int[] _triangles;

    private void Create()
    {
        GameObject importedGO = new GameObject();
        importedGO.transform.name = importedName;
        importedGO.transform.position = new Vector3(5, 5, 5);

        Mesh mesh = new Mesh();

        importedGO.AddComponent<MeshFilter>().mesh = mesh;
        importedGO.AddComponent<MeshRenderer>().material = material;

        //вершины
        _vertices = new Vector3[vertices.Count];
        for (int i = 0; i < _vertices.Length; i++)
        {
            _vertices[i] = vertices[i];
        }

        _normals = new Vector3[normals.Count];
        for(int i = 0; i < _normals.Length;  i++)
        {
            _normals[i] = normals[i];
        }

        //треугольники
        /*_triangles = new int[(int)(face.Count * 0.5f)];
         * 
         *
        for (int i = 0, j = 0; i < _triangles.Length - 5; i += 6, j += 12)
        {
            _triangles[i + 0] = face[j + 0] - 1;
            _triangles[i + 1] = face[j + 3] - 1;
            _triangles[i + 2] = face[j + 6] - 1;
            _triangles[i + 3] = face[j + 0] - 1;
            _triangles[i + 4] = face[j + 6] - 1;
            _triangles[i + 5] = face[j + 9] - 1;
        }

        /*
        _triangles = new int[]
        {
            0, 1, 2,
            0, 2, 3,

            4, 7, 6,
            4, 6, 5
        };
        */

        _triangles = new int[triangles.Count];
        for (int i = 0; i < _triangles.Length; i++)
        {
            _triangles[i] = triangles[i];
        }


    
        mesh.Clear();

        //mesh.SetVertices(vertices);
        //mesh.SetNormals(normals);
        //mesh.SetTriangles(triangles, 0);
        mesh.vertices = _vertices;
        mesh.triangles = _triangles;

       // mesh.RecalculateNormals();
       // mesh.RecalculateTangents();
       // mesh.RecalculateBounds();

        mesh.Optimize();
        mesh.RecalculateNormals();
    }

    //чтение вектора
    private Vector3 ReadVectorFromLine(string line)
    {
        int i = 0;

        float ReadFloatFromLine()
        {
            string stringValue = "";
            float value = 0;


            for (; i < line.Length; i++)
            {
                char ch = line[i];

                if (ch >= '0' && ch <= '9' || ch == '-')
                {
                    stringValue += ch.ToString();
                    if(i == line.Length - 1)
                    {
                    }
                }
                else
                {
                    if(ch == '.')
                    {
                        stringValue += ',';
                        continue;
                    }
                }

                if (ch == ' ' || i == line.Length - 1)
                {
                    if (stringValue != "")
                    {
                        //value = float.Parse(stringValue, CultureInfo.InvariantCulture);
                        value = float.Parse(stringValue);

                        return value;
                    }
                }
            }




            return value;
        }

        return new Vector3(ReadFloatFromLine(), ReadFloatFromLine(), ReadFloatFromLine());
    }

    //чтение граней
    private void ReadFace(string line)
    {
        List<int> _face = new List<int>();
        //string curTrianglesIndexes = "";


        string value = "";
        for (int i = 0; i < line.Length; i++)
        {
            if(line[i] >= '0' && line[i] <= '9')
            {
                value += line[i].ToString();

                if (i == line.Length - 1)
                {
                    _face.Add(int.Parse(value));
                    value = "";
                }
                else if(line[i + 1] == '/' || line[i + 1] == ' ')
                {
                    _face.Add(int.Parse(value));
                    value = "";
                }
            }
            else
            {
                if(line[i] == '/' && line[i + 1] == '/')
                {
                    _face.Add(0);
                    value = "";
                }
                else
                {
                    if(line[i] == ' ')
                    {
                        value = "";
                    }
                }
            }
        }

        for(int i = 0; i < _face.Count / 3; i+= 3)
        {
            break;
            triangles.Add(_face[i] - 1);
        }

        int k = (int)(_face.Count / 3);

        for (int i = 0, of = 0; i < _face.Count - 5 - of; i += 6 + of)
        {
            of = 0;
            triangles.Add(_face[i + 0] - 1);

            //если грань имеет две вершины
            if (k > 1)
            {
                of++;
                triangles.Add(_face[i + 3] - 1);
            }

            //если грань имеет трех вершины
            if (k > 2)
            {
                of++;
                triangles.Add(_face[i + 6] - 1);
            }
                
            
            //если грань имеет более трех вершин
            if (k > 3)
            {
                of++;
                //of = 3;
                triangles.Add(_face[i + 0] - 1);
                triangles.Add(_face[i + 6] - 1);
                triangles.Add(_face[i + 9] - 1);
            }
        }
    }

    //нахождение заданной последовательности символов
    private bool SequenceChar(string strMain, string strSeq)
    {
        string value = "";
        for(int i = 0; i < strSeq.Length;  i++)
        {
            value += strMain[i];
        }

        return (value == strSeq);
    }

    //нахождение заданной строки
    private bool FindString(string strMian, string strReq)
    {
        int i = 0;

        foreach(char ch in strMian)
        {
            if (ch == strReq[i])
            {
                i++;
            }
            else
            {
                i = 0;
            }

            if(i ==  strReq.Length)
            {
                return true;
            }
        }

        return false;
    }

    private void Clear()
    {
        vertices.Clear();
        normals.Clear();
        triangles.Clear();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F) && false)
        {
            Clear();
            //face.Clear();

            GameObject g = GameObject.Find("imp");
            if (g) Destroy(g);
            //triangles.Clear();
            Start();
            
        }
    }
}
