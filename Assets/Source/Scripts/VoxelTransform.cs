/*
 * Скрипт для перемещения вокселей
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voxelator
{
    public class VoxelTransform : MonoBehaviour
    {
        public Vector3Int position;
        public Transform voxel;

        public InputField inputFieldX, inputFieldY, inputFieldZ;
        public Text textRotX, textRotY, textRotZ;

        public VirtualVoxels virtualVoxels;

        private Transform grid;

        private bool rot;
        private Vector3 newRot;

        private VoxelsControl voxelsControl;

        private Transform pointRot; //точка вращения
        private bool update = true;

        private void Awake()
        {
            virtualVoxels.Awake();
            voxelsControl = GetComponent<VoxelsControl>();
            pointRot = new GameObject().transform;
            pointRot.name = "PointRot";
        }

        void Start()
        {
            grid = GameObject.Find("Grid").transform;
        }


        private void RotationParameters()
        {
            //определение центра
            Vector3 centerVoxels = Vector3.zero;
            for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
            {
                centerVoxels += voxelsControl.selectedVoxels[i].transform.position;
            }
            centerVoxels /= voxelsControl.selectedVoxels.Count;


            //определение точки вращения
            pointRot.position = voxelsControl.selectedVoxels[0].transform.position;
            float dist = Vector3.Distance(pointRot.position, centerVoxels);
            for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
            {
                float newDist = Vector3.Distance(voxelsControl.selectedVoxels[i].transform.position, centerVoxels);
                if (newDist < dist)
                {
                    dist = newDist;
                    pointRot.position = voxelsControl.selectedVoxels[i].transform.position;
                }
            }
        }

        private void Update()
        {
            UpdateTransform();
            virtualVoxels.Update();

            if (rot)
            {
                for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
                {
                    if(voxelsControl.selectedVoxels[i].transform.parent != null)
                    {
                        voxelsControl.selectedVoxels[i].transform.parent.SetParent(pointRot);
                    }
                    else
                    {
                        voxelsControl.selectedVoxels[i].transform.SetParent(pointRot);
                    }
                }

                   Quaternion q = Quaternion.Euler(newRot.x, newRot.y, newRot.z);
               // voxel.rotation = Quaternion.RotateTowards(voxel.rotation, q, Time.deltaTime * 300f);

                pointRot.rotation = Quaternion.RotateTowards(pointRot.rotation, q, Time.deltaTime * 300f);


                if (voxelsControl.selectedVoxels.Count > 1 && false)
                {
                    for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
                    {
                        voxelsControl.selectedVoxels[i].transform.rotation = voxel.rotation;
                        //voxelsControl.selectedVoxels[i].transform.position = voxel.rotation;
                    }
                }
                if (pointRot.rotation == q)
                {
                   // voxel.GetComponent<VoxelMeshGenerator>().CalculateRot();

                    for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
                    {
                        if (voxelsControl.selectedVoxels[i].transform.parent.name == "Part")
                        {
                            Transform parent = voxelsControl.selectedVoxels[i].transform.parent;
                            if (parent.rotation != Quaternion.identity)
                            {
                                parent.SetParent(null);
                                GameObject parent0 = new GameObject();


                                while (parent.childCount > 0)
                                {
                                    parent.GetChild(0).SetParent(parent0.transform);
                                }
                                parent.rotation = Quaternion.identity;
                                while (parent0.transform.childCount > 0)
                                {
                                    parent0.transform.GetChild(0).SetParent(parent);
                                }

                                Destroy(parent0);
                            }
                        }
                        else
                        {
                            voxelsControl.selectedVoxels[i].transform.parent = null;
                        }


                        voxelsControl.selectedVoxels[i].GetComponent<VoxelMeshGenerator>().CalculateRot();

                        voxelsControl.saveCapturePos[i] = new Vector3 (DropRound(voxelsControl.selectedVoxels[i].transform.position.x, 1),
                            DropRound(voxelsControl.selectedVoxels[i].transform.position.y, 1),
                            DropRound(voxelsControl.selectedVoxels[i].transform.position.z, 1));
                    }

                    pointRot.rotation = Quaternion.identity;
                    rot = false;
                }
            }
        }

        private void LateUpdate()
        {

        }

        public void UpdateTransform()
        {
            if (rot) return;
            if (voxel != null)
            {
                if (CorrectInput(inputFieldX.text) && CorrectInput(inputFieldY.text) && CorrectInput(inputFieldZ.text))
                {
                    Vector3Int col = GridCollision(position);

                    inputFieldX.text = col.x.ToString();
                    inputFieldY.text = col.y.ToString();
                    inputFieldZ.text = col.z.ToString();

                    position.x = int.Parse(inputFieldX.text);
                    position.y = int.Parse(inputFieldY.text);
                    position.z = int.Parse(inputFieldZ.text);

                    voxel.position = position + new Vector3(0, 0.5f, 0);
                }
            }

        }


        public void FillVirtual()
        {
            virtualVoxels.Fill();
        }

        public void SetVoxel(Transform voxel)
        {
            if (this.voxel != voxel)
            {
                //voxel.position -= new Vector3(0.5f, 0.5f, 0.5f);
                //position = new Vector3Int((int)(voxel.position.x), (int)(voxel.position.y - 0.5f), (int)(voxel.position.z));
                SetPosition(voxel.position);
                //aprint(voxel.position.y);

                inputFieldX.text = position.x.ToString();
                inputFieldY.text = position.y.ToString();
                inputFieldZ.text = position.z.ToString();

                this.voxel = voxel;
            }

        }

        public void SetPositionX(float posX)
        {
            position.x = (int)Mathf.Round(posX);
            //UpdateTransform();
        }

        public void SetPositionY(float posY)
        {
            position.y = Round(posY);
            //UpdateTransform();
        }

        public void SetPositionZ(float posZ)
        {
            position.z = (int)Mathf.Round(posZ);
            //UpdateTransform();
        }

        public void SetPosition(Vector3 position)
        {
            this.position.x = Round(position.x);
            this.position.y = Round(position.y);
            this.position.z = Round(position.z);
        }

        public int Round(float value)
        {
            if (value - (int)value > 0.5f)
            {
                
                return (int)value + 1;
            }
            else
            {
                return (int)value;
            }
        }

        public float DropRound(float value, int count)
        {
            count *= 10;
            value *= count;
            value = (int)value;
            value /= count;
            return value;
        }

        public Vector3 GetPosition()
        {
            return position;
            return new Vector3(int.Parse(inputFieldX.text), int.Parse(inputFieldY.text), int.Parse(inputFieldZ.text));
        }

        private Vector3Int GridCollision(Vector3Int point)
        {
            //point -= new Vector3(0.5f, 0.5f, 0.5f);

            if (point.x < 0) point.x = 0;
            else if(point.x > grid.localScale.x * 10f - 1f) point.x = (int)(grid.localScale.x * 10f - 1f);

            if (point.y < 0) point.y = 0;
            else if (point.y > grid.lossyScale.y * 10 - 1f) point.y = (int)(grid.lossyScale.y * 10 - 1f);

            if (point.z < 0) point.z = 0;
            else if (point.z > grid.localScale.z * 10f - 1f) point.z = (int)(grid.localScale.z * 10f - 1f);

            return point;
        }

        private bool CorrectInput(string text)
        {
            if (text == "") return false;

            if (text.Length == 1)
            {
                if (text[0] == '-')
                {
                    return false;
                }
            }

            return true;
        }

        public void RotX(int dir)
        {
            if (rot || voxel == null) return;
            Vector3 curRot = voxel.eulerAngles;
            if (curRot.x <= -180 && curRot.x > -90) dir = -dir;
            newRot = new Vector3(curRot.x + dir, curRot.y, curRot.z);
            rot = true;
            RotationParameters();
        }
        public void RotY(int dir)
        {
            if (rot || voxel == null) return;
            Vector3 curRot = voxel.eulerAngles;
            newRot = new Vector3(curRot.x, curRot.y + dir, curRot.z);
            rot = true;
            RotationParameters();
        }
        public void RotZ(int dir)
        {
            if (rot || voxel == null) return;
            Vector3 curRot = voxel.eulerAngles;
            newRot = new Vector3(curRot.x, curRot.y, curRot.z + dir);
            rot = true;
            RotationParameters();
        }
    }

    [System.Serializable]
    public struct VirtualVoxels
    {
        public GameObject voxelVirtual;
        public MyGUI.MyFlag myFlagVirtual;
        public InputField inputField_posX, inputField_posY, inputField_posZ;
        public InputField inputField_countX, inputField_countY, inputField_countZ;


        private Vector3Int pos, scale;

        private Transform grid;
        private CreateInstanceVoxel createInstanceVoxel;
        private MyGUI.MyColor myColor;

        private CoordinateSystem coordinateSystem;

        public void Awake()
        {
            voxelVirtual = GameObject.Find("VoxelVirtual");
            grid = GameObject.Find("Grid").transform;
            createInstanceVoxel = GameObject.Find("CreateInstanceVoxel").GetComponent<CreateInstanceVoxel>();
            myColor = Camera.main.GetComponent<MyGUI.CursorPanelsGUI>().windowGUI[1].panel.transform.GetChild(0).transform.Find("Colors").GetComponent<MyGUI.MyColor>();

            coordinateSystem = GameObject.Find("CoordinateSystem").GetComponent<CoordinateSystem>();

        }

        public void Update()
        {
            voxelVirtual.SetActive(false);

            if (myFlagVirtual.active)
            {
                if (CorrectInput(inputField_posX.text) && CorrectInput(inputField_posY.text) && CorrectInput(inputField_posZ.text))
                {
                    pos = GridCollision(new Vector3Int(int.Parse(inputField_posX.text), int.Parse(inputField_posY.text), int.Parse(inputField_posZ.text)));

                    inputField_posX.text = pos.x.ToString();
                    inputField_posY.text = pos.y.ToString();
                    inputField_posZ.text = pos.z.ToString();

                    voxelVirtual.transform.position = pos + new Vector3(0, 0.5f, 0);

                    /*if (posVirtual.Count != col.x * col.y * col.z)
                    {

                    }

                     */


                    if (CorrectInput(inputField_countX.text) && CorrectInput(inputField_countY.text) && CorrectInput(inputField_countZ.text))
                    {
                        scale = GridCollisionCount(new Vector3Int(int.Parse(inputField_countX.text), int.Parse(inputField_countY.text), int.Parse(inputField_countZ.text)) + pos);
                        scale -= pos;

                        if (scale.x == 0 || scale.y == 0 || scale.z == 0) return;

                        inputField_countX.text = scale.x.ToString();
                        inputField_countY.text = scale.y.ToString();
                        inputField_countZ.text = scale.z.ToString();


                        voxelVirtual.transform.position = pos + new Vector3(0.5f * scale.x - 0.5f, 0.5f * scale.y, 0.5f * scale.z - 0.5f);
                        voxelVirtual.transform.localScale = scale + new Vector3 (0.01f, 0.01f, 0.01f);

                        voxelVirtual.SetActive(true);
                    }
                }
            }
        }

        public void TransformArea(Vector3 newPos)
        {
            inputField_posX.text = ((int)newPos.x).ToString();
            inputField_posY.text = ((int)newPos.y).ToString();
            inputField_posZ.text = ((int)newPos.z).ToString();
        }

        public Vector3 GetPosition()
        {
            return GridCollision(new Vector3Int(int.Parse(inputField_posX.text), int.Parse(inputField_posY.text), int.Parse(inputField_posZ.text)));
        }

       

        public void Fill()
        {
            if (!myFlagVirtual.active) return;
            Collider[] collider = Physics.OverlapBox(pos, scale);

            for (int x = 0; x < scale.x; x++)
            {
                for (int y = 0; y < scale.y; y++)
                {
                    for (int z = 0; z < scale.z; z++)
                    {
                        Vector3 vPos = pos + new Vector3(x, y + 0.5f, z);
                        bool b = true;

                        for (int i = 0; i < collider.Length; i++)
                        {
                            if(vPos == collider[i].transform.position)
                            {
                                b = false;
                                break;
                            }
                        }
                        if (b)
                        {
                            VoxelMaterials voxelMaterials = createInstanceVoxel.Create(vPos).GetComponent<VoxelMaterials>();
                            voxelMaterials.standard = myColor.GetCurrentMaterial();
                            voxelMaterials.Start();
                            voxelMaterials.Standard();
                        }
                    }
                }
            }
        }

        private bool CorrectInput(string text)
        {
            if (text == "") return false;

            if (text.Length == 1)
            {
                if (text[0] == '-')
                {
                    return false;
                }
            }

            return true;
        }

        private Vector3Int GridCollision(Vector3Int point)
        {
            //point -= new Vector3(0.5f, 0.5f, 0.5f);

            if (point.x < 0) point.x = 0;
            else if (point.x > grid.localScale.x * 10f - 1f) point.x = (int)(grid.localScale.x * 10f - 1f);

            if (point.y < 0) point.y = 0;
            else if (point.y > grid.lossyScale.y * 10 - 1f) point.y = (int)(grid.lossyScale.y * 10 - 1f);

            if (point.z < 0) point.z = 0;
            else if (point.z > grid.localScale.z * 10f - 1f) point.z = (int)(grid.localScale.z * 10f - 1f);

            return point;
        }

        private Vector3Int GridCollisionCount(Vector3Int point)
        {
            //point -= new Vector3(0.5f, 0.5f, 0.5f);

            if (point.x < 0) point.x = 0;
            else if (point.x > grid.localScale.x * 10f) point.x = (int)(grid.localScale.x * 10f);

            if (point.y < 0) point.y = 0;
            else if (point.y > grid.lossyScale.y * 10) point.y = (int)(grid.lossyScale.y * 10);

            if (point.z < 0) point.z = 0;
            else if (point.z > grid.localScale.z * 10f) point.z = (int)(grid.localScale.z * 10f);

            return point;
        }
    }
}
