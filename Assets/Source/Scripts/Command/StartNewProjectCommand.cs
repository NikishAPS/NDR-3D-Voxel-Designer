using UnityEngine;
using NDR.UI;

public class StartNewProjectCommand : Command
{
    [SerializeField]
    private Panel _newProjectPanel, _projectPanel, _inspectorPanel, _statisticsPanel;
    [SerializeField]
    private InputField _projectName;
    [SerializeField]
    private InputField _projectSaveLocation;
    [SerializeField]
    private InputField _fieldWidth;
    [SerializeField]
    private InputField _fieldHeight;
    [SerializeField]
    private InputField _fieldDepth;
    [SerializeField]
    private Switcher _switcherInc;

    public override void Execute()
    {
        ChunksManager.SetParameters(new Vector3Int(_fieldWidth.Int, _fieldHeight.Int, _fieldDepth.Int), _switcherInc.Value);
        VoxelatorManager.Project.Init(_projectName.String, _projectSaveLocation.String);

        _newProjectPanel.Close();
        _projectPanel.Open();
        _statisticsPanel.Open();
        _inspectorPanel.Open();
    }
}
