using UnityEngine;

public class ViewPanel : Panel
{
    [SerializeField] private ScreenAxes _screenAxes;

    public override void OnMouseMove()
    {
        if (ScreenAxes.CurAxis != null) return;

        ModeManager.OnMouseMove();
    }

    public override void OnLMouseDown()
    {
        _screenAxes.OnLMouseDown();
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
