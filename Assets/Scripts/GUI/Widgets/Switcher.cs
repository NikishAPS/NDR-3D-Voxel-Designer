using UnityEngine;
using CustomUnityEvents;

public class Switcher : Widget
{
    public int Value
    {
        get => _value;
        set
        {
            _value = value;
        }
    }

    [SerializeField] private int _value;
    [SerializeField] private EventInt SwitchEvent;
    [SerializeField] private SwitcherButton[] _switcherButtons;
    [SerializeField] private SwitcherButton _curSwitcherButton;
    [SerializeField] private SwitcherButton _selectedSwitcherButton;


    public override void OnInitImage()
    {
        Init();
        foreach (SwitcherButton button in _switcherButtons)
            button.Init();
    }

    public override bool Inside()
    {
        foreach (SwitcherButton button in _switcherButtons)
        {
            if (button.Inside(InputEvent.MousePosition))
            {
                _curSwitcherButton = button;
                return true;
            }
        }

        _curSwitcherButton = null;
        return false;
    }

    public override void SetColor(Color color)
    {
        foreach (SwitcherButton button in _switcherButtons)
        {
            if(button != _selectedSwitcherButton)
                button.SetColor(_defaultColor);
            else
                button.SetColor(_selectedColor);
        }

        _curSwitcherButton?.SetColor(color);
    }



    public override void OnClick()
    {
        if(_curSwitcherButton != null)
        {
            _selectedSwitcherButton = _curSwitcherButton;
            _value = _selectedSwitcherButton.Value;
            SwitchEvent?.Invoke(_value);
        }
    }
   
    public void SwitchButtons(SwitcherButton button)
    {
        button?.SetColor(_defaultColor);
        _curSwitcherButton = button;
    }

    private void Init()
    {
        _switcherButtons = GetComponentsInChildren<SwitcherButton>();
        _curSwitcherButton = null;
        _selectedSwitcherButton = _switcherButtons[0];
        _value = _selectedSwitcherButton.Value;
    }

}
