using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomUnityEvents;
using UnityEngine.Events;

public class Widget : MonoBehaviour
{
    [SerializeField]
    private Command[] _commands;

    public RectTransform RectTransform => _rectTransform;
    protected RectTransform _rectTransform;


    
    public virtual void Init()
    {
        BaseInit();
    }

    public virtual void Tick() { }

    public void ExecuteCommands()
    {
        if (_commands != null)
        {
            foreach (Command command in _commands)
            {
                if (command != null)
                    Invoker.Execute(command);
            }
        }
    }


    protected void BaseInit()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
}
