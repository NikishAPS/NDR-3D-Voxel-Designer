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
            if (!Axes.IsHighlightedAxis())
            {
                if (!SceneData.ControlGUI.IsPanel)
                {
                    CastVertexResult castResult = Raycast.CastVertexByMouse(SceneData.RayLength);

                    if (castResult != null)
                    {
                        if (castResult.Vertex != null)
                        {
                            ChunksManager.SelectedVertex = castResult.Vertex;
                            Axes.Position = castResult.Vertex.Position;
                            Axes.Active = true;

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
        Axes.Active = false;
        //SceneData.DragSystem.drag -= SceneData.Chunk.OffsetVertex;
        Axes.DragPosition -= MoveVertex;
        InputEvent.LMouseDown -= LMouseDown;
    }

    public override void Enable()
    {
        //SceneData.DragSystem.drag += SceneData.Chunk.OffsetVertex;
        Axes.DragPosition += MoveVertex;
        InputEvent.LMouseDown += LMouseDown;
    }

    public Vector3? MoveVertex(Vector3 offset)
    {
        if (_vertex != null)
        {
            VoxelatorManager.Coordinates.Value = (_vertex.Position - _vertex.PivotPosition) * ChunksManager.IncrementOption;
            return ChunksManager.MoveVertex(offset);
        }

        return null;
    }

    public void LMouseDown()
    {
        if (!Axes.IsHighlightedAxis())
        {
            _vertex = null;
            Axes.Active = false;

            if (!SceneData.ControlGUI.IsPanel)
            {
                CastVertexResult castResult = Raycast.CastVertexByMouse(SceneData.RayLength);

                if (castResult != null)
                {
                    if (castResult.Vertex != null)
                    {
                        ChunksManager.SelectedVertex = castResult.Vertex;
                        Axes.Position = castResult.Vertex.Position;
                        Axes.Active = true;
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
