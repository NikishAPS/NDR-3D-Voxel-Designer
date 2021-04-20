using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxelator
{
    public class VoxelMeshGenerator : MonoBehaviour
    {
        //public Vector3Int size = new Vector3Int(1, 1, 1);
        public Vector3[] offset;

        public Vector3[] points;

        //[HideInInspector]
        public Vector3[] vertices;
        //[HideInInspector]
        public int[] triangles;
        [HideInInspector]
        public Mesh mesh;
        [HideInInspector]
        //public MeshCollider meshCollider;
        private BoxCollider boxCollider;

        private VoxelMeshGenerator voxelMeshGenerator;



        public void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            //meshCollider = GetComponent<MeshCollider>();
            boxCollider = GetComponent<BoxCollider>();
            voxelMeshGenerator = GetComponent<VoxelMeshGenerator>();


            //if (createShape)
            //CreateShape();

            UpdateMesh();
        }

        void Update()
        {
            //CreateShape();
            //UpdateMesh();
        }

        /*public int verticesCount;
        public int pointsCount;*/

        public void CreateShape()
        {
            print("Shape");
            //int length = (int)(Mathf.Pow(size.x + 1, 2) * 2 + (size.x) * 4 * (size.x - 1));
            //int lengthPoints = (int)(Mathf.Pow(size + 1, 2) * 2 + (size) * 4 * (size - 1));
            //int lengthPoints = size * 4 * 2 + (size - 1) * 4;
            //int lengthVertices = (size + 1) * (size + 1) * 6;

            int size = 1;
            int verticesCount = (size + 1) * (size + 1) * 6;
            int pointsCount = size * 4 * 2 + (size - 1) * 4;
            pointsCount = (int)(Mathf.Pow(size + 1, 2) * 2 + (size) * 4 * (size - 1));
            int trianglesCount = size * size * 6 * 6;


            vertices = new Vector3[]
            {
                //Rear
                new Vector3 (-0.5f, -0.5f, -0.5f),
                new Vector3 (-0.5f, 0.5f, -0.5f),
                new Vector3 (0.5f, 0.5f, -0.5f),
                new Vector3 (0.5f, -0.5f, -0.5f),

                //Front
                new Vector3 (0.5f, -0.5f, 0.5f),
                new Vector3 (0.5f, 0.5f, 0.5f),
                new Vector3 (-0.5f, 0.5f, 0.5f),
                new Vector3 (-0.5f, -0.5f, 0.5f),

                //Left
                new Vector3 (-0.5f, -0.5f, 0.5f),
                new Vector3 (-0.5f, 0.5f, 0.5f),
                new Vector3 (-0.5f, 0.5f, -0.5f),
                new Vector3 (-0.5f, -0.5f, -0.5f),

                //Right
                new Vector3 (0.5f, -0.5f, -0.5f),
                new Vector3 (0.5f, 0.5f, -0.5f),
                new Vector3 (0.5f, 0.5f, 0.5f),
                new Vector3 (0.5f, -0.5f, 0.5f),

                //Bottom
                new Vector3 (-0.5f, -0.5f, 0.5f),
                new Vector3 (-0.5f, -0.5f, -0.5f),
                new Vector3 (0.5f, -0.5f, -0.5f),
                new Vector3 (0.5f, -0.5f, 0.5f),

                //Top
                new Vector3 (-0.5f, 0.5f, -0.5f),
                new Vector3 (-0.5f, 0.5f, 0.5f),
                new Vector3 (+0.5f, 0.5f, 0.5f),
                new Vector3 (0.5f, 0.5f, -0.5f), 
            };



            triangles = new int[]
            {
                0, 1, 3, //face front
	            1, 2, 3,

                4, 5, 7,
                5, 6, 7,

                8, 9, 11,
                9, 10, 11,

                12, 13, 15,
                13, 14, 15,

                16, 17, 19,
                17, 18, 19,

                20, 21, 23,
                21, 22, 23
            };


            offset = new Vector3[pointsCount];

            points = new Vector3[pointsCount];

            
            for (int i = 0, c = 0; i < vertices.Length; i++)
            {
                if (VertexOfEdge(vertices[i]) || true)
                {
                    for (int j = 0; j < vertices.Length; j++)
                    {
                        if(vertices[i] == vertices[j])
                        {
                            bool b = true;
                            for(int k = 0; k < pointsCount; k++)
                            {
                                if(points[k] == vertices[i])
                                {
                                    b = false;
                                    break;
                                }
                            }
                            if(b)
                            {
                                points[c] = vertices[i];
                                c++;
                            }
                        }
                    }
                }
            }

            
            
        }

        private bool VertexOfEdge(Vector3 ver)
        {
            ver.x = Mathf.Abs(ver.x);
            ver.y = Mathf.Abs(ver.y);
            ver.z = Mathf.Abs(ver.z);

            return ver.x == 0.5f && ver.y == 0.5f || ver.x == 0.5f && ver.z == 0.5f || ver.y == 0.5f && ver.z == 0.5f;
        }

        public void UpdateMesh()
        {
            print("UpdateMesh");

            Vector3[] ver = new Vector3[vertices.Length];
            for(int i = 0;  i < ver.Length; i++)
            {
                Vector3 v = vertices[i];

                for(int j = 0; j < points.Length; j++)
                {
                    if(vertices[i] == points[j])
                    {
                        v += offset[j];
                        break;
                    }
                }


                ver[i] = v;
            }
            
            mesh.Clear();

            mesh.vertices = ver;
            mesh.triangles = triangles;

            mesh.Optimize();
            mesh.RecalculateNormals();

            //meshCollider.sharedMesh.Clear();
            //meshCollider.sharedMesh = mesh;
        }

        //расчет вершин после поворота
        public void CalculateRot()
        {
            Vector3 v = Vector3.zero;

            if (transform.eulerAngles.x != 0)
            {
                if (transform.eulerAngles.x < 180)
                {
                    v = offset[0];

                    offset[0] = new Vector3(offset[1].x, -offset[1].z, offset[1].y);
                    offset[1] = new Vector3(offset[6].x, -offset[6].z, offset[6].y);
                    offset[6] = new Vector3(offset[7].x, -offset[7].z, offset[7].y);
                    offset[7] = new Vector3(v.x, -v.z, v.y);

                    v = offset[5];

                    offset[5] = new Vector3(offset[4].x, -offset[4].z, offset[4].y);
                    offset[4] = new Vector3(offset[3].x, -offset[3].z, offset[3].y);
                    offset[3] = new Vector3(offset[2].x, -offset[2].z, offset[2].y);
                    offset[2] = new Vector3(v.x, -v.z, v.y);
                }
                else
                {
                    v = offset[2];

                    offset[2] = new Vector3(offset[3].x, offset[3].z, -offset[3].y);
                    offset[3] = new Vector3(offset[4].x, offset[4].z, -offset[4].y);
                    offset[4] = new Vector3(offset[5].x, offset[5].z, -offset[5].y);
                    offset[5] = new Vector3(v.x, v.z, -v.y);

                    v = offset[7];

                    offset[7] = new Vector3(offset[6].x, offset[6].z, -offset[6].y);
                    offset[6] = new Vector3(offset[1].x, offset[1].z, -offset[1].y);
                    offset[1] = new Vector3(offset[0].x, offset[0].z, -offset[0].y);
                    offset[0] = new Vector3(v.x, v.z, -v.y);
                }
            }

            if (transform.eulerAngles.y != 0)
            {
                if(transform.eulerAngles.y < 180)
                {
                    v = offset[0];

                    offset[0] = new Vector3(offset[3].z, offset[3].y, -offset[3].x);
                    offset[3] = new Vector3(offset[4].z, offset[4].y, -offset[4].x);
                    offset[4] = new Vector3(offset[7].z, offset[7].y, -offset[7].x);
                    offset[7] = new Vector3(v.z, v.y, -v.x);

                    v = offset[1];

                    offset[1] = new Vector3(offset[2].z, offset[2].y, -offset[2].x);
                    offset[2] = new Vector3(offset[5].z, offset[5].y, -offset[5].x);
                    offset[5] = new Vector3(offset[6].z, offset[6].y, -offset[6].x);
                    offset[6] = new Vector3(v.z, v.y, -v.x);
                }
                else
                {
                    v = offset[7];

                    offset[7] = new Vector3(-offset[4].z, offset[4].y, offset[4].x);
                    offset[4] = new Vector3(-offset[3].z, offset[3].y, offset[3].x);
                    offset[3] = new Vector3(-offset[0].z, offset[0].y, offset[0].x);
                    offset[0] = new Vector3(-v.z, v.y, v.x);

                    v = offset[6];

                    offset[6] = new Vector3(-offset[5].z, offset[5].y, offset[5].x);
                    offset[5] = new Vector3(-offset[2].z, offset[2].y, offset[2].x);
                    offset[2] = new Vector3(-offset[1].z, offset[1].y, offset[1].x);
                    offset[1] = new Vector3(-v.z, v.y, v.x);
                }
            }

            if (transform.eulerAngles.z != 0)
            {
                if (transform.eulerAngles.z < 180)
                {
                    v = offset[3];

                    offset[3] = new Vector3(-offset[2].y, offset[2].x, offset[2].z);
                    offset[2] = new Vector3(-offset[1].y, offset[1].x, offset[1].z);
                    offset[1] = new Vector3(-offset[0].y, offset[0].x, offset[0].z);
                    offset[0] = new Vector3(-v.y, v.x, v.z);

                    v = offset[4];

                    offset[4] = new Vector3(-offset[5].y, offset[5].x, offset[5].z);
                    offset[5] = new Vector3(-offset[6].y, offset[6].x, offset[6].z);
                    offset[6] = new Vector3(-offset[7].y, offset[7].x, offset[7].z);
                    offset[7] = new Vector3(-v.y, v.x, v.z);
                }
                else
                {
                    v = offset[0];

                    offset[0] = new Vector3(offset[1].y, -offset[1].x, offset[1].z);
                    offset[1] = new Vector3(offset[2].y, -offset[2].x, offset[2].z);
                    offset[2] = new Vector3(offset[3].y, -offset[3].x, offset[3].z);
                    offset[3] = new Vector3(v.y, -v.x, v.z);

                   

                    v = offset[7];

                    offset[7] = new Vector3(offset[6].y, -offset[6].x, offset[6].z);
                    offset[6] = new Vector3(offset[5].y, -offset[5].x, offset[5].z);
                    offset[5] = new Vector3(offset[4].y, -offset[4].x, offset[4].z);
                    offset[4] = new Vector3(v.y, -v.x, v.z);
                }
            }


            transform.rotation = Quaternion.identity;
            UpdateMesh();
        }
    }
}