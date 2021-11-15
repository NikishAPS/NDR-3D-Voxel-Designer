using UnityEngine;
using System.Collections;

public class InspectorPanel : Panel, IColor
{
    [SerializeField] private GameObject[] _tabs;

    [SerializeField] private Color _buildColor = Color.white;
    [SerializeField] private Button _buildColorButton;
    [SerializeField] private Slider _vertexOffsetX;
    [SerializeField] private Slider _vertexOffsetY;
    [SerializeField] private Slider _vertexOffsetZ;

    private ColorPickerPanel _colorPickerPanel;
    private int _tabIndex = 0;

    public override void OnInit()
    {
        _colorPickerPanel = PanelManager.GetPanel<ColorPickerPanel>();
        SetColor(_buildColor);
        Presenter.ChangeModeEvent += OnSwitchTab;
        Presenter.EditVertexEvent += OnEditVertex;

        for(int i = 1; i < _tabs.Length; i++)
            _tabs[i].SetActive(false);
    }

    public void OnOpenColorPickerPanel()
    {
        _colorPickerPanel.Open();
        _colorPickerPanel.ColorObject = this;
    }

    public void OnSetActiveGrid(bool active)
    {
        ChunkManager.VoxelGridActivity = active;
    }

    public void SetColor(Color color)
    {
        _buildColor = color;

        _buildColorButton.DefaultColor = _buildColor;
        _buildColorButton.HoverColor = new Color(_buildColor.r * 0.9f, _buildColor.g * 0.9f, _buildColor.b * 0.9f, _buildColor.a);
        _buildColorButton.SelectedColor = _buildColor;

        _buildColorButton.SetColor(_buildColor);

        ChunkManager.SetVoxelIdByColor(color);
    }

    public void OnMoveVertex(int value)
    {
        if (Presenter.Vertex == null) return;

        Vector3 position = new Vector3(_vertexOffsetX.Value, _vertexOffsetY.Value, _vertexOffsetZ.Value) / ChunkManager.IncrementOption + 
            Presenter.Vertex.PivotPosition;

        Invoker.Execute(new SetVertexPositionCommand(Presenter.Vertex.PivotPosition, position));
    }

    private void OnSwitchTab()
    {
        SwitchTab(Presenter.Mode);
    }

    private void OnEditVertex()
    {
        _vertexOffsetX.Value = Presenter.Vertex.Offset.x * ChunkManager.IncrementOption;
        _vertexOffsetY.Value = Presenter.Vertex.Offset.y * ChunkManager.IncrementOption;
        _vertexOffsetZ.Value = Presenter.Vertex.Offset.z * ChunkManager.IncrementOption;
    }

    private void SwitchTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= _tabs.Length) return;

        _tabs[_tabIndex].SetActive(false);
        _tabIndex = tabIndex;
        _tabs[_tabIndex].SetActive(true);
    }

}
