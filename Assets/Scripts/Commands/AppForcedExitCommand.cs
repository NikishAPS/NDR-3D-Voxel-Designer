public class AppForcedExitCommand : Command
{
    public override void Execute()
    {
        Project.Saved = true;
        Project.Ouit();
    }
}
