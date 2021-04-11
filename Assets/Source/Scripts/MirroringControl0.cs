using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGUI;

namespace Voxelator
{
    

    public class MirroringControl0 : MonoBehaviour
    {
        [System.Serializable]
        public class ReflectedVoxel
        {
            public bool mirX, mirY, mirZ;
            public VoxelMeshGenerator oppositeVoxel;
            public VoxelMeshGenerator keyVoxel;

            public void Update(Vector3 grid)
            {
                Vector3 pos = keyVoxel.transform.position;
                if(mirX) pos.x = grid.x - keyVoxel.transform.position.x - 1;
                if(mirY) pos.y = grid.y - keyVoxel.transform.position.y;
                if(mirZ) pos.z = grid.z - keyVoxel.transform.position.z - 1;

                oppositeVoxel.transform.position = pos;
            }

            public void UpdateVertices(GameObject voxel)
            {
                if (keyVoxel.gameObject != voxel) return;

                int mulX = mirX ? -1 : 1;
                int mulY = mirY ? -1 : 1;
                int mulZ = mirZ ? -1 : 1;

                for (int i = 0; i < keyVoxel.points.Length; i++)
                {
                    for (int j = 0; j < oppositeVoxel.points.Length; j++)
                    {
                        if (keyVoxel.points[i].x == oppositeVoxel.points[j].x * mulX &&
                            keyVoxel.points[i].y == oppositeVoxel.points[j].y * mulY &&
                            keyVoxel.points[i].z == oppositeVoxel.points[j].z * mulZ)
                        {
                            oppositeVoxel.offset[j] = new Vector3(keyVoxel.offset[i].x * mulX, keyVoxel.offset[i].y * mulY, keyVoxel.offset[i].z * mulZ);
                            break;
                        }
                    }
                }

                oppositeVoxel.UpdateMesh();

            }

            public void UpdateVertices()
            {
                int mulX = mirX ? -1 : 1;
                int mulY = mirY ? -1 : 1;
                int mulZ = mirZ ? -1 : 1;

                for (int i = 0; i < keyVoxel.points.Length; i++)
                {
                    for (int j = 0; j < oppositeVoxel.points.Length; j++)
                    {
                        if (keyVoxel.points[i].x == oppositeVoxel.points[j].x * mulX &&
                            keyVoxel.points[i].y == oppositeVoxel.points[j].y * mulY &&
                            keyVoxel.points[i].z == oppositeVoxel.points[j].z * mulZ)
                        {
                            oppositeVoxel.offset[j] = new Vector3(keyVoxel.offset[i].x * mulX, keyVoxel.offset[i].y * mulY, keyVoxel.offset[i].z * mulZ);
                            break;
                        }
                    }
                }

                oppositeVoxel.UpdateMesh();

            }

            public void SetMir(bool x, bool y, bool z)
            {
                mirX = x; mirY = y; mirZ = z;
            }


            public ReflectedVoxel()
            {
                this.oppositeVoxel = null;
                this.keyVoxel = null;
                this.mirX = false;
                this.mirY = false;
                this.mirZ = false;
            }

            public ReflectedVoxel(VoxelMeshGenerator voxelMeshGenerator, bool mirX, bool mirY, bool mirZ)
            {
                this.oppositeVoxel = voxelMeshGenerator;
                this.mirX = voxelMeshGenerator;
                this.mirY = voxelMeshGenerator;
                this.mirZ = voxelMeshGenerator;
            }
        }

        public List<ReflectedVoxel> reflectedVoxels = new List<ReflectedVoxel>();

        private bool updateVerticesNext;

        public MyFlag myFlagX, myFlagY, myFlagZ;
        public bool flagX, flagY, flagZ;

        private VoxelsControl voxelsControl;
        private CreateInstanceVoxel createInstanceVoxel;

        private GridControl grid;

        private int lastCountVoxels;



        void Start()
        {
            voxelsControl = GetComponent<VoxelsControl>();
            createInstanceVoxel = GameObject.Find("CreateInstanceVoxel").GetComponent<CreateInstanceVoxel>();

            grid = GameObject.Find("Grid").GetComponent<GridControl>();
        }


        private void AddReflection(int newCount)
        {
            if (flagX)
            {
                int length = reflectedVoxels.Count;

                //Смешанное (по нескольким осям) отражение
                for (int i = 0; i < length; i++)
                {
                    break;
                    Vector3 pos = reflectedVoxels[i].oppositeVoxel.transform.position;
                    pos.x = grid.GetSize().x - reflectedVoxels[i].oppositeVoxel.transform.position.x - 1;


                    reflectedVoxels.Add(new ReflectedVoxel());
                    int index = reflectedVoxels.Count - 1;

                    reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(reflectedVoxels[i].oppositeVoxel, pos);
                    reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                    reflectedVoxels[index].keyVoxel = reflectedVoxels[i].keyVoxel;

                    reflectedVoxels[index].SetMir(true, reflectedVoxels[i].mirY, reflectedVoxels[i].mirZ);

                }



                length = voxelsControl.voxels.Count;

                //Обычное отражение
                for (int i = length - newCount; i < length; i++)
                {
                    Vector3 pos = voxelsControl.voxels[i].transform.position;
                    pos.x = grid.GetSize().x - voxelsControl.voxels[i].transform.position.x - 1;


                    reflectedVoxels.Add(new ReflectedVoxel());
                    int index = reflectedVoxels.Count - 1;

                    reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(voxelsControl.voxels[i], pos);
                    reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                    reflectedVoxels[index].keyVoxel = voxelsControl.voxels[i];

                    reflectedVoxels[index].SetMir(true, false, false);
                }
            }

            if (flagZ)
            {
                int length = reflectedVoxels.Count;


                length = voxelsControl.voxels.Count;

                //Обычное отражение
                for (int i = length - newCount; i < length; i++)
                {
                    Vector3 pos = voxelsControl.voxels[i].transform.position;
                    pos.z = grid.GetSize().z - voxelsControl.voxels[i].transform.position.z - 1;


                    reflectedVoxels.Add(new ReflectedVoxel());
                    int index = reflectedVoxels.Count - 1;

                    reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(voxelsControl.voxels[i], pos);
                    reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                    reflectedVoxels[index].keyVoxel = voxelsControl.voxels[i];

                    reflectedVoxels[index].SetMir(false, false, true);
                }
            }

        }

        private void RemoveReflection(int newCount)
        {
            for (int i = 0; i < reflectedVoxels.Count; i++)
            {
                if(reflectedVoxels[i].keyVoxel == null)
                {
                    Destroy(reflectedVoxels[i].oppositeVoxel.gameObject);
                    reflectedVoxels.Remove(reflectedVoxels[i]);
                    i--;
                }
            }
        }

        void Update()
        {
            if (updateVerticesNext)
            {
                for (int i = 0; i < reflectedVoxels.Count; i++)
                {
                    reflectedVoxels[i].UpdateVertices();
                }

                updateVerticesNext = false;
            }


            if (myFlagX.active != flagX)
            {
                if (myFlagX.active)
                {
                    int length = reflectedVoxels.Count;

                    //Смешанное (по нескольким осям) отражение
                    for(int i = 0; i < length; i++)
                    {
                        Vector3 pos = reflectedVoxels[i].oppositeVoxel.transform.position;
                        pos.x = grid.GetSize().x - reflectedVoxels[i].oppositeVoxel.transform.position.x - 1;


                        reflectedVoxels.Add(new ReflectedVoxel());
                        int index = reflectedVoxels.Count - 1;

                        reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(reflectedVoxels[i].oppositeVoxel, pos);
                        reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                        reflectedVoxels[index].keyVoxel = reflectedVoxels[i].keyVoxel;

                        reflectedVoxels[index].SetMir(true, reflectedVoxels[i].mirY, reflectedVoxels[i].mirZ);

                        updateVerticesNext = true;
                    }



                    length = voxelsControl.voxels.Count;

                    //Обычное отражение
                    for (int i = 0; i < length; i ++)
                    {
                        Vector3 pos = voxelsControl.voxels[i].transform.position;
                        pos.x = grid.GetSize().x - voxelsControl.voxels[i].transform.position.x - 1;


                        reflectedVoxels.Add(new ReflectedVoxel());
                        int index = reflectedVoxels.Count - 1;

                        reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(voxelsControl.voxels[i], pos);
                        reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                        reflectedVoxels[index].keyVoxel = voxelsControl.voxels[i];

                        reflectedVoxels[index].SetMir(true, false, false);

                        updateVerticesNext = true;


                    }
                }
                else
                {
                    for (int i = 0; i < reflectedVoxels.Count; i++)
                    {
                        if (reflectedVoxels[i].mirX)
                        {
                            Destroy(reflectedVoxels[i].oppositeVoxel.gameObject);
                            reflectedVoxels.Remove(reflectedVoxels[i]);
                            i--;
                        }
                    }
                }
                flagX = myFlagX.active;
            }


            if (myFlagY.active != flagY)
            {
                if (myFlagY.active)
                {
                    int length = reflectedVoxels.Count;


                    //Двойное отражение
                    for (int i = 0; i < length; i++)
                    {
                        Vector3 pos = reflectedVoxels[i].oppositeVoxel.transform.position;
                        pos.y = grid.GetSize().y - reflectedVoxels[i].oppositeVoxel.transform.position.y;

                        reflectedVoxels.Add(new ReflectedVoxel());
                        int index = reflectedVoxels.Count - 1;

                        reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(reflectedVoxels[i].oppositeVoxel, pos);
                        reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                        reflectedVoxels[index].keyVoxel = reflectedVoxels[i].keyVoxel;

                        reflectedVoxels[index].SetMir(reflectedVoxels[i].mirX, true, reflectedVoxels[i].mirZ);

                        updateVerticesNext = true;


                    }



                    length = voxelsControl.voxels.Count;

                    for (int i = 0; i < length; i++)
                    {
                        Vector3 pos = voxelsControl.voxels[i].transform.position;
                        pos.y = grid.GetSize().y - voxelsControl.voxels[i].transform.position.y;


                        reflectedVoxels.Add(new ReflectedVoxel());
                        int index = reflectedVoxels.Count - 1;

                        reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(voxelsControl.voxels[i], pos);
                        reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                        reflectedVoxels[index].keyVoxel = voxelsControl.voxels[i];

                        reflectedVoxels[index].mirY = true;

                        updateVerticesNext = true;


                    }

                }
                else
                {
                    for (int i = 0; i < reflectedVoxels.Count; i++)
                    {
                        if (reflectedVoxels[i].mirY)
                        {
                            Destroy(reflectedVoxels[i].oppositeVoxel.gameObject);
                            reflectedVoxels.Remove(reflectedVoxels[i]);
                            i--;
                        }
                    }
                }
                flagY = myFlagY.active;
            }


            if (myFlagZ.active != flagZ)
            {
                if (myFlagZ.active)
                {
                    int length = reflectedVoxels.Count;


                    //Двойное отражение
                    for (int i = 0; i < length; i++)
                    {
                        Vector3 pos = reflectedVoxels[i].oppositeVoxel.transform.position;
                        pos.z = grid.GetSize().z - reflectedVoxels[i].oppositeVoxel.transform.position.z - 1;


                        reflectedVoxels.Add(new ReflectedVoxel());
                        int index = reflectedVoxels.Count - 1;

                        reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(reflectedVoxels[i].oppositeVoxel, pos);
                        reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                        reflectedVoxels[index].keyVoxel = reflectedVoxels[i].keyVoxel;

                        reflectedVoxels[index].SetMir(reflectedVoxels[i].mirX, reflectedVoxels[i].mirY, true);

                        updateVerticesNext = true;


                    }

                    length = voxelsControl.voxels.Count;

                    for (int i = 0; i < length; i++)
                    {
                        Vector3 pos = voxelsControl.voxels[i].transform.position;
                        pos.z = grid.GetSize().z - voxelsControl.voxels[i].transform.position.z - 1;


                        reflectedVoxels.Add(new ReflectedVoxel());
                        int index = reflectedVoxels.Count - 1;

                        reflectedVoxels[index].oppositeVoxel = createInstanceVoxel.CopyOnly(voxelsControl.voxels[i], pos);
                        reflectedVoxels[index].oppositeVoxel.GetComponent<VoxelMaterials>().Standard();
                        reflectedVoxels[index].keyVoxel = voxelsControl.voxels[i];

                        reflectedVoxels[index].mirZ = true;

                        updateVerticesNext = true;


                    }

                }
                else
                {
                    for (int i = 0; i < reflectedVoxels.Count; i++)
                    {
                        if (reflectedVoxels[i].mirZ)
                        {
                            Destroy(reflectedVoxels[i].oppositeVoxel.gameObject);
                            reflectedVoxels.Remove(reflectedVoxels[i]);
                            i--;
                        }
                    }
                }
                flagZ = myFlagZ.active;
            }


            
        }

        private void LateUpdate()
        {
            if (voxelsControl.voxels.Count == 0)
            {
                for (int i = 0; i < reflectedVoxels.Count; i++)
                {
                    Destroy(reflectedVoxels[i].oppositeVoxel.gameObject);
                }
                reflectedVoxels.Clear();
                return;
            }
            else
            {
                if (!flagX && !flagY && !flagZ) return;

                int newCount = voxelsControl.voxels.Count - lastCountVoxels; 

                if (newCount > 0)
                {
                    AddReflection(newCount);
                    lastCountVoxels = voxelsControl.voxels.Count;
                }
                else if(newCount < 0)
                {
                    RemoveReflection(newCount);
                }
            }

           
            

            for (int i = 0; i < reflectedVoxels.Count; i++)
            {
                reflectedVoxels[i].Update(grid.GetSize());
            }

            if(voxelsControl.mode == 3 && voxelsControl.selectedVoxels.Count > 0)
            for(int i =0; i < reflectedVoxels.Count; i++)
            {
                reflectedVoxels[i].UpdateVertices(voxelsControl.selectedVoxels[voxelsControl.selectedVoxels.Count - 1]);
            }
        }
    }
}
