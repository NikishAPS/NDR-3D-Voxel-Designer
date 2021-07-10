using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUnityEvents;
using UnityEngine.Events;

public class Switcher : Widget
{
    [SerializeField]
    private int _mode = 0;

    [SerializeField]
    private Color _default, _hover, _selected;

    private SwitchButton[] switchButtons;

    public UnityEvent ChangeValue;
    public EventInt InitIntFields;

    public override void Init()
    {
        switchButtons = GetComponentsInChildren<SwitchButton>();

        for(int i = 0; i < switchButtons.Length; i++)
            switchButtons[i].Init();

        SwitchMode(0, switchButtons[0].GetValue());

        SceneData.EventInput.MouseMove += OnMouseMove;
        SceneData.EventInput.MouseDown0 += OnMouseDown0;
    }

    public override void Tick()
    {

    }

    public void OnMouseMove()
    {
        for (int i = 0; i < switchButtons.Length; i++)
        {
            if (i != _mode)
                switchButtons[i].SetColor(_default);
            else
                switchButtons[i].SetColor(_selected);

            if (switchButtons[i].InRect())
            {
                switchButtons[i].SetColor(_hover);
            }
        }
    }

    public void OnMouseDown0()
    {
        for(int i = 0; i < switchButtons.Length; i++)
        {
            if(switchButtons[i].InRect())
            {
                //switchButtons[i].SetColor(_selected);
                SwitchMode(i, switchButtons[i].GetValue());
                break;
            }
        }
    }

    public void SwitchMode(int mode, int value)
    {
        _mode = mode;
        ChangeValue?.Invoke();
        InitIntFields?.Invoke(value);
    }

}
