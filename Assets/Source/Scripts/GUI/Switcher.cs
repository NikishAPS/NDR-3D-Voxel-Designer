using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUnityEvents;
using UnityEngine.Events;

public class Switcher : Widget
{
    [SerializeField]
    private int _mode = 0;

    private SwitchButton[] _switchButtons;
    private SwitchSelector _switchSelector;

    public UnityEvent ChangeValue;
    public EventInt InitIntFields;

    public int Value => _switchButtons[_mode].GetValue();

    public override void Init()
    {
        _switchButtons = GetComponentsInChildren<SwitchButton>();
        _switchSelector = GetComponentInChildren<SwitchSelector>();

        for (int i = 0; i < _switchButtons.Length; i++)
            _switchButtons[i].Init();

        SwitchMode(0, _switchButtons[0].GetValue());

        SceneData.EventInput.MouseMove += OnMouseMove;
        SceneData.EventInput.MouseDown0 += OnMouseDown0;

    }

    public override void Tick()
    {

    }

    public void OnMouseMove()
    {
        for (int i = 0; i < _switchButtons.Length; i++)
        {
            if (i != _mode)
                _switchButtons[i].SetDefaultColor();
            else
                _switchButtons[i].SetSelectedColor();

            if (_switchButtons[i].InRect())
            {
                _switchButtons[i].SetHoverColor();
            }
        }
    }

    public void OnMouseDown0()
    {
        for(int i = 0; i < _switchButtons.Length; i++)
        {
            if(_switchButtons[i].InRect())
            {
                //switchButtons[i].SetColor(_selected);
                SwitchMode(i, _switchButtons[i].GetValue());
                break;
            }
        }
    }

    public void SwitchMode(int mode, int value)
    {
        _mode = mode;
        _switchSelector.Set(_switchButtons[mode].GetRect());
        ChangeValue?.Invoke();
        InitIntFields?.Invoke(value);
        ExecuteCommands();
    }

}
