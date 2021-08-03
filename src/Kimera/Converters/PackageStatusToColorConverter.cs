using Kimera.Data.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Kimera.Converters
{
    [ValueConversion(typeof(PackageStatus), typeof(SolidColorBrush))]
    public class PackageStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PackageStatus status = (PackageStatus)value;

            if (status == PackageStatus.NeedProcessing || status == PackageStatus.Compressed)
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 203, 5));
            }
            else if (status == PackageStatus.FileNotFound || status == PackageStatus.DataNotFound || status == PackageStatus.Exception)
            {
                return new SolidColorBrush(Color.FromArgb(255, 217, 30, 24));
            }
            else if (status == PackageStatus.Playable)
            {
                return new SolidColorBrush(Color.FromArgb(255, 0, 230, 64));
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
