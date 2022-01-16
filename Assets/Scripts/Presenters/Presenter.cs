using UnityEngine;

public static class Presenter
{
    public static Void ChangeModeEvent;
    public static Void ChangeMirrorEvent;
    public static Void EditVertexEvent;

    public static int Mode => ModeManager.Mode;
    //public static Vector3Bool Mirror => ChunkManager.Mirror;
    public static Vector3Bool Mirror => Vector3Bool.False;
    //public static Vertex Vertex => ChunkManager.SelectedVertex;
    public static VertexUnit Vertex => null;

    public static bool Saved
    {
        get => true;
        set
        {
            StatisticsPanel.Saved = value;
        }
    }

    public static void SwitchMode(int mode)
    {
        ModeManager.SwitchMode(mode);

        ChangeModeEvent?.Invoke();
    }

    public static void EditVertex()
    {
        EditVertexEvent?.Invoke();
    }

    public static void SetVertexPosition(Vector3 pivotVertexPosition, Vector3 position)
    {
        //ChunkManager.SetVertexPositionWithUpdateChunks(pivotVertexPosition, position);
    }

}
