using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPanel : Panel
{
    public void Exit()
    {
        if (Project.Saved)
        {
            Open();
        }
        else
        {
            Project.Ouit();
        }
    }

    public void SaveAndExit()
    {
        if(Project.TryToSave())
        {
            Project.Ouit();
        }
    }
}
