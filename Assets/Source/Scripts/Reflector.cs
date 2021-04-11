using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxelator
{
    public class Reflector : MonoBehaviour
    {
        public MyGUI.MyFlag myFlag;
        public MyGUI.MyColor myColor;
        public bool flag;

        public List<VoxelMeshGenerator> reflectedVoxels;

        private VoxelsControl voxelsControl;
        private GridControl gridControl;
        private CreateInstanceVoxel createInstanceVoxel;

        private int colorIndex;

        private bool rot;
        private int rotID;

        public GameObject gridMiddle;

        void Start()
        {
            voxelsControl = GetComponent<VoxelsControl>();

            gridControl = GameObject.Find("Grid").GetComponent<GridControl>();

            createInstanceVoxel = GameObject.Find("CreateInstanceVoxel").GetComponent<CreateInstanceVoxel>();
        }

        void Update()
        {
            gridMiddle.SetActive(flag);
            if (myFlag.active != flag)
            {
                if (myFlag.active)
                {
                    for (int i = 0; i < voxelsControl.voxels.Count; i++)
                    {
                        AddVoxel(i);
                    }
                }
                else
                {
                    for (int i = 0; i < reflectedVoxels.Count; i++)
                    {
                        Destroy(reflectedVoxels[i].gameObject);
                    }
                    reflectedVoxels.Clear();
                }

                flag = myFlag.active;
            }


            if (!flag) return;



            if (voxelsControl.voxels.Count == 0)
            {
                for (int i = 0; i < reflectedVoxels.Count; i++)
                {
                    Destroy(reflectedVoxels[i].gameObject);
                }
                reflectedVoxels.Clear();
                return;
            }

            if (reflectedVoxels.Count < voxelsControl.voxels.Count)
            {
                for (int i = reflectedVoxels.Count; i < voxelsControl.voxels.Count; i++)
                {
                    AddVoxel(i);
                }
                return;
            }

            if (reflectedVoxels.Count > voxelsControl.voxels.Count)
            {
                for (int i = reflectedVoxels.Count - 1; i >= voxelsControl.voxels.Count; i--)
                {
                    break;
                    Destroy(reflectedVoxels[i].gameObject);
                    reflectedVoxels.Remove(reflectedVoxels[i]);
                }


                for (int i = 0; i < voxelsControl.voxels.Count; i++)
                {
                    if (reflectedVoxels[i].transform.position != GetPos(i))
                    {
                        Destroy(reflectedVoxels[i].gameObject);
                        reflectedVoxels.Remove(reflectedVoxels[i]);
                        //i--;

                        if (reflectedVoxels.Count == voxelsControl.voxels.Count)
                            break;
                    }
                }

                if(reflectedVoxels.Count != voxelsControl.voxels.Count)
                {
                    Destroy(reflectedVoxels[reflectedVoxels.Count - 1].gameObject);
                    reflectedVoxels.Remove(reflectedVoxels[reflectedVoxels.Count - 1]);
                }
                return;
            }


            //Update position
            if (voxelsControl.mode == 2)
            {
                for (int i = 0; i < reflectedVoxels.Count; i++)
                {
                    reflectedVoxels[i].transform.position = GetPos(i);
                }
            }

            //Update rotation
            if (voxelsControl.GetSelectedVoxel())
            {
                if (voxelsControl.GetSelectedVoxel().transform.rotation != Quaternion.identity)
                {
                    {
                        if (!rot)
                        {
                            rot = true;
                            rotID = voxelsControl.GetSelectedVoxelID();
                        }
                    }
                }
            }
            if (rot)
            {
                Vector3 rotV = voxelsControl.voxels[rotID].transform.eulerAngles;

                reflectedVoxels[rotID].transform.eulerAngles = new Vector3(-rotV.x, -rotV.y, rotV.z);

                if (reflectedVoxels[rotID].transform.rotation == Quaternion.identity)
                {
                    for (int i = 0; i < reflectedVoxels[rotID].offset.Length; i++)
                    {
                        Vector3 newPos = voxelsControl.voxels[rotID].offset[i];
                        reflectedVoxels[rotID].offset[i] = new Vector3(newPos.x, newPos.y, -newPos.z);
                    }

                    reflectedVoxels[rotID].UpdateMesh();

                    rot = false;
                }
            }

            //Update Vertices
            if (voxelsControl.mode == 3 && voxelsControl.selectedVoxels.Count > 0)
            {
                for (int index = 0; index < reflectedVoxels.Count; index++)
                {
                    if (Vector3.Distance(reflectedVoxels[index].transform.position, voxelsControl.GetSelectedVoxel().transform.position) < 2f || true)
                    {
                        for (int i = 0; i < reflectedVoxels[index].offset.Length; i++)
                        {
                            if (reflectedVoxels[index].offset != voxelsControl.voxels[index].offset)
                            {
                                Vector3 newPos = voxelsControl.voxels[index].offset[i];
                                reflectedVoxels[index].offset[i] = new Vector3(newPos.x, newPos.y, -newPos.z);
                            }
                            else
                            {
                                break;
                            }
                        }
                        reflectedVoxels[index].UpdateMesh();
                    }
                }
            }


            //UpdateColor
            if (colorIndex != myColor.index)
            {
                for (int i = 0; i < voxelsControl.voxels.Count; i++)
                {
                    reflectedVoxels[i].GetComponent<VoxelMaterials>().SetStandard(voxelsControl.voxels[i].GetComponent<VoxelMaterials>().standard);
                }
                colorIndex = myColor.index;
            }

        }

        Vector3 GetPos(int i)
        {
            Vector3 pos = voxelsControl.voxels[i].transform.position;
            pos.z = gridControl.GetSize().z - voxelsControl.voxels[i].transform.position.z - 1;
            return pos;
        }

        void AddVoxel(int index)
        {
            reflectedVoxels.Add(createInstanceVoxel.CopyOnly(voxelsControl.voxels[index], GetPos(index)));
            reflectedVoxels[index].GetComponent<VoxelMaterials>().Standard();
            reflectedVoxels[index].tag = "Untagged";

            //reflectedVoxels[index].transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            for (int i = 0; i < reflectedVoxels[index].points.Length; i++)
            {
                reflectedVoxels[index].points[i].z = -reflectedVoxels[index].points[i].z;
                reflectedVoxels[index].offset[i].z = -reflectedVoxels[index].offset[i].z;
            }

            int mul = reflectedVoxels[index].triangles[reflectedVoxels[index].triangles.Length - 1] == 22 ? -1 : 1;

            for (int j = 0; j < reflectedVoxels[index].triangles.Length; j += 6)
            {
                reflectedVoxels[index].triangles[j + 0] += 3 * mul;
                reflectedVoxels[index].triangles[j + 1] += -1 * mul;
                reflectedVoxels[index].triangles[j + 2] += -1 * mul;
                reflectedVoxels[index].triangles[j + 3] += -1 * mul;
                reflectedVoxels[index].triangles[j + 4] += -1 * mul;
                reflectedVoxels[index].triangles[j + 5] += -1 * mul;
            }
        }

        private void UpdateVertices()
        {
        }

        public void ApplyMirror()
        {
            foreach (VoxelMeshGenerator voxelMeshGenerator in reflectedVoxels)
            {
                voxelMeshGenerator.tag = "Voxel";
                voxelsControl.voxels.Add(voxelMeshGenerator);
            }
            myFlag.active = false;
            reflectedVoxels.Clear();
        }
    }
}
