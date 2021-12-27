using UnityEngine;

public class EditMode : Mode, IDrag
{
    public override void OnEnable()
    {
        Voxelator.VertexChunkManager.VerticesActive = true;

        Axes.SetDragObject(this);
        if (Voxelator.VertexChunkManager.SelectedVertex != null)
        {
            Axes.Position = Voxelator.VertexChunkManager.SelectedVertex.OffsetPosition.Value;
            Axes.Active = true;
        }

        InputEvent.XHold += OnXHold;
        InputEvent.YHold += OnYHold;
        InputEvent.ZHold += OnZHold;
    }

    public override void OnDisable()
    {
        Voxelator.VertexChunkManager.VerticesActive = false;
        Axes.Active = false;

        InputEvent.XHold -= OnXHold;
        InputEvent.YHold -= OnYHold;
        InputEvent.ZHold -= OnZHold;
    }

    public override void OnLMouseDown()
    {
        if (!Axes.IsHighlightedAxis())
        {
            Axes.Active = false;

            VertexCastResult castResult = VoxelRaycast.VertexCastByMouse(SceneParameters.RayLength);

            if (castResult != null)
            {
                Axes.Position = castResult.Vertex.OffsetPosition.Value;
                Axes.Active = true;

                Invoker.Execute(new SelectVertexCommand(castResult.Vertex.Position));
            }
        }
    }

    public override void OnLMouseUp()
    {
        //if(_vertex != null)
        //{
            
        //}
    }

    public void OnXHold()
    {
        if (InputEvent.MouseScrollDelta == 0) return;
        Invoker.Execute(new ShiftSelectedVertexByStepCommand(Vector3Int.right * (int)InputEvent.MouseScrollDelta));
        Axes.Position = Voxelator.VertexChunkManager.SelectedVertex.OffsetPosition.Value;
    }

    public void OnYHold()
    {
        if (InputEvent.MouseScrollDelta == 0) return;
        Invoker.Execute(new ShiftSelectedVertexByStepCommand(Vector3Int.up * (int)InputEvent.MouseScrollDelta));
        Axes.Position = Voxelator.VertexChunkManager.SelectedVertex.OffsetPosition.Value;
    }

    public void OnZHold()
    {
        if (InputEvent.MouseScrollDelta == 0) return;
        Invoker.Execute(new ShiftSelectedVertexByStepCommand(new Vector3Int().Forward() * (int)InputEvent.MouseScrollDelta));
        Axes.Position = Voxelator.VertexChunkManager.SelectedVertex.OffsetPosition.Value;
    }

    public void OnDrag(DragTransform dragValue, out DragTransform dragResult)
    {
        dragResult = new DragTransform();
        Voxelator.DragSelectedVertex(dragValue.Position, out dragResult.Position);
        Voxelator.UpdateChunks();
    }

    public DragTransform? GetDragCoordinates()
    {
        if (Voxelator.VertexChunkManager.SelectedVertex != null)
            return new DragTransform(Voxelator.VertexChunkManager.SelectedVertex.GetOffset() * Voxelator.IncrementOption);

        return null;
    }

    public void OnEndDrag(DragTransform dragResult)
    {
        Invoker.Execute(new SaveVertexDragValue(dragResult.Position));
    }
}
