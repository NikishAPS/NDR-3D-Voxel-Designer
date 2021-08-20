using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartProjectCommand : Command
{
    [SerializeField]
    private Panel _savePanel;
    [SerializeField]
    private Panel _newProjectPanel;
    [SerializeField]
    private Panel _projectPanel;
    [SerializeField]
    private Panel _inspectorPanel;
    [SerializeField]
    private Panel _statisticsPanel;

    public override void Execute()
    {
        if(Project.Saved)
        {
            _savePanel.Close();
            _projectPanel.Close();
            _inspectorPanel.Close();
            _statisticsPanel.Close();
            _newProjectPanel.Open();

            SceneData.ModeControl.Disable();
            ChunksManager.Release();
            GridManager.Grids[Direction.Down].Active = false;
        }
        else
        {
            _savePanel.Open();
        }
    }
}
