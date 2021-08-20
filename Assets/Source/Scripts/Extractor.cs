using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extractor : MonoBehaviour
{
    private Coordinates _coordinates;

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetRotation(Quaternion rot)
    {
        Vector3 euler = rot.eulerAngles;
        //transform.eulerAngles = new Vector3(Mathf.Abs(euler.x), Mathf.Abs(euler.y), Mathf.Abs(euler.z));
        transform.eulerAngles = euler;
    }

    public void SetScale(Vector3 scale)
    {
        transform.localScale = new Vector3().Set(scale);
    }

    public Vector3 GetScale()
    {
        return transform.localScale;
    }


    private void Awake()
    {
        _coordinates = FindObjectOfType<Coordinates>();
    }
}
