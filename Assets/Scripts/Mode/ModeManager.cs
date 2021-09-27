public static class ModeManager
{
    public static int Mode { get; private set; }

    private static Mode[] _modes;
    private static Mode _curMode => _modes[Mode];

    static ModeManager()
    {
        Mode = 0;
        _modes = new Mode[]
        {
            new BuildMode(),
            new SelectMode(),
            new EditMode(),
            new OBJMode()
        };

        _curMode.OnEnable();
    }

    public static void SwitchMode(int value)
    {
        if (Mode < 0 || Mode >= _modes.Length) return;

        _curMode.OnDisable();
        Mode = value;
        _curMode.OnEnable();
    }

    public static void OnMouseMove()
    {
        _curMode.OnMouseMove();
    }

    public static void OnLMouseDown()
    {
        _curMode.OnLMouseDown();
    }

    public static void OnLMouseUp()
    {
        _curMode.OnLMouseUp();
    }

    public static void OnRMouseDown()
    {
        _curMode.OnRMouseDown();
    }

    public static void OnRMouseUp()
    {
        _curMode.OnRMouseUp();
    }
    
}
