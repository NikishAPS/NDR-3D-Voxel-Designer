using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DragSystem : MonoBehaviour
{
    public delegate void Drag(Vector3 startPos, Vector3 offset);

    public Drag drag;

    [SerializeField]
    private float size = 1;

    [SerializeField]
    private Vector3 position;
    [SerializeField]
    private Vector3 dragValue;

    [SerializeField]
    private Vector3 captures;
    [SerializeField]
    private Vector3 capturePos;
    [SerializeField]
    private float captureSize;

    [SerializeField]
    private MeshRenderer _axisX, _axisY, _axisZ, _axisXY, _axisXZ, _axisYZ;


    private void OnEnable()
    {
        SceneData.eventInput.mouseDown0 += OnMouseDown0;
        SceneData.eventInput.mouse0 += OnMouse0;
        SceneData.eventInput.mouseUp0 += OnMouseUp0;
        SceneData.eventInput.notMouse0 += OnNotMouse0;
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
 
    public Vector3Int GetDragValue()
    {
        return SceneData.Vector3FloatToInt(dragValue);
    }

    public bool CheckCapture()
    {
        return _axisX.enabled || _axisY.enabled || _axisZ.enabled ||
            _axisXY.enabled || _axisXZ.enabled || _axisYZ.enabled;
    }

    private void OnNotMouse0()
    {
        UpdateTransform();

        if (captures != Vector3.zero) return;

        _axisX.enabled = _axisY.enabled = _axisZ.enabled =
            _axisXY.enabled = _axisXZ.enabled = _axisYZ.enabled = false;
        Ray ray = SceneData.camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, SceneData.rayLength))
        {
            if (hit.transform.gameObject == _axisX.gameObject) { _axisX.enabled = true; }
            else if (hit.transform.gameObject == _axisY.gameObject) { _axisY.enabled = true; }
            else if (hit.transform.gameObject == _axisZ.gameObject) { _axisZ.enabled = true; }
            else if (hit.transform.gameObject == _axisXY.gameObject) { _axisXY.enabled = true; }
            else if (hit.transform.gameObject == _axisXZ.gameObject) { _axisXZ.enabled = true; }
            else if (hit.transform.gameObject == _axisYZ.gameObject) { _axisYZ.enabled = true; }
        }
    }

    private void OnMouseDown0()
    {
        if (_axisX.enabled) captures.x = 1;
        if (_axisY.enabled) captures.y = 1;
        if (_axisZ.enabled) captures.z = 1;
        if (_axisXY.enabled) captures.x = captures.y = 1;
        if (_axisXZ.enabled) captures.x = captures.z = 1;
        if (_axisYZ.enabled) captures.y = captures.z = 1;

        //capturePos = SceneData.eventInput.MousePos;
        //capturePos = SceneData.camera.transform.TransformPoint(SceneData.eventInput.MousePos);
        capturePos = SceneData.eventInput.MousePos;
        captureSize = Vector3.Distance(transform.position, SceneData.camera.transform.position) * size * 0.01f;
    }

    private void OnMouse0()
    {
        //Vector3 dragDir = SceneData.camera.transform.TransformPoint(SceneData.eventInput.MousePos) - capturePos;
        Vector3 dragDir = SceneData.camera.transform.TransformDirection(SceneData.eventInput.MousePos - capturePos);
        dragValue.x = dragDir.x * captures.x;
        dragValue.y = dragDir.y * captures.y;
        dragValue.z = dragDir.z * captures.z;

        dragValue *= captureSize;

        UpdateTransform();

        drag?.Invoke(position, dragValue);
    }

    private void OnMouseUp0()
    {
        _axisX.enabled = _axisY.enabled = _axisZ.enabled =
            _axisXY.enabled = _axisXZ.enabled = _axisYZ.enabled = false;
        captures = Vector3.zero;
        dragValue = Vector3.zero;
    }

    private void UpdateTransform()
    {
        transform.position = position + dragValue;

        float distance = Vector3.Distance(SceneData.camera.transform.position, transform.position);
        transform.localScale = Vector3.one * distance * size;
    }

    public void SetPosition(Vector3 pos)
    {
        position = pos;
        UpdateTransform();
    }

    public void ResetDragValue()
    {
        position += dragValue;
        capturePos = SceneData.eventInput.MousePos;
    }

    public void OffsetPosition(Vector3 value)
    {
        dragValue -= value;
        position += value;
        //capturePos = SceneData.camera.transform.TransformPoint(SceneData.eventInput.MousePos) - dragValue / captureSize;
        capturePos = SceneData.eventInput.MousePos - SceneData.camera.transform.InverseTransformDirection(dragValue / captureSize);
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
