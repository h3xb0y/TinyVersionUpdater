using System;
using System.Reactive.Subjects;

namespace TinyVersionUpdaterConsole
{
  public interface ICommand
  {
    string Name();
    
    IObservable<Result> Execute(string[] args);
  }
}