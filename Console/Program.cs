using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TinyVersionUpdater;
using TinyVersionUpdaterConsole.Commands;

namespace TinyVersionUpdaterConsole
{
  internal static class Program
  {
    private static readonly List<ICommand> Commands = new()
    {
      new Checker(),
      new Worker()
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

      if (commandName == new Worker().Name())
        Process.Start(Updater.Config.ExecutablePath);
    }

    private static void Write(Result text)
      => Console.WriteLine(text.ToString().ToUpper());
  }
}