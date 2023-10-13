using Core.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Vectorly.UI
{
   public class IntToModeConverter : IValueConverter
   {
      public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value;

      public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (Mode)value;

   }
}
