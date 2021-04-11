using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorTest : MonoBehaviour
{
    public Transform child;
    public float rotY;
    public Vector3 childPos;
    public Vector3 childCurPos;

    public bool updatePos;

    void Start()
    {
        
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<Voxelator.VoxelsControl>().enabled = !GetComponent<Voxelator.VoxelsControl>().enabled;
        }

        return;
        if(updatePos)
        {
            childPos = child.position - transform.position;
            updatePos = false;
        }

        childCurPos = transform.position + new Vector3(Mathf.Cos(rotY * Mathf.Deg2Rad) * childPos.x, transform.position.y,
            Mathf.Sin(rotY * Mathf.Deg2Rad) * childPos.z);

        childCurPos = transform.position + childPos;

        child.position = childCurPos;

        transform.rotation = Quaternion.Euler(0, rotY, 0);
        child.rotation = transform.rotation;
    }
}
