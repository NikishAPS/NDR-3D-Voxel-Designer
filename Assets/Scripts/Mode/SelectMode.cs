using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMode : Mode, IDrag
{
    private CastResult _castResult;


  
    public override void OnEnable()
    {
        ChunkManager.SetSelectedMeshActive(true);
        Axes.SetDragObject(this);
        InputEvent.DeleteDown += Delete;

        if(ChunkManager.SelectedVoxelCount > 0)
        {
            Axes.Position = ChunkManager.MiddleSelectedPos;
            Axes.Active = true;
        }
    }

    public override void OnDisable()
    {
        ChunkManager.SetSelectedMeshActive(false);
        Axes.Active = false;
        Extractor.Active = false;

        InputEvent.DeleteDown -= Delete;
    }

    public override void OnMouseMove()
    {
        _castResult = null;
        Extractor.Active = false;

        if (!Axes.IsHighlightedAxis())
        {
            _castResult = VoxelRaycast.CastByMouse(SceneData.RayLength);
            if (_castResult != null)
            {
                if (ChunkManager.InField(_castResult.CurrentVoxelPosition))
                {
                    Extractor.Position = _castResult.CurrentVoxelPosition;
                    Extractor.Active = true;
                    //VoxelatorManager.Coordinates.Value = _castResult.point;
                }
                else
                {
                    _castResult = null;
                }
            }
        }
    }

    public override void OnLMouseDown()
    {
        if (_castResult != null)
        {
            if (InputEvent.IsLShiftHold)
            {
                ChunkManager.SelectVoxel(_castResult.CurrentVoxelPosition);
            }
            else
            {
                ChunkManager.ResetVoxelSelection();
                ChunkManager.SelectVoxel(_castResult.CurrentVoxelPosition);
            }

            Axes.Position = ChunkManager.MiddleSelectedPos;
            Axes.Active = true;
        }
        else if (!Axes.IsHighlightedAxis())
        {
            ChunkManager.ResetVoxelSelection();
            Axes.Active = false;
        }
    }

    public void Delete()
    {
        Axes.Active = false;
        ChunkManager.DeleteSelectedVoxels();
    }

    public bool OnTryDrag(DragTransform dragValue)
    {
        return ChunkManager.MoveSelectedVoxels(dragValue);
    }

    public DragTransform GetDragCoordinates()
    {
        if(ChunkManager.SelectedVoxelCount == 1)
        {
            return new DragTransform(ChunkManager.SelectedVoxelPositions[0], Vector3.zero);
            //return new DragTransform(ChunkManager.GetSelectedVoxel(ChunkManager.SelectedVoxelPositions[0]).Position, Vector3.zero);
        }

        return null;
    }

    public void OnEndDrag(DragTransform dragValue)
    {

    }
}
