using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMode : Mode
{
    private Vector3? _vertexPos;

    public override void Tick()
    {
        if (SceneData.EventInput.MouseDown0)
        {
            if (!SceneData.DragSystem.CheckCapture())
            {
                if (!SceneData.ControlGUI.IsPanel)
                {
                    CastResult castResult = VoxelRayCast.CastVerticesByMouse(SceneData.RayLength);

                    if (castResult != null)
                    {
                        if (castResult.vertex != null)
                        {
                            Vector3 verPos = castResult.point + Vector3.one * 0.5f;
                            Vector3 offsetPos = (Vector3)SceneData.Chunk.GetOffsetVertexByPos(verPos);
                            _vertexPos = verPos;

                            SceneData.DragSystem.SetPosition(verPos + offsetPos);
                            SceneData.DragSystem.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public void Tesss(Vector3 startPos, Vector3 offset)
    {
        if (_vertexPos != null && offset != Vector3.zero)
        {
            SceneData.Chunk.OffsetVertexByPos((Vector3)_vertexPos, offset);
            SceneData.Chunk.UpdateMesh();
            SceneData.DragSystem.OffsetPosition(offset);
        }
    }

    public override void Disable()
    {
        SceneData.DragSystem.SetActive(false);
        SceneData.DragSystem.drag -= Tesss;
    }

    public override void Enable()
    {
        SceneData.DragSystem.drag += Tesss;
    }
}
