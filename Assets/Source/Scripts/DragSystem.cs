using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragSystem : MonoBehaviour
{
    [SerializeField]
    private bool active;

    [SerializeField]
    private float size = 1;

    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Vector3 dragValue;

    [SerializeField]
    private Vector3 captures;
    private Vector3 capturePos;
    private float captureSize;

    [SerializeField]
    private MeshRenderer axisX, axisY, axisZ;


    private void OnEnable()
    {
        SceneData.eventInput.mouseDown0 += OnMouseDown0;
        SceneData.eventInput.mouse0 += OnMouse0;
        SceneData.eventInput.mouseUp0 += OnMouseUp0;
    }

    private void Start()
    {
        SetActive(false);
    }

    private void OnDisable()
    {
        SceneData.eventInput.mouseDown0 -= OnMouseDown0;
        SceneData.eventInput.mouse0 -= OnMouse0;
        SceneData.eventInput.mouseUp0 -= OnMouseUp0;
    }

    private void Update()
    {
        UpdateTransform();

       

        if (captures != Vector3.zero) return;

        axisX.enabled = axisY.enabled = axisZ.enabled = false;
        Ray ray = SceneData.camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, SceneData.rayLength))
        {
            if (hit.transform.gameObject == axisX.gameObject) axisX.enabled = true;
            else if (hit.transform.gameObject == axisY.gameObject) axisY.enabled = true;
            else if (hit.transform.gameObject == axisZ.gameObject) axisZ.enabled = true;
        }
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
        UpdateTransform();
    }

    public Vector3Int GetDragValue()
    {
        return SceneData.Vector3FloatToInt(dragValue);
    }

    public bool CheckCapture()
    {
        return axisX.enabled || axisY.enabled || axisZ.enabled;
    }

    private void OnMouseDown0()
    {
        if (axisX.enabled) captures.x = 1;
        if (axisY.enabled) captures.y = 1;
        if (axisZ.enabled) captures.z = 1;

        capturePos = SceneData.eventInput.MousePos;
        captureSize = Vector3.Distance(transform.position, SceneData.camera.transform.position) * size * 0.01f;
    }

    private void OnMouse0()
    {
        Vector3 transformDirection = SceneData.camera.transform.TransformDirection((Vector3)SceneData.eventInput.MousePos - capturePos);
        dragValue.x = transformDirection.x * captures.x;
        dragValue.y = transformDirection.y * captures.y;
        dragValue.z = transformDirection.z * captures.z;

        dragValue *= captureSize;
    }

    private void OnMouseUp0()
    {
        axisX.enabled = axisY.enabled = axisZ.enabled = false;
        captures = Vector3.zero;
        dragValue = Vector3.zero;
    }

    private void UpdateTransform()
    {
        transform.position = position + dragValue;

        float distance = Vector3.Distance(SceneData.camera.transform.position, transform.position);
        transform.localScale = Vector3.one * distance * size;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
