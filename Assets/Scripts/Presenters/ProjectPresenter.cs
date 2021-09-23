public static class ProjectPresenter
{
    public static bool Saved
    {
        get => Project.Saved;
        set
        {
            Project.Saved = value;
            StatisticsPanel.Saved = value;
        }
    }
}
