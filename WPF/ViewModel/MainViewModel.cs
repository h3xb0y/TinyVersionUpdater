using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using TinyVersionUpdaterConsole;
using TinyVersionUpdaterConsole.Commands;
using WPF.Annotations;

namespace WPF.ViewModel
{
  public sealed class MainViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    
    private static readonly List<ICommand> Commands = new()
    {
      new Checker(), new Worker()
    };
    
    private AppState _currentState;

    public AppState CurrentState
    {
      get => _currentState;
      set
      {
        if (value == _currentState)
          return;

        _currentState = value;
        OnPropertyChanged(nameof(CurrentState));
      }
    }

    public MainViewModel()
      => Initialize();

    public void CheckNewVersion()
    {
      CurrentState = AppState.Checking;
    }

    private void Initialize()
    {
      var args = Global.AppArgs;
      
      if (args == null || args.Length == 0)
      {
        CurrentState = AppState.CanCheck;
        return;
      }
      

      var commandName = args[0];

      var command = Commands.FirstOrDefault(x => x.Name().Equals(commandName));
      if (command == null)
      {
        CurrentState = AppState.CanCheck;
        return;
      }
      
      var commandArgs = args
        .Skip(1)
        .ToArray();

      command.Execute(commandArgs);
    }
    
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}