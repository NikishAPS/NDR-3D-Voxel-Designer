using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Voxelator
{
    public class GridControl : MonoBehaviour
    {

        public InputField width, depth, height;

        //[HideInInspector]
        public Vector3 center;

        public VoxelsControl voxelsControl;
        [HideInInspector]
        public MeshCollider meshCollider;

        public RectTransform ruler;
        private RectTransform line, startPoint, endPoint;
        private Text value;
        public Vector3 point1, point2;
        private bool setPoint1;
        private MeshCollider meshColliderOfRuler;

        private Transform collidersPoint;
       // private SphereCollider[] sphereColliders = new SphereCollider[8];


        void Start()
        {
            center = transform.localScale * 5f;

            meshCollider = GetComponent<MeshCollider>();
            meshColliderOfRuler = gameObject.AddComponent<MeshCollider>();
            meshColliderOfRuler.sharedMesh = meshCollider.sharedMesh;

            line = ruler.Find("Line").GetComponent<RectTransform>();
            startPoint = ruler.Find("StartPoint").GetComponent<RectTransform>();
            endPoint = ruler.Find("EndPoint").GetComponent<RectTransform>();
            value = ruler.Find("Value").GetComponent<Text>();

            collidersPoint = new GameObject().transform;
            collidersPoint.transform.name = "CollidersPointOfRuler";

            for (int i = 0; i < 8; i++)
            {
                SphereCollider sphereCollider = new GameObject().AddComponent<SphereCollider>();
                sphereCollider.radius = 0.1f;

                sphereCollider.transform.name = "Collider" + (i + 1).ToString();
                sphereCollider.transform.SetParent(collidersPoint);

                sphereCollider.transform.SetParent(collidersPoint);
            }


           /* //Bottom
            sphereColliders[0].transform.position = new Vector3(-0.5f, -0.5f, 0.5f);
            sphereColliders[1].transform.position = new Vector3(-0.5f, -0.5f, -0.5f);
            sphereColliders[2].transform.position = new Vector3(0.5f, -0.5f, -0.5f);
            sphereColliders[3].transform.position = new Vector3(0.5f, -0.5f, 0.5f);

            //Top
            sphereColliders[4].transform.position = new Vector3(-0.5f, 0.5f, -0.5f);
            sphereColliders[5].transform.position = new Vector3(-0.5f, 0.5f, 0.5f);
            sphereColliders[6].transform.position = new Vector3(+0.5f, 0.5f, 0.5f);
            sphereColliders[7].transform.position = new Vector3(0.5f, 0.5f, -0.5f);

    */
        }

        void LateUpdate()
        {
            return;
            //функция автоматической установки края линейки на ближайшую вершину вокселя
            void SetAutoPoint(ref Vector3 point, ref RaycastHit hit)
            {
                if (!Input.GetKey(KeyCode.LeftControl))
                {
                    if (hit.transform.tag == "Voxel")
                    {
                        collidersPoint.gameObject.SetActive(true);
                        collidersPoint.position = hit.transform.position;

                        VoxelMeshGenerator voxelMeshGenerator = hit.transform.GetComponent<VoxelMeshGenerator>();

                        for (int i = 0; i < collidersPoint.childCount; i++)
                        {
                            collidersPoint.GetChild(i).transform.localPosition =
                                voxelMeshGenerator.points[i] + voxelMeshGenerator.offset[i];
                        }
                    }
                    if (hit.transform.parent == collidersPoint)
                    {
                        point = hit.transform.position;
                    }
                }
            }

            //расчет линейки
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                meshColliderOfRuler.enabled = true;

                RaycastHit hit = new RaycastHit();
                voxelsControl.ScreenRaycast(ref hit);
                if (hit.collider != null)
                {
                    line.parent.gameObject.SetActive(true);
                    if (!setPoint1)
                    {
                        point1 = hit.point;

                        if (Input.GetMouseButtonDown(0))
                            setPoint1 = true;

                        SetAutoPoint(ref point1, ref hit);
                        point2 = point1;
                    }
                    else
                    if (Input.GetMouseButton(0))
                    {
                        point2 = hit.point;
                        SetAutoPoint(ref point2, ref hit);
                    }
                }
            }
            else
            {
                setPoint1 = false;

                meshColliderOfRuler.enabled = false;

                //активация/деактивация линейки
                line.parent.gameObject.SetActive(Vector3.Distance(point1, point2) > 0.1f);

                //коллайдеры вершин вокселя
                collidersPoint.gameObject.SetActive(line.parent.gameObject.activeSelf);
            }


            //если ленейка активна
            if (line.parent.gameObject.activeSelf)
            {
                startPoint.position = Camera.main.WorldToScreenPoint(point1);
                endPoint.position = Camera.main.WorldToScreenPoint(point2);
                line.position = (startPoint.position + endPoint.position) * 0.5f;
                line.sizeDelta = new Vector2(Vector2.Distance(startPoint.position, endPoint.position), line.sizeDelta.y);

                float alphaX = Vector3.Angle(Vector2.right, endPoint.position - startPoint.position);
                float alphaY = Vector3.Angle(Vector2.up, endPoint.position - startPoint.position);

                float alpha = (alphaX > 0 && alphaY > 90) ?
                    alpha = -Vector3.Angle(Vector2.right, endPoint.position - startPoint.position) :
                    Vector3.Angle(Vector2.right, endPoint.position - startPoint.position);

                line.rotation = Quaternion.Euler(0f, 0f, alpha);

                value.transform.position = line.position + Vector3.up * 20;
                value.text = ((int)(Vector3.Distance(point1, point2) * 100f) * 0.01f).ToString();

                //вычисление значения

            }
        }

        public void SetRulerColliderActive(bool active)
        {
            meshColliderOfRuler.enabled = active;
        }

        public Vector3 GetSize()
        {
            return transform.localScale * 10f;
        }

        public void UpdateSize()
        {
            if (!CorrectInput(width.text))
                width.text = "2";
            if (!CorrectInput(height.text))
                height.text = "2";
            if (!CorrectInput(depth.text))
                depth.text = "2";

            transform.localScale = new Vector3(int.Parse(width.text) * 0.5f, int.Parse(height.text) * 0.5f, int.Parse(depth.text) * 0.5f) * 0.2f;
            transform.position = new Vector3(int.Parse(width.text) * 0.5f - 0.5f, 0, int.Parse(depth.text) * 0.5f - 0.5f);
            gameObject.SetActive(true);


            center = transform.localScale * 5f;

        }

        private bool CorrectInput(string text)
        {
            if (text.Length != 0)
            {
                if (text.Length == 1)
                {
                    if (text[0] == '-' || text[0] == '0' || text[0] == '1')
                    {
                        return false;
                    }
                }
                else
                {
                    if (text[0] == '-') return false;
                }
            }
            else return false;

            return true;
        }

        public bool InGrid(Vector3 point)
        {
            return point.x >= 0f && point.x <= transform.localScale.x * 10f - 1f &&
                point.y >= 0 && point.y <= transform.lossyScale.y * 10f &&
               point.z >= 0f && point.z <= transform.localScale.z * 10f - 1f;
        }
    }
}
