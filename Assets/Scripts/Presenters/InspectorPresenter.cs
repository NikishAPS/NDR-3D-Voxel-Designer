using UnityEngine;

public static class InspectorPresenter
{
    private static InspectorPanel _inspectorPanel;

    static InspectorPresenter()
    {
        _inspectorPanel = Object.FindObjectOfType<InspectorPanel>();
    }


}
