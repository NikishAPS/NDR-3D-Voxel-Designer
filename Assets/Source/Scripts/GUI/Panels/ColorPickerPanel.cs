using UnityEngine;
using CustomUnityEvents;

public class ColorPickerPanel : Panel
{
    public IColor ColorObject { get; set; }

    public override void OnOpen()
    {
        PanelManager.PinThePanel(this);
    }

    public override void OnClose()
    {
        PanelManager.PinThePanel(null);
    }

    public void SetColor(Color color)
    {
        if (ColorObject != null)
            ColorObject.SetColor(color);
    }
}
