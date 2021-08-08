using Kimera.Data.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Kimera.Converters
{
    [ValueConversion(typeof(Age), typeof(string))]
    public class AgeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Age)value == Age.All)
            {
                return "All";
            }
            else if ((Age)value == Age.R15)
            {
                return "R15";
            }
            else if ((Age)value == Age.R18)
            {
                return "R18";
            }
            else
            {
                return "All";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((string)value == null)
            {
                return Age.All;
            }
            else
            {
                if ((string)value == "All")
                {
                    return Age.All;
                }
                else if ((string)value == "R15")
                {
                    return Age.R15;
                }
                else if ((string)value == "R18")
                {
                    return Age.R18;
                }
                else
                {
                    return Age.All;
                }
            }
        }
    }
}