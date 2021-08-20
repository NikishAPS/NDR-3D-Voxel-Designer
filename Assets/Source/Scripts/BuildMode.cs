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

    public override void Tick()
    {
        //return;
        //SceneData.Extractor.SetActive(false);

        //if (!SceneData.ControlGUI.IsPanel)
        //{
        //    //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    //RaycastHit hit;

        //    CastResult castResult = VoxelRayCast.CastByMouse(SceneData.RayLength);
        //    if (castResult != null)
        //    {

        //        if (SceneData.Chunk.InChunk(SceneData.Vector3FloatToInt(castResult.lastPoint)))
        //        {
        //            SceneData.Extractor.SetActive(true);
        //            SceneData.Extractor.transform.position = castResult.lastPoint;

        //            if (SceneData.EventInput.GetMouseDown0)
        //            {
        //                int id = SceneData.ColorTest.id;
        //                ChunksManager.CreateVoxel(id, castResult.lastPoint);
        //                return;
        //                SceneData.Chunk.CreateVoxel(SceneData.ColorTest.id, castResult.lastPoint);
        //            }
        //        }
        //    }
        //}
    }

    public override void Enable()
    {
        InputEvent.MouseMove += MouseMove;
        InputEvent.LMouseDown += LMouseDown;
        InputEvent.RMouseDown += RMouseDown;
        InputEvent.RMouseUp += RMouseUp;
    }

    public override void Disable()
    {
        SceneData.Extractor.SetActive(false);

        InputEvent.MouseMove -= MouseMove;
        InputEvent.LMouseDown -= LMouseDown;
        InputEvent.RMouseDown -= RMouseDown;
        InputEvent.RMouseUp -= RMouseUp;
    }

    public void MouseMove()
    {
        RayCast();


        if (_castResult != null)
        {
            Vector3Int endPos = SceneData.Extractor.GetPosition().ToVector3Int();

            if(_voxelAreaMode == 2)
                _endVoxelAreaPosition.y = endPos.y;
            else
                _endVoxelAreaPosition = endPos;

            if (_voxelAreaMode == 0)
            {
                VoxelatorManager.Coordinates.Value = SceneData.Extractor.GetPosition();
            }
            else //(_voxelAreaMode != 0)
            {
                SceneData.Extractor.SetPosition((_startVoxelAreaPosition + _endVoxelAreaPosition).ToVector3() * 0.5f);
                SceneData.Extractor.SetScale((_endVoxelAreaPosition - _startVoxelAreaPosition).Abs() + Vector3.one);
                VoxelatorManager.Coordinates.Value = SceneData.Extractor.GetScale();
            }
        }
    }

    public void LMouseDown()
    {
        if (!SceneData.ControlGUI.IsPanel)
        {
            if (_voxelAreaMode == 0) _startVoxelAreaPosition = _endVoxelAreaPosition;
            else GridManager.Grids[Direction.Left].Active = false;

            //ChunksManager.CreateVoxels(_startVoxelAreaPosition, _endVoxelAreaPosition);
            Commands.CreateVoxelsCommand.SetAndExe(_startVoxelAreaPosition, _endVoxelAreaPosition);

            _voxelAreaMode = 0;
            ResetExtractor();
        }
    }

    public void RMouseDown()
    {
        if (_castResult != null)
        {
            if (_voxelAreaMode == 0)
            {
                VoxelatorManager.Coordinates.Value = SceneData.Extractor.GetScale();
                _startVoxelAreaPosition = SceneData.Extractor.GetPosition().ToVector3Int();
                _voxelAreaMode = 1;
            }
        }
    }

    public void RMouseUp()
    {
        if (_voxelAreaMode != 0)
        {
            _voxelAreaMode = 2;

            VoxelatorManager.Coordinates.Value = SceneData.Extractor.GetScale();

            GridManager.Grids[Direction.Left].Set(true,
                new Vector3Int(Mathf.Min(_startVoxelAreaPosition.x, _endVoxelAreaPosition.x), 0, Mathf.Min(_startVoxelAreaPosition.z, _endVoxelAreaPosition.z)),
                new Vector3Int(Mathf.Max(_startVoxelAreaPosition.x, _endVoxelAreaPosition.x), ChunksManager.FieldSize.y, Mathf.Max(_startVoxelAreaPosition.z, _endVoxelAreaPosition.z)));
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

        if (!SceneData.ControlGUI.IsPanel)
        {
            _castResult = Raycast.CastByMouse(SceneData.RayLength);
            if (_castResult != null)
            {
                //MonoBehaviour.print(_castResult.lastPoint);
                if (ChunksManager.InField(_castResult.lastPoint))
                {
                    SceneData.Extractor.SetPosition(_castResult.lastPoint);
                    SceneData.Extractor.SetActive(true);
                }
                else
                {
                    _castResult = null;
                }
            }
        }
    }



    private void ResetExtractor()
    {
        SceneData.Extractor.SetPosition(_endVoxelAreaPosition);
        SceneData.Extractor.SetScale(Vector3.one);
    }

}
