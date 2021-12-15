using System;
using System.Reactive.Subjects;
using TinyVersionUpdater;

namespace TinyVersionUpdaterConsole.Commands
{
  public class Worker : ICommand
  {
    public string Name()
      => "update";

    public IObservable<Result> Execute(string[] args)
    {
      var result = new Subject<Result>();
      
      Console.WriteLine("Checking is need load new version...");
      
      new Updater()
        .IsNeedUpdate()
        .Subscribe(isNeedUpdate =>
        {
          if (!isNeedUpdate)
          {
            result.OnNext(Result.Fail);
            return;
          }
          
          Console.WriteLine("Loading new version...");

          new Updater()
            .LoadLastVersion()
            .Subscribe(isUpdated => result.OnNext(isUpdated));
        });

      return result;
    }
  }
}