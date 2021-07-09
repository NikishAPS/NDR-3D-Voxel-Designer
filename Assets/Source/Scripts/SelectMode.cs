using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMode : Mode
{
    public override void Tick()
    {
        SceneData.Extractor.SetActive(false);
        if (!SceneData.ControlGUI.IsPanel)
        {
            if (SceneData.EventInput.Delete) SceneData.Chunk.DeleteSelectedVoxels();

            if (!IsDrag()) RayCastInvoke();
        }
    }

    public override void Disable()
    {
        SceneData.Chunk.SetSelectedMeshActive(false);
        SceneData.DragSystem.SetActive(false);
        SceneData.DragSystem.drag -= SceneData.Chunk.MoveSelectedVoxels;
        SceneData.Extractor.SetActive(false);
    }

    public override void Enable()
    {
        SceneData.Chunk.SetSelectedMeshActive(true);
        SceneData.DragSystem.drag += SceneData.Chunk.MoveSelectedVoxels;
    }

    private bool IsDrag()
    {
        if (SceneData.Chunk.Selector.SelectedVoxelIndices.Count == 0)
        {
            SceneData.DragSystem.SetActive(false);
            return false;
        }
        else
        {
            SceneData.DragSystem.SetActive(true);
            //SceneData.dragSystem.SetPosition(SceneData.chunk.MiddleSelectedPosition);

            if (!SceneData.DragSystem.CheckCapture())
            {
                return false;
            }

            //if (SceneData.dragSystem.GetDragValue() != Vector3Int.zero)
            //    SceneData.chunk.MoveVoxels(SceneData.dragSystem.GetDragValue());
            //SceneData.chunk.MoveVoxel();


        }

        return true;
    }

    private void RayCastInvoke()
    {
        CastResult castResult = VoxelRayCast.CastByMouse(SceneData.RayLength);
        if (castResult != null)
        {
            if (castResult.voxel != null)
            {
                SceneData.Extractor.SetActive(true);
                SceneData.Extractor.SetPosition(castResult.point);
            }

            if (SceneData.EventInput.MouseDown0)
            {
                if (!SceneData.EventInput.LShift) SceneData.Chunk.ResetSelection();
                SceneData.Chunk.SelectVoxel(castResult.point);
                SceneData.DragSystem.SetPosition(SceneData.Chunk.Selector.MiddleSelectedPos);
            }
        }
    }
}
