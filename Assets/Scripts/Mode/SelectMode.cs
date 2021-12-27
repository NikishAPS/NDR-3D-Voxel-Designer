using UnityEngine;

public class SelectMode : Mode, IDrag
{
    private CastResult _castResult;
  
    public override void OnEnable()
    {
        Voxelator.VoxelChunkManager.GridSelectedVoxelActivity = true;
        Axes.SetDragObject(this);
        InputEvent.DeleteDown += Delete;

        if(VoxelUnit.SelectedCount > 0)
        {
            Axes.Position = VoxelUnit.MiddleSelectedPosition;
            Axes.Active = true;
        }
    }

    public override void OnDisable()
    {
        Voxelator.VoxelChunkManager.GridSelectedVoxelActivity = false;
        Axes.Active = false;
        Extractor.Active = false;

        InputEvent.DeleteDown -= Delete;
    }

    public override void OnMouseMove()
    {
        _castResult = null;
        Extractor.Active = false;

        if (!Axes.IsHighlightedAxis())
        {
            _castResult = VoxelRaycast.CastByMouse(SceneParameters.RayLength);
            if (_castResult != null)
            {
                if (Voxelator.InsideField(_castResult.CurrentVoxelPosition))
                {
                    Extractor.Position = _castResult.CurrentVoxelPosition;
                    Extractor.Active = true;
                    //VoxelatorManager.Coordinates.Value = _castResult.point;
                }
                else
                {
                    _castResult = null;
                }
            }
        }
    }

    public override void OnLMouseDown()
    {
        if (_castResult != null)
        {
            if (InputEvent.IsLShiftHold)
            {
                Voxelator.SelectVoxel(_castResult.CurrentVoxelPosition, true);
            }
            else
            {
                Voxelator.ResetVoxelSelection();
                Voxelator.SelectVoxel(_castResult.CurrentVoxelPosition, true);
            }

            Voxelator.UpdateChunks();

            Axes.Position = VoxelUnit.MiddleSelectedPosition;
            Axes.Active = true;
        }
        else if (!Axes.IsHighlightedAxis())
        {
            Voxelator.ResetVoxelSelection();
            Voxelator.UpdateChunks();
            Axes.Active = false;
        }
    }

    public void Delete()
    {
        Axes.Active = false;
        Voxelator.DeleteSelectedVoxels();
        Voxelator.UpdateChunks();
    }

    public void OnStartDrag()
    {

    }

    public void OnDrag(DragTransform dragValue, out DragTransform dragResult)
    {
        dragResult = new DragTransform();

        Voxelator.DragSelectedVoxels(dragValue.Position, out dragResult.Position);
        Voxelator.UpdateChunks();
    }

    public DragTransform? GetDragCoordinates()
    {
        if(VoxelUnit.SelectedCount == 1)
        {
            //return new DragTransform(ChunkManager.SelectedVoxelPositions[0], Vector3.zero);
            return new DragTransform(VoxelUnit.SelectedHead.Position, Vector3.zero);
        }

        return null;
    }

    public void OnEndDrag(DragTransform dragResult)
    {
        Invoker.Execute(new SaveSelectedVoxelDragValue(dragResult.Position));
    }

}
