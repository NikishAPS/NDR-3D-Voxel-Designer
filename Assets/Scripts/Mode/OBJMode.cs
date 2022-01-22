using UnityEngine;

public class ObjMode : Mode, IDrag
{
    public override void OnEnable()
    {
        InputEvent.LMouseDown += OnLMouseDown;
        //Axes.SetDragObject(this);
    }

    public override void OnDisable()
    {
        Axes.Active = false;
        Axes.ScaleActive = false;
        InputEvent.LMouseDown -= OnLMouseDown;
    }

    public override void OnLMouseDown()
    {
        RaycastHit hit;
        if (!Axes.IsHighlightedAxis())
        {
            OBJControl.SelectModel(null);
            Axes.Active = false;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform.CompareTag("OBJ"))
                {
                    Axes.Position = hit.transform.position;
                    Axes.Active = true;
                    Axes.ScaleActive = true;
                    OBJControl.SelectModel(hit.transform);
                }
            }
        }
    }

    public bool OnDrag1(DragTransform dragValue)
    {
        return OBJControl.TryDrag(dragValue);
    }

    public DragTransform GetDragCoordinates1()
    {
        if (OBJControl.SelectedModel != null)
        {
            return new DragTransform((OBJControl.SelectedModel.position * 100).RoundToFloat() / 100f,
                OBJControl.SelectedModel.localScale);
        }

        return new DragTransform();
    }

    public void OnDrag(DragTransform dragValue, out DragTransform dragResult)
    {
        dragResult = new DragTransform();
    }

    public DragTransform? GetDragCoordinates()
    {
        return null;
    }

    public void OnEndDrag(DragTransform dragResult)
    {

    }
}
