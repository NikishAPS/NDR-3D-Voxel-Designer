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
        InputEvent.XHold += OnXHold;
        InputEvent.YHold += OnYHold;
        InputEvent.ZHold += OnZHold;
    }

    public override void OnDisable()
    {
        Axes.Active = false;
        //Axes.DragPosition -= ChunksManager.MoveVertex;

        InputEvent.XHold -= OnXHold;
        InputEvent.YHold -= OnYHold;
        InputEvent.ZHold -= OnZHold;
    }

    public override void OnLMouseDown()
    {
        if (!Axes.IsHighlightedAxis())
        {
            _vertex = null;
            Axes.Active = false;

            VertexCastResult castResult = VoxelRaycast.VertexCastByMouse(SceneData.RayLength);

            if (castResult != null)
            {
                if (castResult.Vertex != null)
                {
                    //ChunkManager.SelectedVertex = castResult.Vertex;
                    Axes.Position = castResult.Vertex.Position;
                    Axes.Active = true;
                    _vertex = castResult.Vertex;

                    Invoker.Execute(new SelectVertexCommand(castResult.Vertex));

                    //Vector3 verPos = castResult.point + Vector3.one * 0.5f;
                    // Vector3 offsetPos = (Vector3)SceneData.Chunk.GetOffsetVertexByPos(verPos);
                    //_vertexPos = verPos;

                    // SceneData.DragSystem.SetPosition(verPos + offsetPos);
                    //SceneData.DragSystem.SetActive(true);
                }
            }
        }
    }

    public override void OnLMouseUp()
    {
        if(_vertex != null)
        {
            
        }
    }

    public bool OnTryDrag(DragTransform dragValue)
    {
        return ChunkManager.TryMoveVertex(dragValue);
    }

    public void OnXHold()
    {
        if (_vertex == null || InputEvent.IsMouseScroll == 0) return;
        Vector3 position = _vertex.Position + Vector3.right * InputEvent.IsMouseScroll / ChunkManager.IncrementOption;
        Invoker.Execute(new SetVertexPositionCommand(_vertex.PivotPosition, position));
        Presenter.EditVertex();
    }

    public void OnYHold()
    {
        if (_vertex == null || InputEvent.IsMouseScroll == 0) return;
        Vector3 position = _vertex.Position + Vector3.up * InputEvent.IsMouseScroll / ChunkManager.IncrementOption;
        Invoker.Execute(new SetVertexPositionCommand(_vertex.PivotPosition, position));
        Presenter.EditVertex();
    }

    public void OnZHold()
    {
        if (_vertex == null || InputEvent.IsMouseScroll == 0) return;
        Vector3 position = _vertex.Position + Vector3.forward * InputEvent.IsMouseScroll / ChunkManager.IncrementOption;
        Invoker.Execute(new SetVertexPositionCommand(_vertex.PivotPosition, position));
        Presenter.EditVertex();
    }

    public DragTransform GetDragCoordinates()
    {
        if(ChunkManager.SelectedVertex != null)
        {
            return new DragTransform(ChunkManager.SelectedVertex.Offset * ChunkManager.IncrementOption, Vector3.zero);
        }

        return null;
    }

    public void OnEndDrag(DragTransform dragValue)
    {
        if (dragValue.Position != Vector3.zero)
        {
            Invoker.Execute(new ApproveVertexOffsetCommand(_vertex, dragValue.Position));
        }
    }
}
