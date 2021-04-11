using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxelator
{
    public class CameraControl : MonoBehaviour
    {
        public Vector3 target;

        public float sensitivity = 5f, zoom;

        public Vector3 offset;//, offsetTarget;

        private Vector3 _target;

        private Vector2 angles, lastMouse;

        private bool viewNormal;

        private VoxelsControl voxelsControl;

        private float speedView;

        private Transform grid;


        void Start()
        {
            voxelsControl = GetComponent<VoxelsControl>();

            grid = GameObject.Find("Grid").transform;

            _target = new Vector3(grid.localScale.x, 0, grid.localScale.z) * 5f;
            offset.z = -15;

            angles = new Vector2(320f, -50f);
        }


        void Update()
        {
            if (Input.GetMouseButton(2))
            {
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    angles.x = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
                    angles.y += Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;

                    /*if (angles.x > 0) angles.x -= 360;
                    else if (angles.x < -360) angles.x += 360;
                    if (angles.y > 0) angles.y -= 360;
                    else if (angles.y < -360) angles.y += 360;*/

                    angles.y = Mathf.Clamp(angles.y, -90, 90);
                }
                else
                {
                    Vector2 mouseDir = (lastMouse - (Vector2)Input.mousePosition) * 0.03f;

                    //offsetTarget.x += mouseDir.x;
                    //offsetTarget.y += mouseDir.y;

                    _target += transform.TransformDirection(mouseDir);
                }
            }


            lastMouse = Input.mousePosition;



            if (Input.GetKeyDown(KeyCode.Space))
            {
                //speedView = Vector3.Distance(transform.position, voxelsControl.selectedVoxel.transform.position);
                viewNormal = true;
                //target = voxelsControl.selectedVoxel != null ? voxelsControl.selectedVoxel.transform : null;

                float mul = 1; //Смещене цели наблюдения при выделении нескольких объектов


                if (voxelsControl.selectedVoxels.Count > 0)
                {
                    target = Vector3.zero;
                    for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
                    {
                        target += voxelsControl.selectedVoxels[i].transform.position;
                    }

                    target /= voxelsControl.selectedVoxels.Count;


                    //Если выделено несколько вокселей, то камеру надо настроить так, чтобы видеть их все
                    if (voxelsControl.selectedVoxels.Count > 1)
                    {
                        for (int i = 0; i < voxelsControl.selectedVoxels.Count; i++)
                        {
                            for (int j = 0; j < voxelsControl.selectedVoxels.Count; j++)
                            {
                                if (Vector3.Distance(voxelsControl.selectedVoxels[i].transform.position, voxelsControl.selectedVoxels[j].transform.position) > mul)
                                {
                                    mul = Vector3.Distance(voxelsControl.selectedVoxels[i].transform.position, voxelsControl.selectedVoxels[j].transform.position);
                                }
                            }
                        }

                        mul *= 0.3f;
                        if (mul < 1f) mul = 1f;
                    }

                    //target = voxelsControl.selectedVoxels[voxelsControl.selectedVoxels.Count - 1].transform.position;
                    speedView = Vector3.Distance(transform.position, target);
                }
                else if(voxelsControl.selectedImported == null)
                {

                    target = grid.localScale * 5f;

                    mul = 0;
                    if (grid.localScale.x > mul) mul = grid.localScale.x;
                    if (grid.localScale.y > mul) mul = grid.localScale.y;
                    if (grid.localScale.z > mul) mul = grid.localScale.z;



                    mul *= 4f;

                    speedView = Vector3.Distance(transform.position, Vector3.zero);

                   
                }
                else if(voxelsControl.mode == 4)
                {
                    target = voxelsControl.selectedImported.position;
                    speedView = Vector3.Distance(transform.position, target);
                }

                _target += transform.forward * (offset.z + 4f * mul);
                offset.z = -4f * mul;
            }

            if (viewNormal)
            {

                _target = Vector3.MoveTowards(_target, target, Time.deltaTime * 1.5f * speedView);
                //offsetTarget = Vector3.MoveTowards(offsetTarget, Vector3.zero, Time.deltaTime * 15f * speedView);

                //if(_target == _t && offsetTarget == Vector3.zero)
                if (_target == target)
                {
                    viewNormal = false;
                }
            }


            float multiplierZoom = Input.GetKey(KeyCode.LeftShift) ? 4f : Input.GetKey(KeyCode.LeftControl) ? 0.25f : 1f;
            if (Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoom * multiplierZoom * Vector3.Distance(transform.position, _target);
            else if (Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoom * multiplierZoom * Vector3.Distance(transform.position, _target);
            offset.z = Mathf.Clamp(offset.z, -2500, -2f);

            transform.localEulerAngles = new Vector3(-angles.y, angles.x, 0);
            transform.position = transform.localRotation * (offset + Vector3.forward) + _target;


            /*
            //Camera position
            Vector3 pos = new Vector3(0,
                cameraDist * Sin(angles.y),
                cameraDist * Cos(angles.y));

            pos = new Vector3(cameraDist * Sin(angles.x),
                 cameraDist * Sin(angles.y) * Cos(angles.y),
                 cameraDist * Cos(angles.x) * Sin(angles.y));

            Camera.main.transform.position = pos;


            //Camera rotation
            Vector3 rot = Vector3.zero;


            switch (QuarterDefinition(angles.y))
            {
                case 1: rot.x = Vector3.Angle(target - pos, target -  new Vector3(pos.x, 0, pos.z)); break;
                case 2: rot.x = -Vector3.Angle(target - pos, target -  new Vector3(pos.x, 0, pos.z)) + 180; break;
                case 3: rot.x = Vector3.Angle(target - pos, target -  new Vector3(pos.x, 0, pos.z)) + 180; break;
                case 4: rot.x = -Vector3.Angle(target - pos, target -  new Vector3(pos.x, 0, pos.z)); break;
                default: break;
            }

            switch (QuarterDefinition(angles.x))
            {
                case 1: rot.y = -Vector3.Angle(target - pos, Vector3.forward); break;
                case 2: rot.y = -Vector3.Angle(target - pos, Vector3.forward); break;
                case 3: rot.y = Vector3.Angle(target - pos, Vector3.forward); break;
                case 4: rot.y = Vector3.Angle(target - pos, Vector3.forward); break;
                default: break;
            }
            print(QuarterDefinition(angles.x));
            Camera.main.transform.eulerAngles = rot;


            /*Camera.main.transform.position = new Vector3(cameraDist * Sin(angles.x) * Cos(angles.y),
                cameraDist * Sin(angles.y) * Cos(angles.x),
                cameraDist * Cos(angles.x)) + target;*/





            /* Camera.main.transform.position = new Vector3(cameraDist * Mathf.Cos(cameraPos.x * Mathf.Deg2Rad),
                 0,
                 cameraDist * Mathf.Sin(cameraPos.x * Mathf.Deg2Rad));*/





        }

        private float Sin(float angle)
        {
            return Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        private float Cos(float angle)
        {
            return Mathf.Cos(angle * Mathf.Deg2Rad);
        }

        private int QuarterDefinition(float angle)
        {
            if (angle < 0 && angle > -90)
                return 1;
            else if (angle < -90 && angle > -180)
                return 2;
            else if (angle < -180 && angle > -270)
                return 3;
            else return 4;
        }

    }
}
