using System;
using System.Reflection;
using System.Text;
using TinyVersionUpdater;

namespace TinyVersionUpdaterConsole.Commands.CheckerInnerCommands
{
  public class CheckHasUpdate : ICommand
  {
    public string Name()
      => "has_update";

    public Result Execute(string[] args)
    {
      if (args.Length == 0)
        return Result.Fail;

      try
      {
        var version = new Version(args[0]);
        var lastVersion = new Version(new Updater().LastVersion());


        return version.CompareTo(lastVersion) == 1 ? Result.Ok : Result.Fail;
      }
      catch (Exception e)
      {
        return Result.Fail;
      }
    }
  }
}