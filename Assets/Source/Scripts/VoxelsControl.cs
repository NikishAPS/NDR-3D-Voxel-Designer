using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyGUI;






namespace Voxelator
{
    public class VoxelsControl : MonoBehaviour
    {
        //public GameObject selectedVoxel;
        //[HideInInspector]
        public List<VoxelMeshGenerator> voxels;
        public List<GameObject> selectedVoxels;
        [SerializeField]
        //private List<Transform> selectedImported = new List<Transform>();
        public Transform selectedImported;
        private CreateInstanceVoxel createInstanceVoxel;
        private Transform grid;
        private CoordinateSystem coordinateSystem;
        public int selectedVertex;

        private VoxelTransform voxelTransform;
        private TransformOBJ transformOBJ;

        public GameObject extractor, outlineVoxel, outlineVertex, highlightVertex, extractorVertex;
        //public GameObject extractor, outlineVoxel, outlineVertex, highlightVertex;

        public MyFlag autoFit;
        public MySwitch mySwitch, mySwitchVoxelSettings;
        public MyFlag showBounds;
        public MySlider sliderX, sliderY, sliderZ;

        private bool selectorsVertices;
        private GameObject[] gb_selectorsVertices;

        private MeshCollider meshColliderGrid;

        private Vector3 mouseLastSpeed;

        private CursorPanelsGUI cursorPanelsGUI;

        private float mouseMoveSpeed;


        public Vector3Int[] saveVoxelTransform;
        public bool savePosX, savePosY, savePosZ;

        private MaterialsControl materialsControl;

        public Transform voxelArea, voxelSelectArea;
        private Vector3 voxelAreaStartPos;

        private MessageSystem messageSystem;

        [HideInInspector]
        public int mode;

        private Vector3 mouseDir;

        public ScreenSelector screenSelector;

        private Mode[] modes;
       
        void Start()
        {
            modes = new Mode[]
                {
                    new BuildMode(this),
                    new SelectMode(this)
                };



            grid = GameObject.Find("Grid").transform;

            meshColliderGrid = grid.GetComponent<GridControl>().meshCollider;

            coordinateSystem = GameObject.Find("CoordinateSystem").GetComponent<CoordinateSystem>();

            createInstanceVoxel = GameObject.Find("CreateInstanceVoxel").GetComponent<CreateInstanceVoxel>();

            voxelTransform = GetComponent<VoxelTransform>();
            transformOBJ = GetComponent<TransformOBJ>();
            cursorPanelsGUI = GetComponent<CursorPanelsGUI>();

            materialsControl = GetComponent<MaterialsControl>();

            csPos = new GameObject().transform;
            csPos.name = "CS_POS";

            messageSystem = GameObject.Find("Canvas").transform.Find("MessageSystem").GetComponent<MessageSystem>();
        }

        void Update()
        {


            mouseDir = Input.mousePosition - mouseLastSpeed;
            mouseLastSpeed = Input.mousePosition;


            if (Input.GetKeyDown(KeyCode.Q)) mySwitch.mode = 1;
            if (Input.GetKeyDown(KeyCode.W)) mySwitch.mode = 2;
            if (Input.GetKeyDown(KeyCode.E)) mySwitch.mode = 3;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                mySwitch.mode++;
                if (mySwitch.mode > 3)
                    mySwitch.mode = 1;
            }

            if (mySwitch.mode != mode)
            {
                ChangedMode();
            }


            if (!Input.GetKey(KeyCode.LeftAlt))
            {
                switch (mySwitch.mode)
                {
                    //case 1: modes[0].UpdateMode(); break;
                    case 1: Mode1(); break;
                    case 2: Mode2(); break;
                    case 3: Mode3(); break;
                    case 4: Mode4(); break;
                }
            }
            else
            {
                return;
            }
            

            grid.transform.GetChild(0).gameObject.SetActive(showBounds.active);
        }

        private bool InGrid(Vector3 point)
        {
            return point.x >= 0f && point.x <= grid.localScale.x * 10f - 1f &&
                point.y >= 0 && point.y <= grid.lossyScale.y * 10f &&
               point.z >= 0f && point.z <= grid.localScale.z * 10f - 1f;
        }

        private void ExtractorRot(ref RaycastHit hit)
        {
            if (hit.collider == null) return;

            switch (hit.collider.tag)
            {
                case "Grid":
                    extractor.transform.rotation = Quaternion.identity;
                    break;

                case "Voxel":
                    extractor.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    break;

                default: extractor.transform.rotation = Quaternion.identity; break;
            }

        }

        private bool ExtractorPos(ref RaycastHit hit)
        {
            //print(hit.collider);
            bool col = hit.collider != null;
            if (!col) return false;

            Vector3 point = hit.point;
            switch (hit.collider.tag)
            {
                case "Voxel":
                    point = hit.transform.position + extractor.transform.up * 0.5f;
                    break;

                default:
                    point.x = (point.x - (int)point.x < 0.5f) ? (int)point.x : (int)point.x + 1;
                    point.y = (point.y - (int)point.y < 0.5f) ? (int)point.y : (int)point.y + 1;
                    point.z = (point.z - (int)point.z < 0.5f) ? (int)point.z : (int)point.z + 1;
                    break;
            }

            extractor.transform.position = point;

            return col;
        }

        private Vector3 OffsetExtractorPos(Vector3 point)
        {
            float mulX = point.x > 0 ? 1 : -1;
            float mulZ = point.z > 0 ? 1 : -1;
            return new Vector3(0.5f * mulX * 2f, 0f, 0.5f * mulZ * 2f);
        }

        private bool MouseButton(int index, bool hold)
        {
            if (hold)
            {
                if (Input.GetMouseButton(index))
                {
                    return cursorPanelsGUI.cursorInGameScene;
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(index))
                {
                    return cursorPanelsGUI.cursorInGameScene;
                }
            }
            return false;
        }

        private bool MouseButtonUp(int index)
        {
            return Input.GetMouseButtonUp(index) && cursorPanelsGUI.cursorInGameScene;
        }

        public void Clear()
        {
            GameObject[] go_Voxels = GameObject.FindGameObjectsWithTag("Voxel");
            GameObject[] go_Imported = GameObject.FindGameObjectsWithTag("Imported");

            for (int i = 0; i < voxels.Count; i++)
            {
                Destroy(voxels[i].gameObject);
            }
            voxels.Clear();
            for (int i = 0; i < go_Voxels.Length; i++)
            {
                //Destroy(go_Voxels[i].gameObject);
            }
            selectedVoxels.Clear();
            createInstanceVoxel.voxelsCopy.Clear();

            for (int i = 0; i < go_Imported.Length; i++)
            {
                Destroy(go_Imported[i].gameObject);
            }

            mySwitch.mode = 1;

            for (int i = 0; i < mySwitchVoxelSettings.active.Length; i++)
            {
                mySwitchVoxelSettings.active[i] = true;
            }

            saveCapturePos.Clear();
        }

        public void Center()
        {
            if (selectedVoxels.Count > 0)
            {
                voxelTransform.SetPositionX((int)(grid.transform.localScale.x * 5f));
                voxelTransform.SetPositionY((int)(grid.localScale.y * 5f));
                voxelTransform.SetPositionZ((int)(grid.transform.localScale.z * 5f));
                voxelTransform.SetVoxel(selectedVoxels[selectedVoxels.Count - 1].transform);
            }
        }

        public void SelectAll()
        {
            selectedVoxels.Clear();
            GameObject[] go_Voxels = GameObject.FindGameObjectsWithTag("Voxel");

            for (int i = 0; i < go_Voxels.Length; i++)
            {
                selectedVoxels.Add(go_Voxels[i]);
            }

            SetMaterialsVoxels(true);
        }

        public void DeselectAll()
        {
            SetMaterialsVoxels(false);
            selectedVoxels.Clear();
        }

        public void SetMaterialsVoxelsCopy(bool copy)
        {
            if (copy)
            {
                for (int i = 0; i < createInstanceVoxel.voxelsCopy.Count; i++)
                {
                    createInstanceVoxel.voxelsCopy[i].GetComponent<VoxelMaterials>().Copy();
                }
            }
            else
            {
                for (int i = 0; i < createInstanceVoxel.voxelsCopy.Count; i++)
                {
                    createInstanceVoxel.voxelsCopy[i].GetComponent<VoxelMaterials>().Standard();
                }
            }
        }

        public void SetMaterialVoxel(GameObject voxel, bool select)
        {
            VoxelMaterials voxelMaterials = voxel.transform.GetComponent<VoxelMaterials>();
            if (select)
            {
                voxelMaterials.Select();
                //selectedVoxels[selectedVoxels.Count - 1].GetComponent<VoxelMaterials>().SelectMain();
            }
            else
            {
                voxelMaterials.Standard();
            }

        }

        public void SetMaterialsVoxels(bool select)
        {
            if (selectedVoxels.Count == 0) return;
            if (select)
            {
                for (int i = 0; i < selectedVoxels.Count - 1; i++)
                    selectedVoxels[i].GetComponent<VoxelMaterials>().Select();
                selectedVoxels[selectedVoxels.Count - 1].GetComponent<VoxelMaterials>().SelectMain();
            }
            else
            {
                for (int i = 0; i < selectedVoxels.Count; i++)
                    selectedVoxels[i].GetComponent<VoxelMaterials>().Standard();
            }
        }

        public void FitVertices()
        {
            for (int i = 0; i < selectedVoxels.Count; i++)
            {
                if (selectedVoxels[i].transform.tag != "Voxel")
                    return;
            }
            if (selectedVoxels.Count > 0)
            {
                VoxelMeshGenerator voxelMeshGenerator = selectedVoxels[selectedVoxels.Count - 1].GetComponent<VoxelMeshGenerator>();
                for (int i = 0; i < selectedVoxels.Count - 1; i++)
                {
                    VoxelMeshGenerator voxelMeshGenerator2 = selectedVoxels[i].GetComponent<VoxelMeshGenerator>();
                    bool b = false;
                    for (int j = 0; j < voxelMeshGenerator.points.Length; j++)
                    {
                        for (int l = 0; l < voxelMeshGenerator2.points.Length; l++)
                        {
                            if (voxelMeshGenerator.points[j] + voxelMeshGenerator.transform.position ==
                                voxelMeshGenerator2.points[l] + voxelMeshGenerator2.transform.position)
                            {
                                voxelMeshGenerator2.offset[l] = voxelMeshGenerator.offset[j];
                                b = true;
                            }
                        }
                    }

                    if (b)
                    {
                        voxelMeshGenerator2.UpdateMesh();
                    }
                }
            }
        }

        public void AutoFitVertices(VoxelMeshGenerator voxelMeshGenerator, bool invers)
        {
            List<VoxelMeshGenerator> nearest = new List<VoxelMeshGenerator>();

            RaycastHit[] hits = Physics.SphereCastAll(voxelMeshGenerator.transform.position, Mathf.Sqrt(2f) * 1.5f, voxelMeshGenerator.transform.position);

            foreach (RaycastHit hit in hits)
            {
                if (hit.transform.tag == "Voxel")
                {
                    VoxelMeshGenerator voxelMeshGenerator0 = hit.transform.GetComponent<VoxelMeshGenerator>();

                    for (int i = 0; i < voxelMeshGenerator.offset.Length; i++)
                    {
                        for (int j = 0; j < voxelMeshGenerator0.offset.Length; j++)
                        {
                            if (voxelMeshGenerator0.offset[j] == voxelMeshGenerator.offset[i]) continue;


                            if (voxelMeshGenerator.points[i] + voxelMeshGenerator.transform.position ==
                                voxelMeshGenerator0.points[j] + voxelMeshGenerator0.transform.position)
                            {
                                if (invers)
                                {
                                    voxelMeshGenerator0.offset[j] = voxelMeshGenerator.offset[i];
                                    voxelMeshGenerator0.UpdateMesh();
                                }
                                else
                                {
                                    voxelMeshGenerator.offset[i] = voxelMeshGenerator0.offset[j];
                                    voxelMeshGenerator.Start();
                                    voxelMeshGenerator.UpdateMesh();

                                }

                                //voxelMeshGenerator.UpdateMesh();
                                //voxelMeshGenerator0.UpdateMesh();

                            }
                        }
                    }
                }
            }
        }

        private void ChangedMode()
        {
            mode = mySwitch.mode;

            if (mode == 1)
            {
                cursorPanelsGUI.windowGUI[1].panel.gameObject.SetActive(false);
                meshColliderGrid.enabled = true;

                SetMaterialsVoxels(false);
                selectedVoxels.Clear();


            }
            else
            {
                extractor.SetActive(false);
                cursorPanelsGUI.windowGUI[0].caption[3].gameObject.SetActive(false);
            }

            if (mode == 2)
            {
                cursorPanelsGUI.windowGUI[1].panel.gameObject.SetActive(false);

                meshColliderGrid.enabled = false;

                SetMaterialsVoxels(true);




            }
            else
            {
                outlineVoxel.SetActive(false);


                materialsControl.ColorActive(false);
                /*if (selectedVoxels.Count > 0)
                {
                    if (mySwitchVoxelSettings.mode != 6)
                        selectedVoxel.GetComponent<MeshRenderer>().material = standart;
                }*/

                for (int i = 0; i < selectedVoxels.Count; i++)
                {
                    selectedVoxels[i].GetComponent<VoxelMaterials>().Standard();
                }

                cursorPanelsGUI.windowGUI[0].caption[4].gameObject.SetActive(false);
            }

            if (mode == 3)
            {
                cursorPanelsGUI.windowGUI[1].panel.gameObject.SetActive(false);

                meshColliderGrid.enabled = false;

                SetMaterialsVoxels(false);

                coordinateSystem.SetEnable(false);

            }
            else
            {
                cursorPanelsGUI.windowGUI[0].caption[5].gameObject.SetActive(false);
                highlightVertex.SetActive(false);
                extractorVertex.SetActive(false);


                selectedVertex = -1;

                if (selectorsVertices)
                {
                    for (int i = 0; i < gb_selectorsVertices.Length; i++)
                    {
                        Destroy(gb_selectorsVertices[i]);
                    }

                    selectorsVertices = false;

                }
            }

            if (mode == 4)
            {
                coordinateSystem.SetEnable(false);
                coordinateSystem.Additions(true);

                cursorPanelsGUI.CaptionActive(0, 8, true);
            }
            else
            {
                //selectedImported.Clear();
                coordinateSystem.Additions(false);
                selectedImported = null;

                cursorPanelsGUI.CaptionActive(0, 8, false);
            }
        }

        public void BlockSwitch()
        {
            for (int i = 0; i < mySwitchVoxelSettings.active.Length - 1; i++)
            {
                mySwitchVoxelSettings.active[i] = mySwitchVoxelSettings.mode - 1 == i;
            }
        }

        public float yVA;
        [SerializeField]
        private bool voxelAreaXZ;
        private void Mode1()
        {
            if (voxelTransform.virtualVoxels.myFlagVirtual.active)
            {
                if (!coordinateSystem.captureX && !coordinateSystem.captureY && !coordinateSystem.captureZ)
                {
                    //coordinateSystem.SetEnable(true);
                    coordinateSystem.transform.position = voxelTransform.virtualVoxels.GetPosition();
                }

                if (coordinateSystem.onX || coordinateSystem.onY || coordinateSystem.onZ)
                {
                    if (coordinateSystem.captureX || coordinateSystem.captureY || coordinateSystem.captureZ)
                    {
                        voxelTransform.virtualVoxels.TransformArea(coordinateSystem.transform.position);
                    }
                    extractor.SetActive(false);
                    return;
                }

            }
            else
            {
                //coordinateSystem.SetEnable(false);
            }

            cursorPanelsGUI.windowGUI[0].caption[3].gameObject.SetActive(true);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (mySwitchVoxelSettings.mode == 6)
            {
                SetMaterialsVoxelsCopy(true);
            }
            else
            {
                SetMaterialsVoxelsCopy(false);
            }


            if (Physics.Raycast(ray, out hit, 10000))
            {
                ExtractorRot(ref hit);
                ExtractorPos(ref hit);
                extractor.SetActive(true);


                /*
                Quaternion hitRot = Quaternion.FromToRotation(Vector3.up, hit.normal);
                extractor.transform.rotation = hitRot;

                extractor.SetActive(true);

                
                if (hit.transform.gameObject.name == "Grid" && false)
                {
                    extractor.transform.position = ExtractorPos(ref hit);
                }
                else
                {
                    switch (hit.transform.tag)
                    {
                        case "Voxel":
                            extractor.transform.position = hit.transform.position + extractor.transform.up * 0.5f;
                            break;
                        case "Imported":
                            extractor.SetActive(false);
                            break;
                        default:
                            extractor.SetActive(false);
                            return;
                            break;
                    }
                }
                */




                if (MouseButton(1, false) && !voxelAreaXZ)
                {
                    //Instantiate(voxel, extractor.transform.position + new Vector3(0, 0.5f, 0), Quaternion.identity);

                    // Vector3 voxelPos = extractor.transform.position + new Vector3(Mathf.Sin(extractor.transform.eulerAngles.x * Mathf.Deg2Rad), 0.5f, 0);
                    if (mySwitchVoxelSettings.mode != 6)
                    {
                        BlockSwitch();
                    }

                    Vector3 voxelPos = extractor.transform.position + extractor.transform.up * 0.5f;

                    //Устранение погрешности путем округления (0.4999 -> 0.5)
                    voxelPos.x = (int)((voxelPos.x + 0.01f) * 10) * 0.1f;
                    voxelPos.y = (int)((voxelPos.y + 0.01f) * 10) * 0.1f;
                    voxelPos.z = (int)((voxelPos.z + 0.01f) * 10) * 0.1f;

                    if (InGrid(voxelPos))
                    {
                        if (mySwitchVoxelSettings.mode == 6)
                        {
                            //if (offsetCopy != null)
                            if (createInstanceVoxel.voxelCopy != null)
                            {
                                /*GameObject v = Instantiate(offsetCopy, voxelPos, Quaternion.identity);

                                v.GetComponent<VoxelMeshGenerator>().createShape = false;
                                v.GetComponent<MeshRenderer>().material = standart;*/


                                VoxelMaterials voxelMaterials = createInstanceVoxel.voxelCopy.GetComponent<VoxelMaterials>();
                                voxelMaterials.Standard();

                                createInstanceVoxel.PastBuild(voxelPos);

                                voxelMaterials.Copy();
                            }
                        }
                        else
                        {
                            // Instantiate(voxel, voxelPos, Quaternion.identity);

                            if (autoFit.active)
                            {

                                VoxelMeshGenerator voxelMeshGenerator = createInstanceVoxel.Create(voxelPos);

                                AutoFitVertices(voxelMeshGenerator, false);

                                voxelMeshGenerator.GetComponent<VoxelMaterials>().SetStandard(materialsControl.GetMaterialByCurIndex());
                            }
                            else
                            {
                                createInstanceVoxel.Create(voxelPos);
                            }
                        }
                    }
                }

                if(Input.GetKey(KeyCode.LeftShift))
                    voxelArea.gameObject.SetActive(false);


                if (MouseButton(0, true))
                {
                    if (!voxelAreaXZ)
                    {
                        yVA = 0;

                        if (mySwitchVoxelSettings.mode != 6)
                        {
                            BlockSwitch();
                        }

                        VoxelArea(voxelArea);
                    }
                }
                else
                {
                    if (voxelArea.gameObject.activeSelf)
                    {
                        voxelAreaXZ = true;
                    }
                    else
                        voxelAreaXZ = false;
                }

            }
            else
            {
                extractor.SetActive(false);

                highlightVertex.SetActive(false);
                if (selectedVertex == -1)

                    extractorVertex.SetActive(false);

            }

            //вверх...
            if (voxelAreaXZ)
            {
                if (voxelArea.gameObject.activeSelf)
                {

                    yVA += Input.GetAxis("Mouse Y") * Time.deltaTime * 50;

                    if (yVA < 1) yVA = 1;
                    voxelArea.position = new Vector3(voxelArea.position.x, voxelAreaStartPos.y + (int)yVA * 0.5f - 0.5f, voxelArea.position.z);

                    voxelArea.localScale = new Vector3(voxelArea.localScale.x, (int)yVA, voxelArea.localScale.z);

                    bool bMouse = Input.GetMouseButtonDown(0);
                    if(Input.GetMouseButtonDown(1))
                    {
                        bMouse = true;
                        voxelArea.localScale = Vector3.zero;
                    }

                    if (bMouse)
                    {
                        if (voxelArea.localScale.x * 0.5f * voxelArea.localScale.y * 0.5f * voxelArea.localScale.z * 0.5f <= 1000)
                        {
                            for (float x = (float)(-voxelArea.localScale.x * 0.5f); x < (float)(voxelArea.localScale.x * 0.5f); x++)
                            {
                                for (float y = (float)(-voxelArea.localScale.y * 0.5f); y < (float)(voxelArea.localScale.y * 0.5f); y++)
                                {
                                    for (float z = (float)(-voxelArea.localScale.z * 0.5f); z < (float)(voxelArea.localScale.z * 0.5f); z++)
                                    {
                                        Vector3 pos = new Vector3(x, y, z) + voxelArea.position + new Vector3(0.5f, 0.5f, 0.5f);
                                        Collider[] collider = Physics.OverlapSphere(pos, 0.05f);

                                        bool b = false;

                                        if (InGrid(pos))
                                        {
                                            b = true;
                                            foreach (Collider col in collider)
                                            {
                                                if (col.CompareTag("Voxel"))
                                                {
                                                    b = false; break;
                                                }
                                            }
                                        }

                                        if (b)
                                        {
                                            VoxelMeshGenerator voxelMeshGenerator = createInstanceVoxel.Create(pos);
                                            AutoFitVertices(voxelMeshGenerator, false);
                                            voxelMeshGenerator.GetComponent<VoxelMaterials>().SetStandard(materialsControl.GetMaterialByCurIndex());
                                        }
                                    }
                                }
                            }
                            voxelArea.gameObject.SetActive(false);
                        }
                        else
                        {
                            voxelArea.gameObject.SetActive(false);
                        }
                    }

                }
            }


        }

        private float ProjectionVector(Vector3 a, Vector3 b)
        {
            float p = (a.x * b.x + a.y * b.y + a.z * b.z) / Mathf.Sqrt(b.x * b.x + b.y * b.y + b.z * b.z);

            return p;
        }

        private Transform csPos; //точка-центр при выделении нескольких вокселей
        //[HideInInspector]
        public List<Vector3> saveCapturePos = new List<Vector3>();
        private void Mode2()
        {
            void AddSelectedVoxel(GameObject voxel)
            {
                Transform parent = voxel.transform.parent;
                if (parent == null)
                {
                    selectedVoxels.Add(voxel);
                    saveCapturePos.Add(voxel.transform.position);
                    SetMaterialVoxel(voxel, true);
                }
                else
                {
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        selectedVoxels.Add(parent.GetChild(i).gameObject);
                        saveCapturePos.Add(parent.GetChild(i).position);
                        SetMaterialVoxel(voxel, true);
                    }
                }

                SetMaterialsVoxels(true);
            }
            void RemoveSelectedVoxel(GameObject voxel)
            {
                Transform parent = voxel.transform.parent;
                if (parent == null)
                {
                    selectedVoxels.Remove(voxel);
                    saveCapturePos.Remove(voxel.transform.position);
                    SetMaterialVoxel(voxel, false);
                }
                else
                {
                    for(int i = 0; i < parent.childCount; i++)
                    {
                        selectedVoxels.Remove(parent.GetChild(i).gameObject);
                        saveCapturePos.Remove(parent.GetChild(i).position);
                        SetMaterialVoxel(voxel, false);
                    }
                }
            }
            void ClearSelectedVoxels()
            {
                selectedVoxels.Clear();
                saveCapturePos.Clear();
                SetMaterialsVoxels(false);
            }

            cursorPanelsGUI.windowGUI[0].caption[4].gameObject.SetActive(true);


            RaycastHit hit = new RaycastHit();
            ScreenRaycast(ref hit);




            //Delegate

            if (MouseButton(0, true))
            {
                if (coordinateSystem.NotCapture())
                {
                    if (!voxelAreaXZ)
                    {
                        meshColliderGrid.enabled = true;
                        ExtractorRot(ref hit);
                        if (ExtractorPos(ref hit))
                        {
                            //VoxelAreaCalculate(voxelSelectArea, hit.transform);
                            VoxelArea(voxelSelectArea);
                            voxelSelectArea.localScale += new Vector3(1f, 1f, 1f) * 0.1f;


                            if (!Input.GetKey(KeyCode.LeftShift))
                            {
                                SetMaterialsVoxels(false);
                                selectedVoxels.Clear();
                                saveCapturePos.Clear();
                            }
                        }
                    }
                }
            }
            else
            {
                meshColliderGrid.enabled = false;

                if(voxelSelectArea.gameObject.activeSelf)
                {
                    voxelAreaXZ = true;
                }
                else
                {
                    voxelAreaXZ = false;
                    yVA = 0;
                }
            }

            if (voxelAreaXZ)
            {
                if (voxelSelectArea.gameObject.activeSelf)
                {
                    yVA += Input.GetAxis("Mouse Y") * Time.deltaTime * 50;

                    if (yVA < 1) yVA = 1;
                    voxelSelectArea.position = new Vector3(voxelSelectArea.position.x, voxelAreaStartPos.y + (int)yVA * 0.5f - 0.5f, voxelSelectArea.position.z);

                    voxelSelectArea.localScale = new Vector3(voxelSelectArea.localScale.x, (int)yVA, voxelSelectArea.localScale.z);

                    if (Input.GetMouseButtonDown(0))
                    {
                        Collider[] collider = Physics.OverlapBox(voxelSelectArea.position, (voxelSelectArea.localScale - new Vector3(1f, 1f, 1f) * 0.1f) * 0.49f);

                        foreach (Collider col in collider)
                        {
                            if (col.tag == "Voxel")
                            {
                                AddSelectedVoxel(col.gameObject);

                                SetMaterialsVoxels(true);
                            }
                        }
                        voxelSelectArea.gameObject.SetActive(false);
                    }
                }
            }


            //выделение вокселя
            if(MouseButton(1, false))
            {
                screenSelector.Run();
            }
            if (MouseButtonUp(1))
            {
                screenSelector.Stop();
                if (coordinateSystem.onX == coordinateSystem.onY == coordinateSystem.onZ == false)
                {
                    // if (Physics.Raycast(ray, out hit, 10000))
                    if (hit.collider != null)
                    {
                        if (hit.transform.tag == "Voxel")
                        {
                            if (Input.GetKey(KeyCode.LeftShift))
                            {
                                bool b = true;
                                for (int i = 0; i < selectedVoxels.Count; i++)
                                {
                                    if (selectedVoxels[i] == hit.transform.gameObject)
                                    {
                                        selectedVoxels[i].GetComponent<VoxelMaterials>().Standard();
                                        if (i == selectedVoxels.Count - 1)
                                        {
                                            if (selectedVoxels.Count - 2 >= 0)
                                            {
                                                selectedVoxels[i - 1].GetComponent<VoxelMaterials>().SelectMain();
                                            }
                                        }
                                        //selectedVoxels.Remove(selectedVoxels[i]);
                                        //saveCapturePos.Remove(saveCapturePos[i]);
                                        RemoveSelectedVoxel(selectedVoxels[i]);
                                        b = false;
                                        break;
                                    }
                                }
                                if (b)
                                {
                                    AddSelectedVoxel(hit.transform.gameObject);
                                    selectedVoxels[selectedVoxels.Count - 1].GetComponent<VoxelMaterials>().SelectMain();


                                    if (selectedVoxels.Count - 2 >= 0)
                                    {
                                        selectedVoxels[selectedVoxels.Count - 2].GetComponent<VoxelMaterials>().Select();
                                    }
                                }
                            }
                            else
                            {
                                for (int i = 0; i < selectedVoxels.Count; i++)
                                {
                                    selectedVoxels[i].GetComponent<VoxelMaterials>().Standard();
                                }

                                //selectedVoxels.Clear();
                                //selectedVoxels.Add(hit.transform.gameObject);

                                ClearSelectedVoxels();
                                AddSelectedVoxel(hit.transform.gameObject);

                                selectedVoxels[0].GetComponent<VoxelMaterials>().SelectMain();

                                // saveCapturePos.Clear();
                                //saveCapturePos.Add(hit.transform.position);



                                if (materialsControl.colorActive)
                                {
                                    SetMaterialsVoxels(false);
                                    materialsControl.myColor.GetMaterial();
                                }
                            }
                        }
                    }
                }
            }

            //выделение всех вокселей / снятие выделения
            if (Input.GetKey(KeyCode.LeftControl))
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (selectedVoxels.Count != 0)
                        DeselectAll();
                    else
                        SelectAll();
                }
            }

            //удаление вокселя(ей)
            if (selectedVoxels.Count > 0)
            {
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    for (int i = 0; i < selectedVoxels.Count; i++)
                    {
                        voxels.Remove(selectedVoxels[i].GetComponent<VoxelMeshGenerator>());
                        Destroy(selectedVoxels[i].gameObject);
                    }
                    selectedVoxels.Clear();
                }

                if (Input.GetKey(KeyCode.LeftControl))
                {
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        createInstanceVoxel.SetVoxelsCopy(selectedVoxels);
                        //createInstanceVoxel.voxelCopy = selectedVoxels[selectedVoxels.Count - 1];
                    }
                    if (Input.GetKeyDown(KeyCode.V))
                    {
                        SetMaterialsVoxels(false);
                        selectedVoxels = createInstanceVoxel.Past();
                        SetMaterialsVoxels(true);
                    }
                }
            }
            else
            {
                coordinateSystem.SetEnable(false);
            }


            if (selectedVoxels.Count > 0)
            {
                csPos.position = Vector3.zero;

                for (int i = 0; i < selectedVoxels.Count; i++)
                {
                    csPos.position += selectedVoxels[i].transform.position;
                }
                csPos.position /= selectedVoxels.Count;

                coordinateSystem.SetEnable(true);
                coordinateSystem.SetTarget(csPos);
                coordinateSystem.SetStep(csPos.position);

               if(selectedVoxels.Count != saveCapturePos.Count)
                {
                    saveCapturePos.Clear();
                    for(int i=0; i < selectedVoxels.Count; i++)
                    {
                        saveCapturePos.Add(selectedVoxels[i].transform.position);
                    }
                }

                for (int i = 0; i < selectedVoxels.Count; i++)
                {
                    voxelTransform.SetVoxel(selectedVoxels[i].transform);
                    voxelTransform.SetPosition(saveCapturePos[i] + coordinateSystem.captureOffset);
                    voxelTransform.UpdateTransform();

                    if (!Input.GetMouseButton(0))
                        saveCapturePos[i] = voxelTransform.GetPosition();
                }



                coordinateSystem.SetStep(csPos.position);
            }
        }

        public void Group()
        {
            if (selectedVoxels.Count > 1)
            {
                GameObject part = new GameObject();
                part.transform.name = "Part";

                for (int i = 0; i < selectedVoxels.Count; i++)
                {
                    Transform part0 = selectedVoxels[i].transform.parent;
                    if (part0 != null)
                    {
                        selectedVoxels[i].transform.SetParent(null);
                        if (part0.childCount == 0) Destroy(part0.gameObject);
                    }
                    selectedVoxels[i].transform.SetParent(part.transform);
                }
            }

            messageSystem.AddMessage("The group successfully");
        }

        public void Unroup()
        {

            for(int i = 0; i < selectedVoxels.Count; i++)
            {
                Transform part = selectedVoxels[i].transform.parent;
                if (part != null)
                {
                    for(; part.childCount != 0;)
                    {
                        part.GetChild(0).SetParent(null);
                        i++;
                    }
                    Destroy(part.gameObject);
                }
                //break;
            }

            messageSystem.AddMessage("The ungroup successfully");

            return;


            int index = 0;
            for (index = 0; index < selectedVoxels.Count; index++)
            {
                Transform parent = selectedVoxels[index].transform.parent;
                if (parent != null)
                {
                    for (int i = 0; i < parent.childCount; i++)
                    {
                        selectedVoxels[index].transform.SetParent(null);
                        index++;
                    }
                    Destroy(parent.gameObject);
                    return;
                }
            }
        }

        public void ScreenRaycast(ref RaycastHit hit)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out hit, 10000);
        }

        private void CopyToClipboard(string copyValue)
        {
            TextEditor text = new TextEditor();
            text.text = copyValue;
            text.SelectAll();
            text.Copy();
        }

        private void Mode3()
        {
            cursorPanelsGUI.windowGUI[0].caption[5].gameObject.SetActive(true);


            if (selectedVoxels.Count > 0)
            {
                if (selectedVoxels[selectedVoxels.Count - 1].transform.tag != "Voxel")
                    return;
                cursorPanelsGUI.windowGUI[0].caption[5].gameObject.SetActive(true);


                VoxelMeshGenerator voxelMeshGenerator = selectedVoxels[selectedVoxels.Count - 1].transform.GetComponent<VoxelMeshGenerator>();


                //скопировать координаты вокселя
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    //UnityEditor.EditorGUIUtility.systemCopyBuffer = "Hello World";
                    if (Input.GetKeyDown(KeyCode.C))
                    {
                        string CopyVertex(Vector3 vertex, bool semicolon)
                        {
                            string str = semicolon ? ";" : "";
                            int mul = 0;
                            for (int i = 0; i < mySwitchVoxelSettings.options.Length; i++)
                            {
                                if (mySwitchVoxelSettings.active[i])
                                {
                                    mul = int.Parse(mySwitchVoxelSettings.options[i].transform.GetChild(0).GetComponent<Text>().text);
                                    break;
                                }
                            }
                            return vertex.x * mul + "," + vertex.y * mul + "," + -vertex.z * mul + str;
                        }


                        string copyValue = "";
                        copyValue += CopyVertex(voxelMeshGenerator.offset[3], true);
                        copyValue += CopyVertex(voxelMeshGenerator.offset[4], true);
                        copyValue += CopyVertex(voxelMeshGenerator.offset[2], true);
                        copyValue += CopyVertex(voxelMeshGenerator.offset[5], true);
                        copyValue += CopyVertex(voxelMeshGenerator.offset[0], true);
                        copyValue += CopyVertex(voxelMeshGenerator.offset[7], true);
                        copyValue += CopyVertex(voxelMeshGenerator.offset[1], true);
                        copyValue += CopyVertex(voxelMeshGenerator.offset[6], false);

                        CopyToClipboard(copyValue);

                        messageSystem.AddMessage("Copy vertices");
                    }
                }

                if (autoFit.active)
                {
                    AutoFitVertices(voxelMeshGenerator, true);
                }


                highlightVertex.SetActive(false);

                if (!selectorsVertices)
                {
                    gb_selectorsVertices = new GameObject[voxelMeshGenerator.points.Length];

                    for (int i = 0; i < voxelMeshGenerator.points.Length; i++)
                    {
                        gb_selectorsVertices[i] = Instantiate(outlineVertex, voxelMeshGenerator.points[i] +
                            voxelMeshGenerator.transform.position + voxelMeshGenerator.offset[i], Quaternion.identity);
                    }

                    selectorsVertices = true;
                }

                for (int i = 0; i < voxelMeshGenerator.points.Length; i++)
                {
                    gb_selectorsVertices[i].transform.position = voxelMeshGenerator.transform.position + voxelMeshGenerator.points[i] + voxelMeshGenerator.offset[i];
                }

                int index = -1;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10000))
                {
                    highlightVertex.SetActive(false);

                    // Vector3 vertexPos = voxelMeshGenerator.transform.position + voxelMeshGenerator.points[i] + voxelMeshGenerator.offset[i];

                    for (int i = 0; i < voxelMeshGenerator.points.Length; i++)
                    {

                        //if (Vector3.Distance(hit.point, voxelMeshGenerator.points[i] + voxelMeshGenerator.transform.position + voxelMeshGenerator.offset[i]) <= 0.1f)

                        //gb_selectorsVertices[i].transform.position = vertexPos;
                        Vector3 vertexPos = voxelMeshGenerator.transform.position + voxelMeshGenerator.points[i] + voxelMeshGenerator.offset[i];


                        //if (hit.transform.position == vertexPos)
                        if (hit.transform == gb_selectorsVertices[i].transform)
                        {
                            highlightVertex.transform.position = vertexPos;
                            highlightVertex.SetActive(true);

                            index = i;
                            break;
                        }
                    }
                }

                if (MouseButton(0, false))
                {
                    if (index != -1)
                    {
                        selectedVertex = index;
                        
                        for (int i = 0; i < gb_selectorsVertices.Length; i++)
                            gb_selectorsVertices[i].SetActive(true);
                        gb_selectorsVertices[index].SetActive(false);
                        


                        extractorVertex.transform.position = voxelMeshGenerator.points[index] + voxelMeshGenerator.transform.position + voxelMeshGenerator.offset[index];
                        extractorVertex.SetActive(true);

                        sliderX.SetValue(voxelMeshGenerator.offset[index].x);
                        sliderY.SetValue(voxelMeshGenerator.offset[index].y);
                        sliderZ.SetValue(-voxelMeshGenerator.offset[index].z);

                        coordinateSystem.SetTarget(extractorVertex.transform);
                        coordinateSystem.SetStep(new Vector3(sliderX.value, sliderY.value, -sliderZ.value));


                        selectedVertex = index;

                        coordinateSystem.SetTarget(null);
                    }
                    else
                    {
                        //selectedVertex = -1;
                        //extractorVertex.SetActive(false);
                    }
                }

                if (Input.GetMouseButton(0) && false)
                {
                    if (selectedVertex != -1)
                    {
                        Vector3 offsetV = transform.TransformDirection(mouseDir);
                        voxelMeshGenerator.offset[selectedVertex] += new Vector3(offsetV.x, offsetV.y, offsetV.z) * 0.001f *
                            Vector3.Distance(voxelMeshGenerator.points[selectedVertex] + voxelMeshGenerator.transform.position + voxelMeshGenerator.offset[selectedVertex], transform.position);
                        extractorVertex.transform.position = voxelMeshGenerator.points[selectedVertex] + voxelMeshGenerator.transform.position + voxelMeshGenerator.offset[selectedVertex];

                        voxelMeshGenerator.UpdateMesh();
                    }
                }

                if (selectedVertex != -1)
                {
                    voxelMeshGenerator.offset[selectedVertex].x = sliderX.stepValue;
                    voxelMeshGenerator.offset[selectedVertex].y = sliderY.stepValue;
                    voxelMeshGenerator.offset[selectedVertex].z = -sliderZ.stepValue;

                    voxelMeshGenerator.UpdateMesh();

                    extractorVertex.transform.position = voxelMeshGenerator.points[selectedVertex] + voxelMeshGenerator.transform.position + voxelMeshGenerator.offset[selectedVertex];
                    extractorVertex.SetActive(true);


                    
                    coordinateSystem.SetEnable(true);
                    coordinateSystem.SetTarget(extractorVertex.transform);


                    /*sliderX.stepValue += coordinateSystem.captureSpeed.x;
                    sliderY.stepValue += coordinateSystem.captureSpeed.y;
                    sliderZ.stepValue += coordinateSystem.captureSpeed.z;*/


                    //sliderX.AddValue(coordinateSystem.captureSpeed.x);
                    //sliderY.AddValue(coordinateSystem.captureSpeed.y);
                    //sliderZ.AddValue(coordinateSystem.captureSpeed.z);

                    if (coordinateSystem.captureX)
                        sliderX.SetValue(coordinateSystem.captureStep.x);
                    if (coordinateSystem.captureY)
                        sliderY.SetValue(coordinateSystem.captureStep.y);
                    if (coordinateSystem.captureZ)
                        sliderZ.SetValue(-coordinateSystem.captureStep.z);

                    coordinateSystem.SetStep(new Vector3(sliderX.value, sliderY.value, -sliderZ.value));


                    //sliderX.SetValue(coordinateSystem.saveTargetPos.x + coordinateSystem)
                }
            }
            else
            {
                selectedVertex = -1;
            }
        }

        private void Mode4()
        {
            if(MouseButton(0, false) || MouseButton(1, false))
            {
                RaycastHit hit = new RaycastHit();
                ScreenRaycast(ref hit);
                if (hit.collider != null)
                {
                    if(hit.transform.tag == "Imported")
                    {
                        /*
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            bool b = true;
                            for (int i = 0; i < selectedImported.Count; i++)
                            {
                                if (selectedImported[i] == hit.transform)
                                {
                                    b = false;
                                    selectedImported.Remove(selectedImported[i]);

                                    //установка системы координат
                                    for (int j = 0; j < selectedImported.Count; j++)
                                    {
                                        csPos.position += selectedImported[j].position;
                                    }
                                    coordinateSystem.SetEnable(true);
                                    coordinateSystem.SetTarget(csPos);
                                    coordinateSystem.SetStep(csPos.position);

                                    break;
                                }
                            }
                            if (b)
                                selectedImported.Add(hit.transform);
                        }
                        else
                        {
                            selectedImported.Clear();
                            selectedImported.Add(hit.transform);
                        }
                        */
                        selectedImported = hit.transform;

                        coordinateSystem.SetEnable(true);
                        coordinateSystem.SetTarget(selectedImported);
                        coordinateSystem.captureStep = selectedImported.position;

                        transformOBJ.SetTarget(selectedImported);
                    }
                }
            }

            if(selectedImported != null)
            {
                if (coordinateSystem.Capture())
                {

                    //selectedImported.position = coordinateSystem.captureStep;
                    transformOBJ.SetPos(coordinateSystem.captureStep);
                    coordinateSystem.captureStep = transformOBJ.GetPos();

                    transformOBJ.SetRot(selectedImported.eulerAngles);

                    transformOBJ.SetScale(coordinateSystem.targetScale);
                }
                else
                {
                    coordinateSystem.captureStep = transformOBJ.GetPos();

                    //coordinateSystem.cap = transformOBJ.GetPos();
                    coordinateSystem.captureScale = transformOBJ.GetScale();
                    coordinateSystem.targetScale = transformOBJ.GetScale();


                }


                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    Destroy(selectedImported.gameObject);
                    coordinateSystem.SetEnable(false);
                    //coordinateSystem.Additions(false);
                }
            }
        }

        private void VoxelArea(Transform voxelArea)
        {
            Vector3 voxelPos = extractor.transform.position + extractor.transform.up * 0.5f;

            //Устранение погрешности путем округления (0.4999 -> 0.5)
            voxelPos.x = (int)((voxelPos.x + 0.01f) * 10) * 0.1f;
            voxelPos.y = (int)((voxelPos.y + 0.01f) * 10) * 0.1f;
            voxelPos.z = (int)((voxelPos.z + 0.01f) * 10) * 0.1f;

            if (!voxelArea.gameObject.activeSelf)
            {
                voxelAreaStartPos = voxelPos;
                voxelArea.gameObject.SetActive(true);
            }

            Vector3 voxelDir = voxelPos - voxelAreaStartPos;

            voxelArea.position = new Vector3(
                (voxelDir.x) * 0.5f + voxelAreaStartPos.x,
                voxelAreaStartPos.y,
                (voxelDir.z) * 0.5f + voxelAreaStartPos.z
                );

            voxelArea.localScale = new Vector3(Mathf.Abs(voxelDir.x) + 1, 1, Mathf.Abs(voxelDir.z) + 1);
        }

        private Vector3 voxelAreaSize;
        private void VoxelAreaCalculate(Transform voxelArea, Transform selected)
        {
            if(!voxelArea.gameObject.activeSelf)
            {
                if(selected.tag == "Voxel")
                {
                    voxelArea.gameObject.SetActive(true);
                    voxelAreaStartPos = selected.position;
                    voxelAreaSize = Vector3.zero;
                }
            }

            float deltaSize = Vector3.Distance(Camera.main.transform.position, voxelAreaStartPos) * 0.5f;


            voxelAreaSize.x += Camera.main.transform.TransformDirection(mouseDir).x * Time.deltaTime * deltaSize;
            voxelAreaSize.y += Camera.main.transform.TransformDirection(mouseDir).y * Time.deltaTime * deltaSize;
            voxelAreaSize.z += Camera.main.transform.TransformDirection(mouseDir).z * Time.deltaTime * deltaSize;

            Vector3 voxelAreaSizeRound = new Vector3(voxelTransform.Round(voxelAreaSize.x), voxelTransform.Round(voxelAreaSize.y), voxelTransform.Round(voxelAreaSize.z));

            voxelArea.position = new Vector3(
               (voxelAreaSizeRound.x) * 0.5f + voxelAreaStartPos.x,
               voxelAreaStartPos.y,
               (voxelAreaSizeRound.z) * 0.5f + voxelAreaStartPos.z
               );

            voxelArea.localScale = voxelAreaSizeRound;

        }

        public GameObject GetSelectedVoxel()
        {
            if (selectedVoxels.Count > 0)
                return selectedVoxels[selectedVoxels.Count - 1];
            return null;
        }

        public int GetSelectedVoxelID()
        {
            for (int i = 0; i < voxels.Count; i++)
            {
                if(voxels[i].gameObject == GetSelectedVoxel())
                {
                    return i;
                }
            }
            return 0;
        }
    }
} 
