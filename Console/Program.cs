using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TinyVersionUpdater;
using TinyVersionUpdaterConsole.Commands;

namespace TinyVersionUpdaterConsole
{
  internal static class Program
  {
    private static readonly List<ICommand> Commands = new()
    {
      new Checker(), new Worker()
    };

    private static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        Write(Result.Fail);
        return;
      }

      var commandName = args[0];

      var command = Commands.FirstOrDefault(x => x.Name().Equals(commandName));
      if (command == null)
      {
        Write(Result.Fail);
        return;
      }

      var commandArgs = args
        .Skip(1)
        .ToArray();

      var hasResponse = false;

      command
        .Execute(commandArgs)
        .Subscribe(result =>
        {
          Write(result);
          hasResponse = true;
        });

      while (!hasResponse)
      {
      }
      
      command.PostExecute();
    }

    private static void Write(Result text)
      => Console.WriteLine(text.ToString().ToUpper());
  }
}