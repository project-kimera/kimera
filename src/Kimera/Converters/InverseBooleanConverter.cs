using System;
using System.Globalization;
using System.Windows.Data;

namespace Kimera.Converters
{
    [ValueConversion(typeof(bool), typeof(bool?))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool?)value == null)
            {
                return null;
            }
            else
            {
                return !(bool?)value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool?)value == null)
            {
                return null;
            }
            else
            {
                return !(bool)value;
            }
        }
    }
}
