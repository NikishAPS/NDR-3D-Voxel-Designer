using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMode : Mode
{
    private Vertex _vertex;

    public override void Tick()
    {
        if (SceneData.EventInput.GetMouseDown0)
        {
            if (!SceneData.DragSystem.CheckCapture())
            {
                if (!SceneData.ControlGUI.IsPanel)
                {
                    CastVertexResult castResult = Raycast.CastVertexByMouse(SceneData.RayLength);

                    if (castResult != null)
                    {
                        if (castResult.Vertex != null)
                        {
                            ChunksManager.SelectedVertex = castResult.Vertex;
                            SceneData.DragSystem.SetPosition(castResult.Vertex.Position);
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

  

    public override void Disable()
    {
        SceneData.DragSystem.SetActive(false);
        //SceneData.DragSystem.drag -= SceneData.Chunk.OffsetVertex;
        SceneData.DragSystem.drag -= MoveVertex;
        InputEvent.LMouseDown -= LMouseDown;
    }

    public override void Enable()
    {
        //SceneData.DragSystem.drag += SceneData.Chunk.OffsetVertex;
        SceneData.DragSystem.drag += MoveVertex;
        InputEvent.LMouseDown += LMouseDown;
    }

    public void MoveVertex(Vector3 startPos, Vector3 offset)
    {
        if (_vertex != null)
        {
            VoxelatorManager.Coordinates.Value = (startPos - _vertex.PivotPosition) * ChunksManager.IncrementOption;
            ChunksManager.MoveVertex(startPos, offset);
        }
    }

    public void LMouseDown()
    {
        if (!SceneData.DragSystem.CheckCapture())
        {
            _vertex = null;
            SceneData.DragSystem.SetActive(false);

            if (!SceneData.ControlGUI.IsPanel)
            {
                CastVertexResult castResult = Raycast.CastVertexByMouse(SceneData.RayLength);

                if (castResult != null)
                {
                    if (castResult.Vertex != null)
                    {
                        ChunksManager.SelectedVertex = castResult.Vertex;
                        SceneData.DragSystem.SetPosition(castResult.Vertex.Position);
                        SceneData.DragSystem.SetActive(true);
                        _vertex = castResult.Vertex;

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
