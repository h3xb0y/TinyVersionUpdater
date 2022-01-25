using System.Windows;

namespace WPF
{
  public partial class App : Application
  {
    private void App_OnStartup(object sender, StartupEventArgs e)
      => Global.AppArgs = e.Args;
  }
}