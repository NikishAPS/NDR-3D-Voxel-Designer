using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer _meshRenderer;

    private float _yOffset = -0.51f;


    public bool Active
    {
        get => gameObject.activeSelf;
        set => gameObject.SetActive(value);
    }

    public Vector3Int Position
    {
        get => transform.localPosition.ToVector3Int();
        set => transform.localPosition = value;
    }

    public Vector3Int Size
    {
        get => transform.localScale.ToVector3Int();
        set
        {
            _meshRenderer.material.SetTextureScale("_MainTex", new Vector3(value.x, value.z));
            transform.localScale = new Vector3(value.x, 1, value.z);

            float meshPositionX = (value.x * 0.5f - 0.5f) / value.x;
            float meshPositionz = (value.z * 0.5f - 0.5f) / value.z;
            _meshRenderer.transform.localPosition = new Vector3(meshPositionX, _yOffset, meshPositionz);
        }
    }

    public bool IsGrid(Vector3Int position)
    {
        return VoxelatorManager.WithinTheArray(Size, GetLocalPosition(position));
    }

    public void Set(bool active, Vector3Int startPoint, Vector3Int endPoint)
    {
        Active = active;

        Vector3Int localStartPoint = GetLocalPosition(startPoint);
        Vector3Int localEndPoint = GetLocalPosition(endPoint);
        int sign = (int)Mathf.Sign(localEndPoint.x - localStartPoint.x);

        Position = sign > 0 ? startPoint : new Vector3Int(endPoint.x, startPoint.y, endPoint.z);

        localEndPoint.x += sign;

        Size = (localEndPoint - localStartPoint).Abs();
    }



    private Vector3Int GetLocalPosition(Vector3Int point)
    {
        return transform.InverseTransformPoint(point).Mul(Size).ToVector3Int();
    }

}


