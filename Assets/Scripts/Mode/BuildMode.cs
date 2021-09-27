using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMode : Mode
{
    private CastResult _castResult;
    private Vector3Int _startVoxelAreaPosition;
    private Vector3Int _endVoxelAreaPosition;

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
        
        RayCast();

        if (_castResult != null)
        {
            Vector3Int endPos = Extractor.Position.ToVector3Int();

            if(_voxelAreaMode == 2)
                _endVoxelAreaPosition.y = endPos.y;
            else
                _endVoxelAreaPosition = endPos;

            if (_voxelAreaMode == 0)
            {
                //VoxelatorManager.Coordinates.Value = SceneData.Extractor.GetPosition();
            }
            else //(_voxelAreaMode != 0)
            {
                Extractor.Position = (_startVoxelAreaPosition + _endVoxelAreaPosition).ToVector3() * 0.5f;
                Extractor.Scale = (_endVoxelAreaPosition - _startVoxelAreaPosition).Abs() + Vector3.one;
                //VoxelatorManager.Coordinates.Value = SceneData.Extractor.GetScale();
            }
        }
    }

    public override void OnLMouseDown()
    {
        if (_voxelAreaMode == 0) _startVoxelAreaPosition = _endVoxelAreaPosition;
        else GridManager.Grids[Direction.Left].Active = false;

        //ChunksManager.CreateVoxels(_startVoxelAreaPosition, _endVoxelAreaPosition);
        Invoker.Execute(new CreateVoxelsCommand(_startVoxelAreaPosition, _endVoxelAreaPosition));

        _voxelAreaMode = 0;
        ResetExtractor();
    }

    public override void OnRMouseDown()
    {
        if (_castResult != null)
        {
            if (_voxelAreaMode == 0)
            {
                //VoxelatorManager.Coordinates.Value = SceneData.Extractor.GetScale();
                _startVoxelAreaPosition = Extractor.Position.ToVector3Int();
                _voxelAreaMode = 1;
            }
        }
    }

    public override void OnRMouseUp()
    {
        if (_voxelAreaMode != 0)
        {
            _voxelAreaMode = 2;

            //VoxelatorManager.Coordinates.Value = SceneData.Extractor.GetScale();

            

            GridManager.Grids[Direction.Left].Set(true,
                new Vector3Int(Mathf.Min(_startVoxelAreaPosition.x, _endVoxelAreaPosition.x), 0, Mathf.Min(_startVoxelAreaPosition.z, _endVoxelAreaPosition.z)),
                new Vector3Int(Mathf.Max(_startVoxelAreaPosition.x, _endVoxelAreaPosition.x), ChunkManager.FieldSize.y, Mathf.Max(_startVoxelAreaPosition.z, _endVoxelAreaPosition.z)));
                //new Vector3Int(_startVoxelAreaPosition.x, ChunksManager.FieldSize.y, _endVoxelAreaPosition.z));


            for (int i = 0; i < Direction.Directions.Length; i++)
                if (i != Direction.Down && i != Direction.Up)
                    return;
            //GridManager.Grids[Direction.Left].Size = new Vector3Int(_endVoxelAreaPosition.z - _startVoxelAreaPosition.z + 1 , 1, ChunksManager.FieldSize.y);
        }
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

    private void ResetExtractor()
    {
        Extractor.Position = _endVoxelAreaPosition;
        Extractor.Scale = Vector3.one;
    }

}
