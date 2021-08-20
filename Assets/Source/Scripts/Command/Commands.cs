using UnityEngine;

public static class Commands
{
    public static readonly StartNewProjectCommand StartNewProjectCommand;
    public static readonly SaveProjectCommand SaveProjectCommand;
    public static readonly LoadProjectCommand LoadProjectCommand;
    public static readonly SetSaveLocationCommand SetSaveLocationCommand;
    public static readonly SetVoxelIdCommand SetVoxelIdCommand;
    public static readonly CreateVoxelsCommand CreateVoxelsCommand;
    public static readonly SelectVoxelCommand SelectVoxelCommand;

    public static readonly AppExitCommand AppExitCommand;
    public static readonly AppForcedExitCommand AppForcedExitCommand;

    static Commands()
    {
        StartNewProjectCommand = Object.FindObjectOfType<StartNewProjectCommand>();
        SaveProjectCommand = Object.FindObjectOfType<SaveProjectCommand>();
        LoadProjectCommand = Object.FindObjectOfType<LoadProjectCommand>();
        SetSaveLocationCommand = Object.FindObjectOfType<SetSaveLocationCommand>();
        SetVoxelIdCommand = Object.FindObjectOfType<SetVoxelIdCommand>();
        CreateVoxelsCommand = Object.FindObjectOfType<CreateVoxelsCommand>();
        SelectVoxelCommand = Object.FindObjectOfType<SelectVoxelCommand>();
        AppExitCommand = Object.FindObjectOfType<AppExitCommand>();
        AppForcedExitCommand = Object.FindObjectOfType<AppForcedExitCommand>();
    }
}
