using System;
using System.Collections.Generic;

public class ReportField<T>
{
    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            foreach (Action<T> action in _actions)
                action(_value);
        }

    }

    public void BindAction(Action<T> action) => _actions.Add(action);
    public void RemoveAction(Action<T> action) => _actions.Remove(action);

    private T _value;
    private List<Action<T>> _actions = new List<Action<T>>();

}
