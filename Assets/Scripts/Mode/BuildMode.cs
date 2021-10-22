using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : Mode
{
    private CastResult _castResult;
    private Vector3Int _startVoxelAreaPosition;
    private Vector3Int _endVoxelAreaPosition;
    private bool _multiBuilding = false;
    private Grid _wall;

    /*
     * 0 - non
     * 1 - horizontal
     * 2 - vertical 
     */
    private int _voxelAreaMode;

    public override void OnEnable()
    {

    }

    public override void OnDisable()
    {
        Extractor.Active = false;

    }

    public override void OnMouseMove()
    {

        //RayCast();

        if (IsGridOrVoxel())
        {
            if (_multiBuilding)
            {
                _endVoxelAreaPosition = _castResult.lastPoint;
                Vector3 middleVoxelAreaPosition = (_startVoxelAreaPosition + _endVoxelAreaPosition) / 2;

                Extractor.Position = (_startVoxelAreaPosition + _endVoxelAreaPosition).ToVector3() * 0.5f;
                Extractor.Scale = (_endVoxelAreaPosition - _startVoxelAreaPosition).Abs() + Vector3.one;

                //MoveGrid();
            }
            else
            {
                Extractor.Position = _castResult.lastPoint;
            }
        }
    }

    public override void OnLMouseDown()
    {
        Invoker.Execute(new CreateVoxelsCommand(_startVoxelAreaPosition, _endVoxelAreaPosition));

        _voxelAreaMode = 0;
        ResetExtractor();

        _multiBuilding = false;
    }

    public override void OnRMouseDown()
    {
        if (_castResult == null) return;
        ResetWall();
        _startVoxelAreaPosition = _castResult.lastPoint;
        _multiBuilding = true;
    }

    public override void OnRMouseUp()
    {
        if(_multiBuilding)
        {
            SetWall();
        }
    }

    private void SetWall()
    {
        if (_multiBuilding)
        {
            Grid newWall = GridManager.GetWallGridByDirection(-Camera.main.transform.forward);

            if (newWall != _wall)
            {
                if (_wall != null) _wall.Active = false;
                _wall = newWall;
                _wall.Active = true;

                _wall.SetOffset((int)_endVoxelAreaPosition.Mul(_wall.Normal.ToVector3Int()).magnitude);
            }
        }


    }

    private void ResetWall()
    {
        if (_wall == null) return;

        _wall.Active = false;
        _wall = null;
    }

    private void RayCast()
    {
        _castResult = null;
        //SceneData.Extractor.SetActive(false);

        _castResult = Raycast.CastByMouse(SceneData.RayLength);
        if (_castResult != null)
        {
            //MonoBehaviour.print(_castResult.lastPoint);
            if (ChunkManager.InField(_castResult.lastPoint))
            {
                Extractor.Position = _castResult.lastPoint;
                Extractor.Active = true;
            }
            else
            {
                _castResult = null;
            }
        }
    }

    private void MoveGrid()
    {
        int gridOffset = (int)((Extractor.Position - (Extractor.Scale / 2)).Mul(_wall.Normal) + Vector3.one * 0.5f).magnitude;
        _wall.SetOffset(gridOffset);


        _wall.SetOffset((int)_endVoxelAreaPosition.Mul(_wall.Normal.ToVector3Int()).magnitude);


        return;

        if (_wall == GridManager.Grids[Direction.Left])
            _wall.SetOffset(_startVoxelAreaPosition.x < _endVoxelAreaPosition.x ? _startVoxelAreaPosition.x : _endVoxelAreaPosition.x);
        else if (_wall == GridManager.Grids[Direction.Right])
            _wall.SetOffset(_startVoxelAreaPosition.x < _endVoxelAreaPosition.x ? _startVoxelAreaPosition.x : _endVoxelAreaPosition.x);
        else if (_wall == GridManager.Grids[Direction.Back])
            _wall.SetOffset(_startVoxelAreaPosition.z < _endVoxelAreaPosition.z ? _startVoxelAreaPosition.z : _endVoxelAreaPosition.z);
        else if (_wall == GridManager.Grids[Direction.Forward])
            _wall.SetOffset(_startVoxelAreaPosition.z < _endVoxelAreaPosition.z ? _startVoxelAreaPosition.z : _endVoxelAreaPosition.z);
    }

    private void ResetExtractor()
    {
        Extractor.Position = _endVoxelAreaPosition;
        Extractor.Scale = Vector3.one;
    }

    private bool IsGridOrVoxel()
    {
        _castResult = Raycast.CastByMouse(SceneData.RayLength);
        if (_castResult != null)
        {
            if (ChunkManager.InField(_castResult.lastPoint))
            {
                //Extractor.Position = _castResult.lastPoint;
                Extractor.Active = true;
                return true;
            }
            else
            {
                _castResult = null;
                return false;
            }
        }

        return false;
    }

}
