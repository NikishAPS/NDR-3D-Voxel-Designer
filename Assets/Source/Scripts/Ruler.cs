using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Voxelator;

public class Ruler : MonoBehaviour
{


    public RectTransform ruler;
    private Image line, startPoint, endPoint;
    private Text value;
    public Vector3 point1, point2;
    private bool setPoint1;

    private Transform collidersPoint;

    private GridControl gridControl;
    private VoxelsControl voxelsControl;

    
    private Image colorControl;
    // private SphereCollider[] sphereColliders = new SphereCollider[8];


    void Start()
    {
        gridControl = FindObjectOfType<GridControl>();
        voxelsControl = FindObjectOfType<VoxelsControl>();
        colorControl = GetComponent<Image>();

        line = ruler.Find("Line").GetComponent<Image>();
        startPoint = ruler.Find("StartPoint").GetComponent<Image>();
        endPoint = ruler.Find("EndPoint").GetComponent<Image>();
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
            gridControl.SetRulerColliderActive(true);

            RaycastHit hit = new RaycastHit();
            voxelsControl.ScreenRaycast(ref hit);
            if (hit.collider != null)
            {
                line.transform.parent.gameObject.SetActive(true);
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

            gridControl.SetRulerColliderActive(false);

            //активация/деактивация линейки
            line.transform.parent.gameObject.SetActive(Vector3.Distance(point1, point2) > 0.1f);

            //коллайдеры вершин вокселя
            collidersPoint.gameObject.SetActive(line.transform.parent.gameObject.activeSelf);
        }


        //если ленейка активна
        if (line.transform.parent.gameObject.activeSelf)
        {
            startPoint.rectTransform.position = Camera.main.WorldToScreenPoint(point1);
            endPoint.rectTransform.position = Camera.main.WorldToScreenPoint(point2);
            line.rectTransform.position = (startPoint.rectTransform.position + endPoint.rectTransform.position) * 0.5f;
            line.rectTransform.sizeDelta = new Vector2(Vector2.Distance(startPoint.rectTransform.position, endPoint.rectTransform.position), line.rectTransform.sizeDelta.y);

            float alphaX = Vector3.Angle(Vector2.right, endPoint.rectTransform.position - startPoint.rectTransform.position);
            float alphaY = Vector3.Angle(Vector2.up, endPoint.rectTransform.position - startPoint.rectTransform.position);

            float alpha = (alphaX > 0 && alphaY > 90) ?
                alpha = -Vector3.Angle(Vector2.right, endPoint.rectTransform.position - startPoint.rectTransform.position) :
                Vector3.Angle(Vector2.right, endPoint.rectTransform.position - startPoint.rectTransform.position);

            line.rectTransform.rotation = Quaternion.Euler(0f, 0f, alpha);

            value.transform.position = line.rectTransform.position + Vector3.up * 20;
            value.text = ((int)(Vector3.Distance(point1, point2) * 100f) * 0.01f).ToString();

            //вычисление значения

        }

        line.color = startPoint.color = endPoint.color = colorControl.color;
    }
}
