using System;
using System.Windows;
using WPF.ViewModel;

namespace WPF
{
  public partial class MainWindow
  {
    public MainWindow()
      => InitializeComponent();

    private void MainWindow_OnInitialized(object? sender, EventArgs e)
      => DataContext = new MainViewModel();

    private void CheckVersion_OnClick(object sender, RoutedEventArgs e)
    {
      if (sender is FrameworkElement element) 
        ((MainViewModel) element.DataContext).CheckNewVersion();
    }
  }
}