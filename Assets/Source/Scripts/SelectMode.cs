using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMode : Mode
{
    private CastResult _castResult;
    public override void Tick()
    {
        //SceneData.Extractor.SetActive(false);
        //if (!SceneData.ControlGUI.IsPanel)
        //{
        //    //if (SceneData.EventInput.GetDelete) SceneData.Chunk.DeleteSelectedVoxels();

        //    if (!IsDrag()) RayCastInvoke();
        //}
    }

    public override void Disable()
    {
        //SceneData.Chunk.SetSelectedMeshActive(false);
        Axes.Active = false;
        //SceneData.DragSystem.drag -= SceneData.Chunk.MoveSelectedVoxels;
        SceneData.Extractor.SetActive(false);
        ChunksManager.SetSelectedMeshActive(false);

        Axes.DragPosition -= ChunksManager.MoveSelectedVoxels;
        SceneData.EventInput.Delete -= Delete;
        InputEvent.MouseMove -= MouseMove;
        InputEvent.LMouseDown -= LMouseDown;
    }

    public override void Enable()
    {
        //SceneData.Chunk.SetSelectedMeshActive(true);
        // SceneData.DragSystem.drag += SceneData.Chunk.MoveSelectedVoxels;

        ChunksManager.SetSelectedMeshActive(true);

        Axes.DragPosition += ChunksManager.MoveSelectedVoxels;
        SceneData.EventInput.Delete += Delete;
        InputEvent.MouseMove += MouseMove;
        InputEvent.LMouseDown += LMouseDown;

        if(ChunksManager.SelectedVoxelCount > 0)
        {
            Axes.Position = ChunksManager.MiddleSelectedPos;
            Axes.Active = true;
        }
    }

    public void MouseMove()
    {
        _castResult = null;
        SceneData.Extractor.SetActive(false);

        if (!Axes.IsHighlightedAxis())
        {
            if (!SceneData.ControlGUI.IsPanel)
            {
                _castResult = Raycast.CastByMouse(SceneData.RayLength);
                if (_castResult != null)
                {
                    if (ChunksManager.InField(_castResult.point))
                    {
                        SceneData.Extractor.SetPosition(_castResult.point);
                        SceneData.Extractor.SetActive(true);
                        VoxelatorManager.Coordinates.Value = _castResult.point;
                    }
                    else
                    {
                        _castResult = null;
                    }
                }
            }
        }
    }

    public void LMouseDown()
    {
        if (!SceneData.ControlGUI.IsPanel)
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
    }

    public void Delete()
    {
        Axes.Active = false;
        ChunksManager.DeleteSelectedVoxels();
    }
}
