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
        //Presenter.EditVertexEvent += OnEditVertex;

        for(int i = 1; i < _tabs.Length; i++)
            _tabs[i].SetActive(false);
    }

    public override void OnOpen()
    {
        Voxelator.VertexChunkManager.EditSelectedVertexEvent += OnEditVertex;

        _vertexOffsetX.Min = -Voxelator.IncrementOption * 1.5f;
        _vertexOffsetX.Max = +Voxelator.IncrementOption * 1.5f;

        _vertexOffsetY.Min = -Voxelator.IncrementOption * 1.5f;
        _vertexOffsetY.Max = +Voxelator.IncrementOption * 1.5f;

        _vertexOffsetZ.Min = -Voxelator.IncrementOption * 1.5f;
        _vertexOffsetZ.Max = +Voxelator.IncrementOption * 1.5f;
    }

    public void OnOpenColorPickerPanel()
    {
        _colorPickerPanel.Open();
        _colorPickerPanel.ColorObject = this;
    }

    public void OnSetActiveGrid(bool active)
    {
        Voxelator.VoxelChunkManager.GridVoxelActivity = active;
    }

    public void SetColor(Color color)
    {
        _buildColor = color;

        _buildColorButton.DefaultColor = _buildColor;
        _buildColorButton.HoverColor = new Color(_buildColor.r * 0.9f, _buildColor.g * 0.9f, _buildColor.b * 0.9f, _buildColor.a);
        _buildColorButton.SelectedColor = _buildColor;

        _buildColorButton.SetColor(_buildColor);

        Voxelator.VoxelColor = new Vector3Byte((byte)(color.r*255), (byte)(color.g * 255), (byte)(color.b * 255));
    }

    public void OnMoveVertex(int value)
    {
        Vector3 vertexOffsetPosition = new Vector3(_vertexOffsetX.Value, _vertexOffsetY.Value, _vertexOffsetZ.Value) /
            Voxelator.IncrementOption;// +
            //Presenter.Vertex.Position;

        Invoker.Execute(new SetVertexOffsetCommand(vertexOffsetPosition));
        Axes.Position = Voxelator.VertexChunkManager.SelectedVertex.OffsetPosition.Value;

        return;

        if (Presenter.Vertex == null) return;

        

        Invoker.Execute(new SetVertexOffsetCommand(Presenter.Vertex.Position, vertexOffsetPosition));

    }

    public void OnResetVertexOffset()
    {
        Invoker.Execute(new SetVertexOffsetCommand(Vector3.zero));
        Axes.Position = Voxelator.VertexChunkManager.SelectedVertex.OffsetPosition.Value;
    }

    private void OnSwitchTab()
    {
        SwitchTab(Presenter.Mode);
    }

    private void OnEditVertex(VertexUnit selectedVertex)
    {
        _vertexOffsetX.Value = selectedVertex.GetOffset().x * Voxelator.IncrementOption;
        _vertexOffsetY.Value = selectedVertex.GetOffset().y * Voxelator.IncrementOption;
        _vertexOffsetZ.Value = selectedVertex.GetOffset().z * Voxelator.IncrementOption;
    }

    private void SwitchTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= _tabs.Length) return;

        _tabs[_tabIndex].SetActive(false);
        _tabIndex = tabIndex;
        _tabs[_tabIndex].SetActive(true);
    }

}
