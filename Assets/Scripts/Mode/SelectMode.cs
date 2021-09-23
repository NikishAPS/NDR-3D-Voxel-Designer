using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMode : Mode, IDrag
{
    private CastResult _castResult;


  
    public override void OnEnable()
    {
        ChunksManager.SetSelectedMeshActive(true);
        Axes.SetDragObject(this);
        InputEvent.Delete += Delete;

        if(ChunksManager.SelectedVoxelCount > 0)
        {
            Axes.Position = ChunksManager.MiddleSelectedPos;
            Axes.Active = true;
        }
    }

    public override void OnDisable()
    {
        ChunksManager.SetSelectedMeshActive(false);
        Axes.Active = false;
        Extractor.Active = false;

        InputEvent.Delete -= Delete;
    }

    public override void OnMouseMove()
    {
        _castResult = null;
        Extractor.Active = false;

        if (!Axes.IsHighlightedAxis())
        {
            _castResult = Raycast.CastByMouse(SceneData.RayLength);
            if (_castResult != null)
            {
                if (ChunksManager.InField(_castResult.point))
                {
                    Extractor.Position = _castResult.point;
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
            if (InputEvent.LShift)
            {
                ChunksManager.SelectVoxel(_castResult.point);
            }
            else
            {
                ChunksManager.ResetVoxelSelection();
                ChunksManager.SelectVoxel(_castResult.point);
            }

            Axes.Position = ChunksManager.MiddleSelectedPos;
            Axes.Active = true;
        }
        else if (!Axes.IsHighlightedAxis())
        {
            ChunksManager.ResetVoxelSelection();
            Axes.Active = false;
        }
    }

    public void Delete()
    {
        Axes.Active = false;
        ChunksManager.DeleteSelectedVoxels();
    }

    public bool OnTryDrag(DragTransform dragValue)
    {
        return ChunksManager.MoveSelectedVoxels(dragValue);
    }

    public DragTransform GetDragCoordinates()
    {
        if(ChunksManager.SelectedVoxelCount == 1)
        {
            return new DragTransform(ChunksManager.GetSelectedVoxel(ChunksManager.SelectedVoxelPositions[0]).Position, Vector3.zero);
        }

        return null;
    }
}
