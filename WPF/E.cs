using System;
using WPF.ViewModel;

namespace WPF
{
  public static class E
  {
    public static string ToText(this AppState state)
    {
      return state switch
      {
        AppState.Checking => "Checking new version",
        AppState.CanCheck => "Check new version?",
        AppState.VersionFound => "New version found. Download now?",
        AppState.VersionIsActual => "Current version is actual",
        _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
      };
    }
  }
}