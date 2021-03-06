using System.Collections.Generic;
using UnityEngine;

public static class Invoker
{
    private static Stack<ICommand> _commands = new Stack<ICommand>();
    private static Stack<ICommand> _com = new Stack<ICommand>();

    static Invoker()
    {
        //InputEvent.ZDown += Undo;
    }

    public static void Execute(ICommand command)
    {
        //Project.Saved.Value = false;
        command.Execute();
        _commands.Push(command);
    }

    public static void Undo()
    {
        ICommand command = _commands.Pop();
        command.Undo();
        _com.Push(command);
    }
}
