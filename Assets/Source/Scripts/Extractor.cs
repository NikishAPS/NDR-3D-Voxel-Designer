using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extractor : MonoBehaviour
{
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void SetRotation(Quaternion rot)
    {
        Vector3 euler = rot.eulerAngles;
        //transform.eulerAngles = new Vector3(Mathf.Abs(euler.x), Mathf.Abs(euler.y), Mathf.Abs(euler.z));
        transform.eulerAngles = euler;
    }
}
