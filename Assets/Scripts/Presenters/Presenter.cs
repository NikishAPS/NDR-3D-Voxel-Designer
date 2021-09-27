using UnityEngine;

public static class Presenter
{
    public static Void ChangeModeEvent;
    public static Void ChangeMirrorEvent;
    public static Void SelectVertexEvent;

    public static int Mode => ModeManager.Mode;
    public static Vector3Bool Mirror => ChunkManager.Mirror;
    public static Vector3 VertexOffset => ChunkManager.SelectedVertex.GetOffset();

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
        ModeManager.SwitchMode(mode);

        ChangeModeEvent?.Invoke();
    }

    public static void SelectVertex(Vector3 vertexOffet)
    {
        SelectVertexEvent?.Invoke();
    }
}
