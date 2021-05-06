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
        SceneData.chunk.transform.GetChild(0).gameObject.SetActive(false);
        SceneData.dragSystem.SetActive(false);
        SceneData.dragSystem.drag -= SceneData.chunk.MoveVoxels;
    }

    public override void Enable()
    {
        SceneData.chunk.transform.GetChild(0).gameObject.SetActive(true);
        SceneData.dragSystem.drag += SceneData.chunk.MoveVoxels;
    }

    private bool IsDrag()
    {
        if (SceneData.eventInput.Delete) SceneData.chunk.DeleteVoxel();

        if (SceneData.chunk.SelectedIndecesCount == 0)
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
        CastResult castResult = RayCaster.CastByMouse(SceneData.rayLength);
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
