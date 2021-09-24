using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewPanel : Panel
{
    public override void OnMouseMove()
    {
        if (ScreenAxes.CurAxis != null) return;

        ModeManager.OnMouseMove();
    }

    public override void OnLMouseDown()
    {
        if (ScreenAxes.CurAxis != null) return;
        ModeManager.OnLMouseDown();
    }

    public override void OnLMouseUp()
    {
        if (ScreenAxes.CurAxis != null) return;

        ModeManager.OnLMouseUp();
    }

    public override void OnRMouseDown()
    {
        if (ScreenAxes.CurAxis != null) return;

        ModeManager.OnRMouseDown();
    }

    public override void OnRMouseUp()
    {
        if (ScreenAxes.CurAxis != null) return;

        ModeManager.OnRMouseUp();
    }

}
