public static class ModeManager
{
    private static int _mode = -1;
    private static  Mode[] _modes = new Mode[0];
    private static Mode _curMode => _modes[_mode];

    public static VoidInt SwitchModeEvent;

    static ModeManager()
    {
        _mode = 0;
        _modes = new Mode[]
        {
            new BuildMode(),
            new SelectMode(),
            new EditMode(),
            new OBJMode()
        };

        _modes[_mode].OnEnable();
    }

    public static void SwitchMode(int value)
    {
        if (_mode < 0 || _mode >= _modes.Length) return;

        _modes[_mode].OnDisable();
        _mode = value;
        _modes[_mode].OnEnable();

        SwitchModeEvent?.Invoke(value);
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
