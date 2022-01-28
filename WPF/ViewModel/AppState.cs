namespace WPF.ViewModel
{
  public enum AppState
  {
    CanCheck,
    Checking,
    VersionFound,
    VersionIsActual,
    NewVersionDownloading,
    VersionUpgradingFailed,
    VersionUpgradedSuccessful
  }
}