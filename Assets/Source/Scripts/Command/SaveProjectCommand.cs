public class SaveProjectCommand : Command
{
    public override void Execute()
    {
        if(VoxelatorManager.Project.TryToSave())
        {
            Project.Saved = true;
        }
    }
}
