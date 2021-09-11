using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectPanel : Panel
{
    private QuestionPanel _questionPanel;

    public override void OnInit()
    {
        _questionPanel = PanelManager.GetPanel<QuestionPanel>();
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
        if(Project.TryToSave())
        {
            _questionPanel.Close();
        }
    }

    public void LoadOBJ()
    {
        OBJControl.Import();
    }

    
    private void CreateNewProject()
    {

    }
}
