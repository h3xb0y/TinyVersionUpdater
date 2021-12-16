using System;
using TinyVersionUpdater;

namespace TinyVersionUpdaterConsole
{
  public interface ICommand
  {
    string Name();
    
    IObservable<Result> Execute(string[] args);

    void PostExecute();
  }
}