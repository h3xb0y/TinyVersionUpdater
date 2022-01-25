using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WPF.ViewModel.Converters
{
  public class AppStateToVisibilityConverter : IValueConverter
  {
    [Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)]
    public AppState RequiredState { get; set; }
    
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
       => value is AppState state && state == RequiredState 
         ? Visibility.Visible 
         : Visibility.Collapsed;
    
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
      => value is Visibility.Collapsed
        ? RequiredState
        : default;
  }
}