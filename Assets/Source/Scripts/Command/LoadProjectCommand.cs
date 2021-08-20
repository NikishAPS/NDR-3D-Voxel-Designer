using UnityEngine;

public class LoadProjectCommand : Command
{
    [SerializeField]
    private Panel _panel;

    public override void Execute()
    {
        if (VoxelatorManager.Project.TryToLoad())
        {
            _panel.Close();
        }
    }
}
