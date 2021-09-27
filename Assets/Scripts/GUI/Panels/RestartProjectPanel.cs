using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartProjectPanel : Panel
{
    public void Restart()
    {
        if (!Project.Saved)
        {
            Open();
        }
        else
        {
            ForceRestart();
        }
    }

    public void ForceRestart()
    {
        foreach (Panel panel in PanelManager.Panels)
        {
            if (panel.GetType() != typeof(NewProjectPanel))
                panel.Close();
            else
                panel.Open();
        }

        ChunkManager.Release();
        GridManager.Grids[Direction.Down].Active = false;
    }
}
