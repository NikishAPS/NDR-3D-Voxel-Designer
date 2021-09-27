using System.Collections;
using System.Collections.Generic;
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

    public void TryCreateNewProject()
    {
        if (!Project.Saved)
        {
            _questionPanel.Open();
            _questionPanel.SetTitles("Save before creating a new project?", "Save", "Don't Save", "Cacel");
            _questionPanel.SetConfirmMethod(SaveProject);
            _questionPanel.SetRejectMethod(CreateNewProject);
        }
        else
        {
            CreateNewProject();
        }
    }

    public void SaveProject()
    {
        if (Project.TryToSave())
        {
            _questionPanel.Close();
        }
    }

    public void LoadOBJ()
    {
        OBJControl.Import();
    }

    public void OnSwitchMode(int mode)
    {
        Invoker.Execute(new SwitchModeCommand(mode));
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

    private void OnSetMode(int mode)
    {
        _modeSwitcher.Switch(mode);
    }

    private void CreateNewProject()
    {

    }
}
