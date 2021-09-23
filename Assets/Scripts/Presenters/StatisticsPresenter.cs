using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatisticsPresenter : MonoBehaviour
{
    private static StatisticsPanel _statisticsPanel;


    public static bool IsProjectSaved
    {
        get => Project.Saved;
        set
        {
            Project.Saved = value;
            _statisticsPanel.Saved = value;
        }
    }



    private void Start()
    {
        _statisticsPanel = PanelManager.GetPanel<StatisticsPanel>();
    }

}
