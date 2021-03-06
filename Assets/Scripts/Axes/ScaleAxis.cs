using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAxis : Axis
{
    public override void OnStartDrag(DragTransform initialDragPoint)
    {
        _initialDragPoint = GetProjectedPoint();
        _initialDragPointProjection = _initialDragPoint;
    }

    public override DragTransform GetDragValue()
    {
        return new DragTransform(Vector3.zero, GetProjectedPoint() - _initialDragPointProjection);
    }

    public override void OffsetDragPoint(DragTransform dragValue)
    {
        _initialDragPoint += dragValue.Scale;
        _initialDragPointProjection += dragValue.Scale;
    }

    protected override Vector3 GetProjectedPoint()
    {
        Vector3 mousePosition = Input.mousePosition;

        Vector3 cameraPoint1 = Camera.main.ScreenToWorldPoint(mousePosition); //getting a point on camera
        Vector3 cameraPoint2 = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 1));
        Vector3 cameraDirection = (cameraPoint2 - cameraPoint1).normalized;
        Vector3 cameraRightDirection = Camera.main.transform.right;
        Vector3 cameraUpDirection = Camera.main.transform.up;

        Vector3 planeNormal = Vector3.Cross(cameraRightDirection, cameraUpDirection);

        float t = (-Vector3.Dot(planeNormal, cameraPoint1) + Vector3.Dot(planeNormal, _initialDragPoint))
            / Vector3.Dot(planeNormal, cameraDirection);

        Vector3 result = cameraDirection * t + cameraPoint1;

        return result.Mul(_direction);
    }


}
