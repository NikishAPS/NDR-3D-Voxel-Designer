using FileBrowser;
using NDR.UI;
using UnityEngine;

public class NewProjectPanel : Panel
{
    [SerializeField] private InputField _projectNameField;
    [SerializeField] private InputField _projectSaveLocationField;
    [SerializeField] private InputField _widthField;
    [SerializeField] private InputField _heightField;
    [SerializeField] private InputField _depthField;
    [SerializeField] private Switcher _switcherIncrement;

    public override void OnInit()
    {
        Open();
    }

    public void OnSetSaveDirectoryPath()
    {
        string[] paths = StandaloneFileBrowser.OpenFolderPanel("Set save location (the project folder will be created here)", Application.dataPath, false);

        if (paths.Length > 0)
        {
            _projectSaveLocationField.SetValue(paths[0]);
        }
    }

    public async void OnCreateNewProject()
    {
        if (Project.Exists(_projectNameField.String, _projectSaveLocationField.String))
        {
            QuestionPanel panel = PanelManager.GetPanel<QuestionPanel>();
            panel.Title.TextValue = "A project with that name already exists. Overwrite it?";
            panel.ConfirmTitle = "Yes";
            panel.RejectTitle = "No";
            panel.CancelTitle = "Cancel";
            panel.Open();

            if (await panel.GetAnswer() != QuestionPanel.AnswerType.Confirm) return;
        }

        Voxelator.Init(new Vector3Int(_widthField.Int, _heightField.Int, _depthField.Int), (Voxelator.IncrementOptionType)_switcherIncrement.Value);

        Project.Init();
        if (!Project.TryCreate(_projectNameField.String, _projectSaveLocationField.String))
            return;

        OnStart();

        Close();
    }

    public void OnOpenExistingProject()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select existing .ndr-file", Application.dataPath, Project.FileExtension, false);
        if (paths.Length > 0)
        {
            if(Project.TryLoad(paths[0]))
            {
                Project.Init();

                OnStart();
                Close();
            }
        }

    }

    private void OnStart()
    {
        ScreenAxes.Active = true;

        PanelManager.GetPanel<ViewPanel>().Open();
        PanelManager.GetPanel<ProjectPanel>().Open();
        PanelManager.GetPanel<StatisticsPanel>().Open();
        PanelManager.GetPanel<InspectorPanel>().Open();

        GridManager.Size = Voxelator.VoxelChunkManager.FieldSize;

        CameraController.Position = Vector3.zero;
        CameraController.MainCamera.nearClipPlane = -Voxelator.FieldSize.Max();
        CameraController.MainCamera.farClipPlane = Voxelator.FieldSize.Max();
    }

}
