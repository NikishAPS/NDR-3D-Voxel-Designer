using System;
using UnityEngine;

public class BuildMode : Mode
{
    private Vector3Int _startVoxelAreaPosition;
    private Vector3Int _endVoxelAreaPosition;
    private Vector3Int _fixPosition;
    private MathPlane _plane;
    private BuildStage _buildStage;

    private enum BuildStage
    {
        Aim,    //set the starting point of the building zone
        Plane,  // set the size of the building zone on the plane
        Axis    //set the length of the building zone alone the current axis
    }


    public override void OnEnable()
    {
        CameraController.MoveEvent += OnCameraMove;
        Extractor.Active = true;

        UpdatePlane();
        _buildStage = BuildStage.Aim;
        OnMouseMove();
    }

    public override void OnDisable()
    {
        CameraController.MoveEvent -= OnCameraMove;

        Extractor.Active = false;

    }

    public void OnCameraMove()
    {
        UpdatePlane();
    }

    public override void OnMouseMove()
    {
        CastResult castResult = Cast();

        switch(_buildStage)
        {
            case BuildStage.Aim:
                {
                    if (castResult != null)
                    {
                        _startVoxelAreaPosition = castResult.PreviousVoxelPosition;
                        _endVoxelAreaPosition = _startVoxelAreaPosition;

                        Extractor.Position = _startVoxelAreaPosition;
                    }
                    break;
                }

            case BuildStage.Plane:
                {
                    Vector3Int intersection = _plane.GetIntersection(
                       Camera.main.ScreenToWorldPoint(Input.mousePosition),
                       Camera.main.transform.forward
                       ).RoundToInt();

                    if (ChunkManager.InField(intersection))
                        _endVoxelAreaPosition = intersection;
                    else if (castResult != null)
                        _endVoxelAreaPosition = castResult.PreviousVoxelPosition;

                    _fixPosition = _endVoxelAreaPosition;

                    break;
                }

            case BuildStage.Axis:
                {
                    Vector3 center = (_startVoxelAreaPosition + _fixPosition) / 2;
                    Vector3Int offsetAlongAxis = Direction.GetCursorOffsetAlongAxis(center).RoundToInt();
                    offsetAlongAxis += _fixPosition;

                    if(ChunkManager.InField(offsetAlongAxis))
                        _endVoxelAreaPosition = offsetAlongAxis;

                    break;
                }
        }


        Vector3 middleVoxelAreaPosition = (_startVoxelAreaPosition + _endVoxelAreaPosition) / 2;

        Extractor.Position = (_startVoxelAreaPosition + _endVoxelAreaPosition).ToVector3() * 0.5f;
        Extractor.Scale = (_endVoxelAreaPosition - _startVoxelAreaPosition).Abs() + Vector3.one;
    }

    public override void OnLMouseDown()
    {
        Invoker.Execute(new CreateVoxelsCommand(_startVoxelAreaPosition, _endVoxelAreaPosition));

        Extractor.Position = _endVoxelAreaPosition;
        Extractor.Scale = Vector3.one;
        _startVoxelAreaPosition = _endVoxelAreaPosition = _fixPosition = Vector3Int.zero;

        _buildStage = BuildStage.Aim;
    }

    public override void OnRMouseDown()
    {
        if (Cast() == null) return;

        _buildStage = BuildStage.Plane;
        OnMouseMove();
    }

    public override void OnRMouseUp()
    {
        if (_buildStage != BuildStage.Plane) return;

        _buildStage = BuildStage.Axis;
        OnMouseMove();
    }

    private CastResult Cast()
    {
        return VoxelRaycast.CastByMouse(SceneParameters.RayLength);
    }

    private void UpdatePlane()
    {
        Vector3Int normal = Direction.GetNormalByView();

        //Vector3Int firstPoint = normal * _startVoxelAreaPosition;
        //Vector3Int secondPoint = firstPoint + (Vector3Int.one - normal).Mul(ChunkManager.FieldSize) * 100;

        //BoundsInt bounds = new BoundsInt(firstPoint, secondPoint);
        //_boxCollider.Bounds = bounds;

        Tuple<Vector3Int, Vector3Int> perpendicularDirection = Direction.GetPerpendicularDirections(normal);

        Vector3 point1 = perpendicularDirection.Item1 + _startVoxelAreaPosition;
        Vector3 point2 = perpendicularDirection.Item2 + _startVoxelAreaPosition;
        Vector3 point3 = point1 + point2 - _startVoxelAreaPosition;
        _plane = new MathPlane(point1, point2, point3);

    }

}
