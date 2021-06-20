using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Mode
{
    public override void Tick()
    {
        SceneData.extractor.SetActive(false);
        if (!SceneData.controlGUI.IsPanel)
        {
            if (!IsDrag()) RayCastInvoke();
        }
    }

    public override void Disable()
    {
        SceneData.chunk.SetSelectedMeshActive(false);
        SceneData.dragSystem.SetActive(false);
        SceneData.dragSystem.drag -= SceneData.chunk.MoveVoxels;
        SceneData.extractor.SetActive(false);
    }

    public override void Enable()
    {
        SceneData.chunk.SetSelectedMeshActive(true);
        SceneData.dragSystem.drag += SceneData.chunk.MoveVoxels;
    }

    private bool IsDrag()
    {
        if (SceneData.eventInput.Delete) SceneData.chunk.DeleteVoxel();

        if (SceneData.chunk.SelectedIndicesCount == 0)
        {
            SceneData.dragSystem.SetActive(false);
            return false;
        }
        else
        {
            SceneData.dragSystem.SetActive(true);
            //SceneData.dragSystem.SetPosition(SceneData.chunk.MiddleSelectedPosition);

            if (!SceneData.dragSystem.CheckCapture())
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
        CastResult castResult = VoxelRayCast.CastByMouse(SceneData.rayLength);
        if (castResult != null)
        {
            if (castResult.voxel != null)
            {
                SceneData.extractor.SetActive(true);
                SceneData.extractor.SetPosition(castResult.point);
            }

            if (SceneData.eventInput.MouseDown0)
            {
                if (!SceneData.eventInput.LShift) SceneData.chunk.ResetSelection();
                SceneData.chunk.SelectVoxel(castResult.point);
                SceneData.dragSystem.SetPosition(SceneData.chunk.MiddleSelectedPosition);
            }
        }
    }
}
