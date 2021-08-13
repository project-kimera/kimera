using Kimera.Data.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Kimera.Converters
{
    [ValueConversion(typeof(PackageStatus), typeof(string))]
    public class PackageStatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            PackageStatus status = (PackageStatus)value;

            if (status == PackageStatus.NeedProcessing)
            {
                return (string)App.Current.Resources["CONV_PSTOTEXT_NEED_PROCESSING_MSG"];
            }
            else if (status == PackageStatus.Compressed)
            {
                return (string)App.Current.Resources["CONV_PSTOTEXT_COMPRESSED_MSG"];
            }
            else if (status == PackageStatus.FileNotFound)
            {
                return (string)App.Current.Resources["CONV_PSTOTEXT_FILE_NOT_FOUND_MSG"];
            }
            else if (status == PackageStatus.DataNotFound)
            {
                return (string)App.Current.Resources["CONV_PSTOTEXT_DATA_NOT_FOUND_MSG"];
            }
            else if (status == PackageStatus.InvalidPackage)
            {
                return (string)App.Current.Resources["CONV_PSTOTEXT_INVALID_PACKAGE_MSG"];
            }
            else if (status == PackageStatus.Exception)
            {
                return (string)App.Current.Resources["CONV_PSTOTEXT_EXCEPTION_MSG"];
            }
            else if (status == PackageStatus.Playable)
            {
                return (string)App.Current.Resources["CONV_PSTOTEXT_PLAYABLE_MSG"];
            }
            else
            {
                return (string)App.Current.Resources["CONV_PSTOTEXT_UNKNOWN_MSG"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}