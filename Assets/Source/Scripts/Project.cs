using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Project : MonoBehaviour
{
    [SerializeField]
    private Panel exitPanel;

    private bool isExit = false;

    private void Awake()
    {
        Application.wantsToQuit += ExitProcessing;
    }

    private bool ExitProcessing()
    {
        exitPanel.Open();
        return isExit;
    }

    public void ApplicationQuit()
    {
        isExit = true;
        Application.Quit();
    }
}
