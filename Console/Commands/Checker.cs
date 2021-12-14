using System.Collections.Generic;
using System.Linq;
using TinyVersionUpdaterConsole.Commands.CheckerInnerCommands;

namespace TinyVersionUpdaterConsole.Commands
{
  public class Checker : ICommand
  {
    private readonly List<ICommand> _checkers = new()
    {
      new CheckHasUpdate()
    };
    
    public string Name()
      => "check";

    public Result Execute(string[] args)
    {
      if (args.Length == 0)
        return Result.Fail;
      
      var name = args[0];
      var command = _checkers.FirstOrDefault(x => x.Name().Equals(name));
      var commandArgs = args.Skip(1).ToArray();

      return command?.Execute(commandArgs) ?? Result.Fail;
    }
  }
}