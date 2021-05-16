using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switcher : Widget
{
    [SerializeField]
    private int mode = 0;

    [SerializeField]
    private Color _default, _hover, _selected;

    private Button[] buttons;

    public override void Init()
    {
        buttons = GetComponentsInChildren<Button>();
        SwitchMode(0);
    }

    public override void Tick()
    {

    }

    public void SwitchMode(int value)
    {
        buttons[mode].SetDefaultColor(_default);
        mode = value;
        buttons[mode].SetDefaultColor(_selected);
    }

    public void UpdateButtons()
    {
        foreach(Button button in buttons)
        {
            button.Tick();
        }
    }
}
