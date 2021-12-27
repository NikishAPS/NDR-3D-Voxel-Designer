﻿using UnityEngine;
using NDR.UI;

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

    public void OnOpenDirectory()
    {
        string path = Project.GetFolderPath("Save Location");
        
        if(path != null)
        _projectSaveLocationField.SetValue(path);
    }

    public void OnCreateNewProject()
    {
        ScreenAxes.Active = true;

        Project.Init(_projectNameField.String, _projectSaveLocationField.String);
        Voxelator.Init(new Vector3Int(_widthField.Int, _heightField.Int, _depthField.Int), _switcherIncrement.Value);
        GridManager.Size = Voxelator.VoxelChunkManager.FieldSize;

        CameraController.Position = Vector3.zero;
        CameraController.MainCamera.nearClipPlane = -Voxelator.FieldSize.Max();
        CameraController.MainCamera.farClipPlane = Voxelator.FieldSize.Max();

        PanelManager.GetPanel<ViewPanel>().Open();
        PanelManager.GetPanel<ProjectPanel>().Open();
        PanelManager.GetPanel<StatisticsPanel>().Open();
        PanelManager.GetPanel<InspectorPanel>().Open();
        Close();
    }

    public void OnOpenExisting()
    {
        if (Project.TryToLoad())
        {
            Close();
        }
    }

}
