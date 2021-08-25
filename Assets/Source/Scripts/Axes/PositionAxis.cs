using UnityEngine;

public class PositionAxis : Axis
{
    public override void OnStartDrag()
    {
        _startDragPosition = Axes.Position;
        _startPointProjection = GetProjectedPoint();
    }

    public override void OnDrag()
    {
        Vector3 dragValue = GetProjectedPoint() - _startPointProjection;
        Axes.Position = _startDragPosition + dragValue;

        Vector3? dragResult = Axes.DragPosition?.Invoke(dragValue);
        if (dragResult != null)
        {
            _startDragPosition += (Vector3)dragResult;
            _startPointProjection += (Vector3)dragResult;
        }
    }

    public override void OnEndDrag()
    {
        Axes.Position = _startDragPosition;
    }



    protected override Vector3 GetProjectedPoint()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3 cameraPoint1 = Camera.main.ScreenToWorldPoint(mousePosition); //getting a point on camera
        Vector3 cameraPoint2 = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane)); //getting a point ahead of the camera
        Vector3 cameraDirection = (cameraPoint2 - cameraPoint1).normalized;

        /*getting a vector lying on the plane of the axis
         * (when we drag some axis, then the resulting vector is perpendicular to this axis */
        Vector3 planeVector = Vector3.Cross(_direction, cameraDirection);
        Vector3 planeNormal = Vector3.Cross(_direction, planeVector); //getting the plane normal

        float t = (-Vector3.Dot(planeNormal, cameraPoint1) + Vector3.Dot(planeNormal, _startDragPosition)) /
            Vector3.Dot(planeNormal, cameraDirection);

        Vector3 result = cameraDirection * t + cameraPoint1;

        return result.Mul(_direction);
    }

}
