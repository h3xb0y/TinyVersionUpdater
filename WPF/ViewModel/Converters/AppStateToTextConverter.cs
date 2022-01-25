using System;
using System.Globalization;
using System.Windows.Data;

namespace WPF.ViewModel.Converters
{
  public class AppStateToTextConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      => value is AppState state
        ? state.ToText()
        : null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}