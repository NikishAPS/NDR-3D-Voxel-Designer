using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastDemo : MonoBehaviour
{
    public float length = 1;
    public Vector3 direction;
    public Vector3 point;

    public List<Vector3> points;

    private void Update()
    {
        
    }

    public void Cast()
    {
        points.Clear();

        for (float f = 0; f < length; f += 0.05f)
        {
            direction = transform.forward * f;
            point = transform.position + direction;

            Vector3 pInt = new Vector3(Mathf.Round(point.x), Mathf.Round(point.y), Mathf.Round(point.z));

            bool inStock = false;
            for (int i =0; i < points.Count; i++)
            {
                if(points[i] == pInt)
                {
                    inStock = true;
                    break;
                }
            }

            if(!inStock)
            {
                points.Add(pInt);
            }

        }
    }

    private void OnDrawGizmos()
    {
        //Cast();

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, point);


        Gizmos.color = Color.green;
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawCube(points[i], Vector3.one * 0.1f);
        }
    }
}
