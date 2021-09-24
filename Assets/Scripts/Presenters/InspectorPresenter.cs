using UnityEngine;

public static class InspectorPresenter
{
    private static InspectorPanel _inspectorPanel;

    static InspectorPresenter()
    {
        _inspectorPanel = PanelManager.GetPanel<InspectorPanel>();
    }

    public static void SwitchTab(int tabIndex)
    {

    }


}
