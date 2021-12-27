using UnityEngine;

public abstract class Axis : MonoBehaviour
{
    [SerializeField] protected Vector3 _direction;
    [SerializeField] protected GameObject _luminaire;
    [SerializeField] protected static Vector3 _initialDragPoint;
    [SerializeField] protected static Vector3 _initialDragPointProjection;

    public void SetHighlight(bool value)
    {
        _luminaire.SetActive(value);
    }

    public abstract void OnStartDrag(DragTransform initialDragPoint);

    public abstract DragTransform GetDragValue();

    public abstract void OffsetDragPoint(DragTransform dragValue);

    protected abstract Vector3 GetProjectedPoint();

}
