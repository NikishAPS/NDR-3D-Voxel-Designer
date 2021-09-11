using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Invoker
{
    private static Stack<Command> _commands = new Stack<Command>();

    public static void Execute(Command command)
    {
        command.Execute();
        _commands.Push(command);

        StatisticsPresenter.IsProjectSaved = false;
    }
}
