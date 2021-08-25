using UnityEngine;
using System.Collections;

public class ObjMode : Mode
{
    public override void Disable()
    {
        Axes.Active = false;
        Axes.ScaleAxesActive = false;
        InputEvent.LMouseDown -= OnLMouseDown;
        Axes.DragPosition -= OBJControl.OnMove;
        Axes.DragScale -= OBJControl.OnScale;
    }

    public override void Enable()
    {
        InputEvent.LMouseDown += OnLMouseDown;
        Axes.DragPosition += OBJControl.OnMove;
        Axes.DragScale += OBJControl.OnScale;
    }

    public void OnLMouseDown()
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
                    Axes.ScaleAxesActive = true;
                    OBJControl.SelectModel(hit.transform);
                }
            }
        }
    }

    public override void Tick()
    {
        //throw new System.NotImplementedException();
    }
}
