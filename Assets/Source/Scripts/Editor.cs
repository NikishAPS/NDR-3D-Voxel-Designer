using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Editor : Mode
{
    private Vector3? _vertexPos;

    public override void Tick()
    {
        if (SceneData.eventInput.MouseDown0)
        {
            if (!SceneData.dragSystem.CheckCapture())
            {
                if (!SceneData.controlGUI.IsPanel)
                {
                    CastResult castResult = VoxelRayCast.CastVerticesByMouse(SceneData.rayLength);

                    if (castResult != null)
                    {
                        if (castResult.vertex != null)
                        {
                            Vector3 verPos = castResult.point + Vector3.one * 0.5f;
                            Vector3 offsetPos = (Vector3)SceneData.chunk.GetOffsetVertexByPos(verPos);
                            _vertexPos = verPos;

                            SceneData.dragSystem.SetPosition(verPos + offsetPos);
                            SceneData.dragSystem.SetActive(true);
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
            SceneData.chunk.OffsetVertexByPos((Vector3)_vertexPos, offset);
            SceneData.chunk.UpdateMesh();
            SceneData.dragSystem.OffsetPosition(offset);
        }
    }

    public override void Disable()
    {
        SceneData.dragSystem.SetActive(false);
        SceneData.dragSystem.drag -= Tesss;
    }

    public override void Enable()
    {
        SceneData.dragSystem.drag += Tesss;
    }
}
