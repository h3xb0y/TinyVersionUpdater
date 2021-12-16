using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using TinyVersionUpdater;
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

    public IObservable<Result> Execute(string[] args)
    {
      var subject = new Subject<Result>();
      
      var name = args[0];
      var command = _checkers.First(x => x.Name().Equals(name));
      var commandArgs = args.Skip(1).ToArray();

      command
        .Execute(commandArgs)
        .Subscribe(result => subject.OnNext(result), _ => subject.OnNext(Result.Fail));

      return subject;
    }
  }
}