using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditMode : Mode, IDrag
{
    private Vertex _vertex;



    public override void OnEnable()
    {
        //Axes.DragPosition += ChunksManager.MoveVertex;
        Axes.SetDragObject(this);
    }

    public override void OnDisable()
    {
        Axes.Active = false;
        //Axes.DragPosition -= ChunksManager.MoveVertex;
    }

    public override void OnLMouseDown()
    {
        if (!Axes.IsHighlightedAxis())
        {
            _vertex = null;
            Axes.Active = false;

            CastVertexResult castResult = Raycast.CastVertexByMouse(SceneData.RayLength);

            if (castResult != null)
            {
                if (castResult.Vertex != null)
                {
                    ChunkManager.SelectedVertex = castResult.Vertex;
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

    public bool OnTryDrag(DragTransform dragValue)
    {
        return ChunkManager.TryMoveVertex(dragValue);
    }

    public DragTransform GetDragCoordinates()
    {
        if(ChunkManager.SelectedVertex != null)
        {
            return new DragTransform(ChunkManager.SelectedVertex.GetOffset() * ChunkManager.IncrementOption, Vector3.zero);
        }

        return null;
    }

}
