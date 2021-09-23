using UnityEngine;

public class ModeManager : MonoBehaviour, IMouseMove, ILMouseDown, ILMouseUp
{
    public static ModeManager This { get; private set; }
    private static int _mode = -1;
    private static  Mode[] _modes = new Mode[0];



    public void Awake()
    {
        This = FindObjectOfType<ModeManager>();

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

    public void SwitchMode(int value)
    {
        if (_mode < 0 || _mode >= _modes.Length) return;

        _modes[_mode].OnDisable();
        _mode = value;
        _modes[_mode].OnEnable();
    }

    public void OnMouseMove()
    {
        _modes[_mode].OnMouseMove();
    }

    public void OnLMouseDown()
    {
        _modes[_mode].OnLMouseDown();
    }

    public void OnLMouseUp()
    {
        _modes[_mode].OnLMouseUp();
    }
    
}
