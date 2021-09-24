using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectPanel : Panel
{
    public Switcher ModeSwitcher => _modeSwitcher;

    [SerializeField] private QuestionPanel _questionPanel;
    [SerializeField] private Switcher _modeSwitcher;

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
        if(Project.TryToSave())
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
        ModeManager.SwitchMode(mode);
    }

    
    private void CreateNewProject()
    {

    }
}
