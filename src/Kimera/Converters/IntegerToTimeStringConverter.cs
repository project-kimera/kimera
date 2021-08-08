using System;
using System.Globalization;
using System.Windows.Data;

namespace Kimera.Converters
{
    [ValueConversion(typeof(int), typeof(string))]
    public class IntegerToTimeStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int hours = (int)value / 3600;
            int minutes = ((int)value / 60) - hours * 60;

            switch (culture.TwoLetterISOLanguageName)
            {
                case "en":
                    return $"{hours}시간 {minutes}분";
                case "ko":
                    return $"{hours}hours, {minutes}minutes";
                default:
                    return $"{hours}hours, {minutes}minutes";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotImplementedException();
        }
    }
}