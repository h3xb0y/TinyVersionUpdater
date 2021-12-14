using System;
using System.Collections.Generic;
using System.Linq;
using TinyVersionUpdaterConsole.Commands;

namespace TinyVersionUpdaterConsole
{
  internal class Program
  {
    private static readonly List<ICommand> Commands = new()
    {
      new Checker()
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
      
      var commandArgs = args.Skip(1).ToArray();

      Write(command.Execute(commandArgs));
    }

    private static void Write(Result text) 
      => Console.WriteLine(text.ToString().ToUpper());
  }
}