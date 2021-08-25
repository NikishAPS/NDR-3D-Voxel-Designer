using UnityEngine;

public abstract class Axis : MonoBehaviour
{
    [SerializeField] protected Vector3 _direction;
    [SerializeField] protected GameObject _luminaire;
    [SerializeField] protected static Vector3 _startDragPosition;
    [SerializeField] protected static Vector3 _startPointProjection;



    public void SetHighlight(bool value)
    {
        _luminaire.SetActive(value);
    }

    public abstract void OnStartDrag();

    public abstract void OnDrag();

    public abstract void OnEndDrag();



    protected abstract Vector3 GetProjectedPoint();

}
