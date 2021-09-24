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

    public static void SwitchMode(int mode)
    {
        PanelManager.GetPanel<ProjectPanel>().ModeSwitcher.Switch(mode);
        ModeManager.SwitchMode(mode);
    }
}
