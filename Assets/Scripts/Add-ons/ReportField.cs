using System;
using System.Collections.Generic;

public struct ReportField<T>
{
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            foreach (Action<T> action in _actions)
                if (value != null)
                    action(_value);
        }
    }

    private T _value;
    private List<Action<T>> _actions;

    public ReportField(T value)
    {
        _value = value;
        _actions = new List<Action<T>>();
    }

    public void BindAction(Action<T> action) => _actions.Add(action);
    public void RemoveAction(Action<T> action) => _actions.Remove(action);

}
