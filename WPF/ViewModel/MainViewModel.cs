using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TinyVersionUpdater;
using WPF.Annotations;

namespace WPF.ViewModel
{
  public sealed class MainViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    
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

    public void CheckNewVersion()
    {
      CurrentState = AppState.Checking;
      
      new Updater()
        .IsNeedUpdate()
        .Subscribe(isNeedUpdate =>
        {
          CurrentState = isNeedUpdate ? AppState.VersionFound : AppState.VersionIsActual;
        });
    }

    public void DownloadNewVersion()
    {
      CurrentState = AppState.NewVersionDownloading;
      
      new Updater()
        .LoadLastVersion()
        .Subscribe(result =>
        {
          CurrentState = result == Result.Ok 
            ? AppState.VersionUpgradedSuccessful 
            : AppState.VersionUpgradingFailed;
        });
    }
    
    [NotifyPropertyChangedInvocator]
    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}