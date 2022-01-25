using UnityEngine;

public class ObjMode : Mode, IDrag
{
    public override void OnEnable()
    {
        Axes.SetDragObject(this);
    }

    public override void OnDisable()
    {
        Axes.Active = false;
        Axes.ScaleActive = false;
        Axes.SetDragObject(null);
    }

    public override void OnLMouseDown()
    {
        RaycastHit hit;
        if (!Axes.IsHighlightedAxis())
        {
            //OBJControl.SelectModel(null);
            //Axes.Active = false;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask("OBJ")))
            {
                Axes.Position = hit.transform.position;
                Axes.Active = true;
                Axes.ScaleActive = true;
                ObjModelManager.Select(hit.transform.root.GetComponent<ObjModel>());
                //OBJControl.SelectModel(hit.transform);
            }
        }
    }

    public void OnDrag(DragTransform dragValue, out DragTransform dragResult)
    {
        ObjModelManager.DragSelectedModel(dragValue, out dragResult);
    }

    public DragTransform? GetDragCoordinates()
    {
        return null;
    }

    public void OnEndDrag(DragTransform dragResult)
    {

    }

}
