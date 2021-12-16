using System;
using System.Reactive.Subjects;
using TinyVersionUpdater;

namespace TinyVersionUpdaterConsole.Commands.CheckerInnerCommands
{
  public class CheckHasUpdate : ICommand
  {
    public string Name()
      => "has_update";

    public IObservable<Result> Execute(string[] args)
    {
      var result = new Subject<Result>();

      new Updater()
        .LastVersion()
        .Subscribe(lastVersion =>
        {
          if (args.Length == 0)
          {
            result.OnNext(Result.Fail);
            return;
          }
          
          var version = new Version(args[0]);
          result.OnNext(lastVersion.CompareTo(version) == 1 ? Result.Ok : Result.Fail);
        });

      return result;
    }

    public void PostExecute()
    {
    }
  }
}