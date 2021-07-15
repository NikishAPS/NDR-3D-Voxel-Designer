using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMode : Mode
{
    private Vector3? _vertexPos;

    public override void Tick()
    {
        if (SceneData.EventInput.GetMouseDown0)
        {
            if (!SceneData.DragSystem.CheckCapture())
            {
                if (!SceneData.ControlGUI.IsPanel)
                {
                    CastVertexResult castResult = VoxelRayCast.CastVertexByMouse(SceneData.RayLength);

                    if (castResult != null)
                    {
                        if (castResult.VertexPoint != null)
                        {
                            SceneData.DragSystem.SetPosition(castResult.VertexPoint.Position);
                            SceneData.DragSystem.SetActive(true);

                            //Vector3 verPos = castResult.point + Vector3.one * 0.5f;
                           // Vector3 offsetPos = (Vector3)SceneData.Chunk.GetOffsetVertexByPos(verPos);
                            //_vertexPos = verPos;

                           // SceneData.DragSystem.SetPosition(verPos + offsetPos);
                            //SceneData.DragSystem.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    public void DragVertex(Vector3 startPos, Vector3 offset)
    {
        if (_vertexPos != null && offset != Vector3.zero)
        {
            SceneData.Chunk.OffsetVertex((Vector3)_vertexPos, offset);
            //SceneData.Chunk.UpdateMesh();
            //SceneData.DragSystem.OffsetPosition(offset);
        }
    }

    public override void Disable()
    {
        SceneData.DragSystem.SetActive(false);
        SceneData.DragSystem.drag -= SceneData.Chunk.OffsetVertex;
    }

    public override void Enable()
    {
        SceneData.DragSystem.drag += SceneData.Chunk.OffsetVertex;
    }
}
