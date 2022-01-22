using System;
using UnityEngine;

public class ObjModel : IDisposable
{
    public ReportField<Vector3> Position;
    public ReportField<float> Size;

    private GameObject _obj;

    public ObjModel(GameObject obj, Vector3 position, float size)
    {
        _obj = obj == null ? new GameObject() : obj;
        _obj.transform.position = position;
        _obj.transform.localScale = Vector3.one * size;

        Position.BindAction(SetPosition);
        Size.BindAction(SetSize);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(_obj);
        Position.RemoveAction(SetPosition);
        Size.RemoveAction(SetSize);
    }

    private void SetPosition(Vector3 value) => _obj.transform.position = value;
    private void SetSize(float value) => _obj.transform.localScale = Vector3.one * value;
}