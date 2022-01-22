using FileBrowser;
using UnityEngine;

public class ProjectPanel : Panel
{
    public Switcher ModeSwitcher => _modeSwitcher;
    public Vector3Bool Mirror
    {
        get => new Vector3Bool(_mirrorX.Value, _mirrorY.Value, _mirrorZ.Value);
        set
        {
            _mirrorX.Value = value.X;
            _mirrorY.Value = value.Y;
            _mirrorZ.Value = value.Z;
        }
    }

    [SerializeField] private Switcher _modeSwitcher;
    [SerializeField] private CheckBox _mirrorX;
    [SerializeField] private CheckBox _mirrorY;
    [SerializeField] private CheckBox _mirrorZ;

    private QuestionPanel _questionPanel;

    public override void OnInit()
    {
        _questionPanel = PanelManager.GetPanel<QuestionPanel>();
        Presenter.ChangeModeEvent += OnPresenterChangeMode;
    }

    public void OnPresenterChangeMode()
    {
        _modeSwitcher.Switch(Presenter.Mode);
    }

    public void OnSetMirror(bool value)
    {
        Invoker.Execute(new SetMirrorCommand(Mirror));
    }

    public void OnPresenterSetMirror()
    {
        Mirror = Presenter.Mirror;
    }

    public void OnCreateNewProject()
    {
        Project.Release();
        Voxelator.Release();

        PanelManager.CloseAll();
        PanelManager.GetPanel<NewProjectPanel>().Open();
    }

    public void OnOpenProject()
    {
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Select .ndr-file", Project.RootPath, Project.FileExtension, false);

        if (paths.Length > 0)
        {
            if(Project.TryLoad(paths[0]))
            {

            }
        }
    }

    public void OnSaveAs()
    {
        PanelManager.GetPanel<SaveAsPanel>().Open();
    }

    public void OnOpenDirectory()
    {
        Project.OpenDirectory();
    }

    public void LoadOBJ()
    {
        OBJControl.Import();
    }

    public void OnClearOBJ()
    {
        OBJControl.Delete();
    }

    public void OnSwitchMode(int mode)
    {
        Invoker.Execute(new SwitchModeCommand(mode));
    }


    private void OnSetMode(int mode)
    {
        _modeSwitcher.Switch(mode);
    }

}
