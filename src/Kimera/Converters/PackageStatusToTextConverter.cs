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
                return "추가 작업 필요";
            }
            else if (status == PackageStatus.Compressed)
            {
                return "압축 상태";
            }
            else if (status == PackageStatus.FileNotFound)
            {
                return "파일 미존재";
            }
            else if (status == PackageStatus.DataNotFound)
            {
                return "데이터 미존재";
            }
            else if (status == PackageStatus.Exception)
            {
                return "예외 발생";
            }
            else if (status == PackageStatus.Playable)
            {
                return "실행 가능";
            }
            else
            {
                return "알 수 없음";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
