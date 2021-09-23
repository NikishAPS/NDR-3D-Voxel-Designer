using UnityEngine;
using CustomUnityEvents;

public class Switcher : Widget
{
    public int Value => _value;
    [SerializeField] private int _value;
    private SwitcherButton _selectedButton;
    [SerializeField] private EventInt SwitchEvent;



    public override void OnInit()
    {
        _selectedButton = GetComponentsInChildren<SwitcherButton>()[0];
        SetButtonColor(_selectedButton, _selectedColor);
        _value = _selectedButton.Value;
    }

    public void Switch(SwitcherButton button)
    {
        SetButtonColor(_selectedButton, _defaultColor);
        _selectedButton = button;
        SetButtonColor(_selectedButton, _selectedColor);

        _value = button.Value;

        SwitchEvent?.Invoke(_value);
    }

    private void SetButtonColor(SwitcherButton switcherButton, Color color)
    {
        switcherButton.DefaultColor = color;
        switcherButton.Image.color = color;
    }
}
