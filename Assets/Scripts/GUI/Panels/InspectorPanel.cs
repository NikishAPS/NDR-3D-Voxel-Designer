using UnityEngine;
using System.Collections;

public class InspectorPanel : Panel, IColor
{
    [SerializeField] private Color _buildColor = Color.white;
    [SerializeField] private Button _buildColorButton;

    private ColorPickerPanel _colorPickerPanel;

    public override void OnInit()
    {
        _colorPickerPanel = PanelManager.GetPanel<ColorPickerPanel>();

        SetColor(_buildColor);
    }

    public void OnOpenColorPickerPanel()
    {
        _colorPickerPanel.Open();
        _colorPickerPanel.ColorObject = this;
    }

    public void SetColor(Color color)
    {
        _buildColor = color;

        _buildColorButton.DefaultColor = _buildColor;
        _buildColorButton.HoverColor = new Color(_buildColor.r * 0.9f, _buildColor.g * 0.9f, _buildColor.b * 0.9f, _buildColor.a);
        _buildColorButton.SelectedColor = _buildColor;

        _buildColorButton.SetColor(_buildColor);

        ChunksManager.SetVoxelIdByColor(color);
    }
}
