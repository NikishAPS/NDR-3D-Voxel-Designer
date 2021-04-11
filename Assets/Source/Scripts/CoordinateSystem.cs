using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using MyGUI;
using System.Runtime.InteropServices;

namespace Voxelator
{
    public class CoordinateSystem : MonoBehaviour
    {
        public float size = 1f, radius;
        private Transform axes;

        private GameObject arrowX, arrowY, arrowZ;
        private GameObject scale, scaleX, scaleY, scaleZ;
        private GameObject rotatorX, rotatorY, rotatorZ;
        private GameObject additions;
        private Camera cameraMain;

        public bool onX, onY, onZ, onCenter;
        public bool captureX, captureY, captureZ, captureCenter;
        public Vector3 capturePos, capturePosStep;

        private CursorPanelsGUI cursorPanelsGUI;

        private Vector2 mouseDir, lastMouse;

        public GameObject coordinatesGUI;
        public GameObject extractor;
        public GameObject voxelArea;

        private Text textX, textY, textZ;
        private Text textAreaX, textAreaY, textAreaZ;

        [SerializeField]
        private Transform target;
        public Vector3 captureSpeed, captureOffset;
        public Vector3 saveTargetPos;

        public Vector3 captureStep, captureRot, captureScale, targetScale;

        private Vector3 targetPos;
        public float deltaSize;

        public bool enableCS;

        //[HideInInspector]
        public Vector3 mulSize = Vector3.one;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 rot = Vector3.zero;

        private Transform rotator;

        [SerializeField]
        private AxesControl axesControl = AxesControl.Arrow;

        [SerializeField]
        enum AxesControl
        {
            Arrow, Rotation, Scale
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SetCursorPos(int x, int y);

        void Start()
        {
            cameraMain = Camera.main;
            axes = transform.Find("Axes");

            arrowX = axes.Find("ArrowX").gameObject;
            arrowY = axes.Find("ArrowY").gameObject;
            arrowZ = axes.Find("ArrowZ").gameObject;

            additions = axes.Find("Additions").gameObject;

            scale = additions.transform.Find("Scale").gameObject;
            scaleX = additions.transform.Find("ScaleX").gameObject;
            scaleY = additions.transform.Find("ScaleY").gameObject;
            scaleZ = additions.transform.Find("ScaleZ").gameObject;

            rotatorX = additions.transform.Find("RotatorX").gameObject;
            rotatorY = additions.transform.Find("RotatorY").gameObject;
            rotatorZ = additions.transform.Find("RotatorZ").gameObject;

            cursorPanelsGUI = cameraMain.GetComponent<CursorPanelsGUI>();

            textX = coordinatesGUI.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>();
            textY = coordinatesGUI.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
            textZ = coordinatesGUI.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>();

            textAreaX = coordinatesGUI.transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>();
            textAreaY = coordinatesGUI.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>();
            textAreaZ = coordinatesGUI.transform.GetChild(1).GetChild(2).GetChild(0).GetComponent<Text>();

            textX.color = new Color(0, 0, 0, 1);
            textY.color = textZ.color = textX.color;

            SetEnable(false);
            Additions(false);

            //для поворота
            rotator = new GameObject().transform;
            rotator.name = "RotatorGO";

        }

        private void LateUpdate()
        {
            UpdateGUI();

            Vector3 offsetSizeNormalized = Vector3.one * (mulSize.x + mulSize.y + mulSize.z);
            axes.localScale = (mulSize) * size * Vector3.Distance(transform.position, cameraMain.transform.position);

            deltaSize = Vector3.Distance(transform.position, cameraMain.transform.position) * 0.05f;

            mouseDir = (Vector2)Input.mousePosition - lastMouse;
            lastMouse = Input.mousePosition;



            if (target == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, 256))
            {
                onX = hit.transform.localPosition.x != 0f;
                onY = hit.transform.localPosition.y != 0f;
                onZ = hit.transform.localPosition.z != 0f;
                onCenter = hit.transform.localPosition == Vector3.zero;


                if (Input.GetMouseButtonDown(0))
                {
                    if (cursorPanelsGUI.cursorInGameScene)
                    {
                        captureX = onX;
                        captureY = onY;
                        captureZ = onZ;
                        captureCenter = onCenter;

                        if(axesControl == AxesControl.Rotation)
                        {
                            captureX = onY && onZ;
                            captureY = onX && onZ;
                            captureZ = onX && onY;
                        }
                    }
                }
            }
            else
            {
                onX = onY = onZ = onCenter = false;
            }


            //определение, какой контрол оси захвачен (arrow, rot, scale)
            if (!Input.GetMouseButton(0))
                if (hit.collider)
                switch (hit.transform.parent.name[0])
                {
                    case 'A': axesControl = AxesControl.Arrow; ResetParam();  break; //если это коллайдеры стрелок 
                    case 'R': axesControl = AxesControl.Rotation; ResetParam(); break; //если это коллайдеры вращения 
                    case 'S': axesControl = AxesControl.Scale; ResetParam(); break; //если это коллайдеры скейла 

                    default: return;
                }



            arrowX.SetActive(false);
            arrowY.SetActive(false);
            arrowZ.SetActive(false);

            scale.SetActive(false);
            scaleX.SetActive(false);
            scaleY.SetActive(false);
            scaleZ.SetActive(false);

            rotatorX.SetActive(false);
            rotatorY.SetActive(false);
            rotatorZ.SetActive(false);

            transform.GetChild(0).gameObject.SetActive(enableCS);

            switch (axesControl)
            {
                case AxesControl.Arrow:
                    {
                        arrowX.SetActive(onX || captureX);
                        arrowY.SetActive(onY || captureY);
                        arrowZ.SetActive(onZ || captureZ);

                        Arrow();

                        UpdateCapture();
                    }
                    break;

                case AxesControl.Rotation:
                    {
                        rotatorX.SetActive(onY && onZ || captureX);
                        rotatorY.SetActive(onX && onZ || captureY);
                        rotatorZ.SetActive(onX && onY || captureZ);

                        Rotation();
                    }
                    break;

                case AxesControl.Scale:
                    {
                        scaleX.SetActive(onX || captureX);
                        scaleY.SetActive(onY || captureY);
                        scaleZ.SetActive(onZ || captureZ);

                        scale.SetActive(onCenter || captureCenter);

                        Scale();
                    }
                    break;
            }

            if (Capture() && false)
            {
                if (Input.mousePosition.x > Screen.width - 10)
                {
                    Vector2 cursorPos = new Vector2(20, Input.mousePosition.y);
                    SetCursorPos((int)cursorPos.x, (int)cursorPos.y);
                    lastMouse = cursorPos;

                }
                else if (Input.mousePosition.x < 10)
                {
                    Vector2 cursorPos = new Vector2(Screen.width - 20, Input.mousePosition.y);
                    SetCursorPos((int)cursorPos.x, (int)cursorPos.y);
                    lastMouse = cursorPos;
                }

                if (Input.mousePosition.y > Screen.height - 10)
                {
                    Vector2 cursorPos = new Vector2(Input.mousePosition.x, Screen.height - 20);
                    SetCursorPos((int)cursorPos.x, (int)cursorPos.y);
                    lastMouse = cursorPos;
                }
                else if (Input.mousePosition.y < 10)
                {
                    Vector2 cursorPos = new Vector2(Input.mousePosition.x, 20);
                    SetCursorPos((int)cursorPos.x, (int)cursorPos.y);
                    lastMouse = cursorPos;
                }
            }
        }


        


    private bool savePosX, savePosY, savePosZ;
        private Vector3 savePos;



        private void ResetParam()
        {
            mulSize = Vector3.one;
        }

        private void Arrow()
        {
            if (Input.GetMouseButton(0))
            {
                onX = captureX;
                onY = captureY;
                onZ = captureZ;

                if (captureX)
                {
                    capturePos.x += cameraMain.transform.TransformDirection(mouseDir).x * Time.deltaTime;
                    captureSpeed.x = cameraMain.transform.TransformDirection(mouseDir).x * Time.fixedDeltaTime * deltaSize;
                    captureOffset.x += captureSpeed.x;
                }
                if (captureY)
                {
                    capturePos.y += cameraMain.transform.TransformDirection(mouseDir).y * Time.deltaTime;
                    captureSpeed.y = cameraMain.transform.TransformDirection(mouseDir).y * Time.fixedDeltaTime * deltaSize;
                    captureOffset.y += captureSpeed.y;
                }
                if (captureZ)
                {
                    capturePos.z += cameraMain.transform.TransformDirection(mouseDir).z * Time.deltaTime;
                    captureSpeed.z = cameraMain.transform.TransformDirection(mouseDir).z * Time.fixedDeltaTime * deltaSize;
                    captureOffset.z += captureSpeed.z;
                }

            }
            else
            {
                captureX = false;
                captureY = false;
                captureZ = false;

                //capturePos = Vector3.zero;
                captureOffset = Vector3.zero;
                captureSpeed = Vector3.zero;

                UpdatePos();
            }
        }

        private void Rotation()
        {
            if (Input.GetMouseButton(0))
            {
                onX = captureX;
                onY = captureY;
                onZ = captureZ;

                if (Capture())
                {
                    rotator.rotation = Quaternion.identity;
                    rotator.position = target.position;
                    target.SetParent(rotator);
                }

                if (captureX)
                {
                    rotation *= Quaternion.Euler(cameraMain.transform.TransformDirection(mouseDir).x * Time.deltaTime * deltaSize, 0, 0);
                    rot.x = -cameraMain.transform.TransformDirection(mouseDir).y * Time.deltaTime * 25f * 2;
                }
                if (captureY)
                {
                    rot.y = cameraMain.transform.TransformDirection(mouseDir).x * Time.deltaTime * 25f * 2;
                }
                if (captureZ)
                {
                    rot.z = cameraMain.transform.TransformDirection(mouseDir).y * Time.deltaTime * 25f * 2;
                }

                rotation.eulerAngles = rot;
                rotator.eulerAngles = rot;

                target.SetParent(null);
                rot = Vector3.zero;

                //target.rotation = rotation;
            }
            else
            {
                captureX = false;
                captureY = false;
                captureZ = false;

                //captureScale = target.localScale;
                //mulSize = Vector3.one;
            }
       }

       private void Scale()
       {
           if (Input.GetMouseButton(0))
           {
               onX = captureX;
               onY = captureY;
               onZ = captureZ;
               onCenter = captureZ;

               if (captureX)
               {
                    mulSize.x += cameraMain.transform.TransformDirection(mouseDir).x * Time.deltaTime;
                }
                if (captureY)
               {
                   mulSize.y += cameraMain.transform.TransformDirection(mouseDir).y * Time.deltaTime;
               }
               if (captureZ)
               {
                   mulSize.z += cameraMain.transform.TransformDirection(mouseDir).z * Time.deltaTime;
               }
               if (captureCenter)
               {
                   mulSize += Vector3.one * (mouseDir.x + mouseDir.y) * Time.deltaTime;
               }

           }
           else
           {
               captureX = false;
               captureY = false;
               captureZ = false;
               captureCenter = false;

               captureScale = targetScale;
               mulSize = Vector3.one;
           }


           targetScale = new Vector3 (mulSize.x *  captureScale.x, mulSize.y * captureScale.y, mulSize.z * captureScale.z);
       }


       public bool NotCapture()
       {
           return !onX && !onY && !onZ && !onCenter;
       }

        public bool Capture()
        {
            return captureX || captureY || captureZ || captureCenter;
        }

       private void UpdateGUI()
       {
           if (!cursorPanelsGUI.cursorInGameScene)
           {
               coordinatesGUI.transform.GetChild(0).gameObject.SetActive(false);
               coordinatesGUI.transform.GetChild(1).gameObject.SetActive(false);
               return;
           }
           coordinatesGUI.transform.GetChild(0).gameObject.SetActive(extractor.activeSelf);
           coordinatesGUI.transform.GetChild(1).gameObject.SetActive(voxelArea.activeSelf);
           coordinatesGUI.transform.position = Input.mousePosition;

           if (extractor.activeSelf)
           {
               textX.text = ((int)extractor.transform.position.x).ToString();
               textY.text = ((int)extractor.transform.position.y).ToString();
               textZ.text = ((int)extractor.transform.position.z).ToString();
           }

           if (voxelArea.activeSelf)
           {
               textAreaX.text = ((int)voxelArea.transform.localScale.x).ToString();
               textAreaY.text = ((int)voxelArea.transform.localScale.y).ToString();
               textAreaZ.text = ((int)voxelArea.transform.localScale.z).ToString();

               float max = textX.text.Length;
               if (max < textY.text.Length) max = textX.text.Length;
               else if (max < textZ.text.Length) max = textZ.text.Length;

               coordinatesGUI.transform.GetChild(1).transform.localPosition = new Vector3(50 + 30 * max, 0, 0);
           }
       }

       //обработка захвата осей
       private void UpdateCapture()
       {
           if (captureX)
           {
               if(!savePosX)
               {
                   savePos = transform.position;
                   savePosX = true;
               }
               transform.position = savePos + captureOffset;
               captureStep.x += captureSpeed.x;
           }
           else
           {
               //captureStep.x = transform.position.x;
               savePosX = false;
           }
           if (captureY)
           {
               if (!savePosY)
               {
                   savePos = transform.position;
                   savePosY = true;
               }
               transform.position = savePos + captureOffset;
               captureStep.y += captureSpeed.y;
           }
           else
           {
               savePosY = false;
           }
           if (captureZ)
           {
               if (!savePosZ)
               {
                   savePos = transform.position;
                   savePosZ = true;
               }
               transform.position = savePos + captureOffset;
               captureStep.z += captureSpeed.z;
           }
           else
           {
               savePosZ = false;
           }
       }

       //установить цель
       public void SetTarget(Transform target)
       {
           if(this.target != target)
           {
               this.target = target;
               if (target == null)
               {
                   //saveTargetPos = Vector3.zero;
               }
               else
               {

                   transform.position = target.position;
                   captureScale = target.localScale;
               }
           }
       }

       //установить позицию
       public void SetTargetPos(Vector3 pos)
       {
           //target = null;
           targetPos = pos;
       }

       public void SetStep(Vector3 curStepValue)
       {
           if (!captureX)
               captureStep.x = curStepValue.x;
           if (!captureY)
               captureStep.y = curStepValue.y;
           if (!captureZ)
               captureStep.z = curStepValue.z;
       }

       public void SetSaveTargetPos(Vector3 pos)
       {
           saveTargetPos = pos;
       }

       //обновить позицию системы координат
       private void UpdatePos()
       {
           if (target != null)
           {
               transform.position = target.position;
           }
           else
           {
               transform.position = targetPos;
           }
       }

       public void SetEnable(bool enabled)
       {
           enableCS = enabled;
           transform.GetChild(0).gameObject.SetActive(enableCS);
        }


        private bool OnAxis(Vector2 pos, float radius)
       {
           Vector2 mousePos = Input.mousePosition;

           return Vector2.Distance(mousePos, pos) <= radius;
       }


       public void Additions(bool active)
       {
           additions.SetActive(active);
       }










       void LateUpdate0()
       {
           coordinatesGUI.transform.GetChild(0).gameObject.SetActive(extractor.activeSelf);
           coordinatesGUI.transform.GetChild(1).gameObject.SetActive(voxelArea.activeSelf);
           coordinatesGUI.transform.position = Input.mousePosition;

           if (extractor.activeSelf)
           {
               textX.text = ((int)extractor.transform.position.x).ToString();
               textY.text = ((int)extractor.transform.position.y).ToString();
               textZ.text = ((int)extractor.transform.position.z).ToString();
           }

           if (voxelArea.activeSelf)
           {
               textAreaX.text = ((int)voxelArea.transform.localScale.x).ToString();
               textAreaY.text = ((int)voxelArea.transform.localScale.y).ToString();
               textAreaZ.text = ((int)voxelArea.transform.localScale.z).ToString();

               float max = textX.text.Length;
               if (max < textY.text.Length) max = textX.text.Length;
               else if (max < textZ.text.Length) max = textZ.text.Length;

               coordinatesGUI.transform.GetChild(1).transform.localPosition = new Vector3(50 + 30 * max, 0, 0);
           }


           axes.localScale = new Vector3(1, 1, 1) * size * Vector3.Distance(transform.position, cameraMain.transform.position);

           /*onX = OnAxis(cameraMain.WorldToScreenPoint(axes.position + Vector3.right * axes.localScale.x), radius * axes.localScale.x);
           onY = OnAxis(cameraMain.WorldToScreenPoint(axes.position + Vector3.up * axes.localScale.y), radius * axes.localScale.y);
           onZ = OnAxis(cameraMain.WorldToScreenPoint(axes.position + Vector3.forward * axes.localScale.z), radius * axes.localScale.z);
           */
                Vector2 mouseDir = (Vector2)Input.mousePosition - lastMouse;
            lastMouse = Input.mousePosition;



            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, 256))
            {
                onX = hit.transform.localPosition.x != 0f;
                onY = hit.transform.localPosition.y != 0f;
                onZ = hit.transform.localPosition.z != 0f;


                if (Input.GetMouseButtonDown(0))
                {
                    if (cursorPanelsGUI.cursorInGameScene)
                    {
                        captureX = onX;
                        captureY = onY;
                        captureZ = onZ;
                    }
                }
            }
            else
            {
                onX = onY = onZ = false;
            }

            if (Input.GetMouseButton(0))
            {
                onX = captureX;
                onY = captureY;
                onZ = captureZ;

                if (captureX)
                {
                    capturePos.x += cameraMain.transform.TransformDirection(mouseDir).x * Time.deltaTime;
                }
                if (captureY)
                {
                    capturePos.y += cameraMain.transform.TransformDirection(mouseDir).y * Time.deltaTime;
                }
                if (captureZ)
                {
                    capturePos.z += cameraMain.transform.TransformDirection(mouseDir).z * Time.deltaTime;
                }

            }
            else
            {
                captureX = false;
                captureY = false;
                captureZ = false;

                capturePos = Vector3.zero;
            }


            arrowX.SetActive(onX);
            arrowY.SetActive(onY);
            arrowZ.SetActive(onZ);




            if (captureX)
            {
                if (!savePosX)
                {
                    savePos = transform.position;
                    savePosX = true;
                }

                transform.position = new Vector3(savePos.x, transform.position.y, transform.position.z) + capturePos;

            }
            else
            {
                savePosX = false;
            }

            if (captureY)
            {
                if (!savePosY)
                {
                    savePos = transform.position;
                    savePosY = true;
                }

                transform.position = new Vector3(transform.position.x, savePos.y, transform.position.z) + capturePos;
            }
            else
            {
                savePosY = false;
            }

            if (captureZ)
            {
                if (!savePosZ)
                {
                    savePos = transform.position;
                    savePosZ = true;
                }

                transform.position = new Vector3(transform.position.x, transform.position.y, savePos.z) + capturePos;
            }
            else
            {
                savePosZ = false;
            }

        }

    }
}
