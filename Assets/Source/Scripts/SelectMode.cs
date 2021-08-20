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
        SceneData.DragSystem.SetActive(false);
        //SceneData.DragSystem.drag -= SceneData.Chunk.MoveSelectedVoxels;
        SceneData.Extractor.SetActive(false);
        ChunksManager.SetSelectedMeshActive(false);

        SceneData.DragSystem.drag -= ChunksManager.MoveSelectedVoxels;
        SceneData.EventInput.Delete -= Delete;
        InputEvent.MouseMove -= MouseMove;
        InputEvent.LMouseDown -= LMouseDown;
    }

    public override void Enable()
    {
        //SceneData.Chunk.SetSelectedMeshActive(true);
        // SceneData.DragSystem.drag += SceneData.Chunk.MoveSelectedVoxels;

        ChunksManager.SetSelectedMeshActive(true);

        SceneData.DragSystem.drag += ChunksManager.MoveSelectedVoxels;
        SceneData.EventInput.Delete += Delete;
        InputEvent.MouseMove += MouseMove;
        InputEvent.LMouseDown += LMouseDown;

        if(ChunksManager.SelectedVoxelCount > 0)
        {
            SceneData.DragSystem.SetPosition(ChunksManager.MiddleSelectedPos);
            SceneData.DragSystem.SetActive(true);
        }
    }

    public void MouseMove()
    {
        _castResult = null;
        SceneData.Extractor.SetActive(false);

        if (!SceneData.DragSystem.CheckCapture())
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

                SceneData.DragSystem.SetPosition(ChunksManager.MiddleSelectedPos);
                SceneData.DragSystem.SetActive(true);
            }
            else if (!SceneData.DragSystem.CheckCapture())
            {
                ChunksManager.ResetVoxelSelection();
                SceneData.DragSystem.SetActive(false);
            }
        }
    }

    public void Delete()
    {
        SceneData.DragSystem.SetActive(false);
        ChunksManager.DeleteSelectedVoxels();
    }
}
